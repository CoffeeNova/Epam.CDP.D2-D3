using FileFormatter.Common;
using FileFormatter.Common.ServiceBusConfiguration;
using FileFormatterService.ServiceBusConfiguration;
using Unity;

namespace FileFormatterService
{
    internal class Bootstrapper : IBootstrapper
    {
        public IUnityContainer Container { get; }

        public Bootstrapper(IUnityContainer container)
        {
            Container = container;
            Register();
        }

        private void Register()
        {
            Container.RegisterType<IServiceConfigurator, ServiceConfigurator>();
            Container.RegisterWithInterceptor<IServiceBusConfigurationFactory, ServiceBusConfigurationFactory>();
            Container.RegisterWithInterceptor<IFileBuilderFactory, FileBuilderFactory>();
            Container.RegisterWithInterceptor<IFileFormatterService, FileFormatterService>();
            Container.RegisterWithInterceptor<IFileFormatterSettingsExchanger, FileFormatterSettingsExchanger>();
            Container.RegisterWithInterceptor<IImageWatcherFactory, ImageWatcherFactory>();
        }
    }
}
