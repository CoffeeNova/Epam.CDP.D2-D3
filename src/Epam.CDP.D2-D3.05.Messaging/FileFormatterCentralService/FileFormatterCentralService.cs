using System;
using System.IO;
using System.Threading.Tasks;
using FileFormatter.Common;
using MessagingApi;
using MessagingApi.AzureServiceBus;
using Newtonsoft.Json;
using Topshelf;

namespace FileFormatterCentralService
{
    public class FileFormatterCentralService
    {
        private readonly IMessagesController _fileQueueController;
        private readonly IFileAssembler _fileAssembler;
        private readonly IServiceBusConfiguration _statusQueueConfig;
        private readonly IServiceBusConfiguration _controlQueueConfig;
        private readonly IFileFormatterSettingsExchanger _settingsExchanger;
        private ServiceState _state;
        private DateTime _settingsFileLastWriteTime;

        public FileFormatterCentralService(IFileAssembler fileAssembler, IServiceBusConfiguration fileQueueConfig, IServiceBusConfiguration statusQueueConfig, IServiceBusConfiguration controlQueueConfig)
        {
#if DEBUG
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            OutputPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\", OutputFolderName));
            SystemFilesPath = Path.GetFullPath(Path.Combine(appDir, "..\\..\\..\\"));
            DefaultNewPageTimeout = 10;
#endif

            _statusQueueConfig = statusQueueConfig;
            _controlQueueConfig = controlQueueConfig;
            _fileQueueController = new AzureMessagesQueueController(fileQueueConfig);
            _fileAssembler = fileAssembler;
            _settingsExchanger = new FileFormatterSettingsExchanger();
        }

        public bool Start(HostControl hostControl)
        {
            if (_state == ServiceState.Started)
                return true;

            if (!TryCreateQueues() || !TryCreateConfigFile())
            {
                hostControl.Stop();
                return true;
            }

            _fileAssembler.StartAssebling(OutputPath, _fileQueueController);

            _settingsExchanger.SubscribeToSettingsSender(GetSettingsFromFile, _controlQueueConfig);
            _settingsExchanger.SubscribeToSettingsReceiver(OnReceiveSettingsFromClients, _statusQueueConfig);

            _state = ServiceState.Started;
            return true;
        }

        public void Stop()
        {
            if (_state == ServiceState.Stopped)
                return;

            _fileAssembler.StopAssebling();
            _settingsExchanger.UnSubscribeFromSettingsSender();
            _settingsExchanger.UnSubscribeFromSettingsReceiver();

            _state = ServiceState.Stopped;
        }

        private bool TryCreateQueues()
        {
            try
            {
                var statusQueueController = new AzureMessagesTopicController(_statusQueueConfig);
                var controlQueueController = new AzureMessagesTopicController(_controlQueueConfig);

                Task.WaitAll(_fileQueueController.CreateQueueAsync(),
                            statusQueueController.CreateQueueAsync(),
                            controlQueueController.CreateQueueAsync());

                return true;
            }
            catch
            {
                //to log
                return false;
            }
        }

        private bool TryCreateConfigFile()
        {
            var configFilePath = Path.Combine(SystemFilesPath, FileFormatterConfigFileName);
            if (File.Exists(configFilePath))
                return true;

            try
            {
                using (var file = File.CreateText(configFilePath))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, new FileFormatterSettings
                    {
                        NewPageTimeOut = DefaultNewPageTimeout
                    });
                }

                return true;
            }
            catch
            {
                //to log cannot create config file
                return false;
            }
        }

        private FileFormatterSettings GetSettingsFromFile()
        {
            try
            {
                var configFilePath = Path.Combine(SystemFilesPath, FileFormatterConfigFileName);
                var lastWriteTime = File.GetLastWriteTime(configFilePath);
                if (_settingsFileLastWriteTime == lastWriteTime)
                    return null;

                _settingsFileLastWriteTime = lastWriteTime;
                var json = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<FileFormatterSettings>(json);
            }
            catch
            {
                //to log "config file with errors"
                return null;
            }
        }

        private async Task OnReceiveSettingsFromClients(FileFormatterSettings settings)
        {
            using (var file = File.AppendText(Path.Combine(SystemFilesPath, ClientsSettingsFileName)))
            {
                var json = JsonConvert.SerializeObject(settings);
                await file.WriteLineAsync(json);
                await file.FlushAsync();
            }
        }


        private enum ServiceState
        {
            Stopped,
            Started
        }

        public static string OutputPath { get; set; }
        public static string SystemFilesPath { get; set; }

        public static int DefaultNewPageTimeout { get; set; } //in ms

        private const string OutputFolderName = "Output";
        private const string ClientsSettingsFileName = "ClientsSettings.log";
        private const string FileFormatterConfigFileName = "FileFormatterConfig.json";
    }
}
