using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Common;
using V1.TaskAsync._02.Annotations;

namespace V1.TaskAsync._02
{
    public class UrlListViewItem : INotifyPropertyChanged
    {
        public string Url { get; set; }

        private DownloadStatus _downloadStatus;

        public DownloadStatus DownloadStatus
        {
            get => _downloadStatus;
            set
            {
                _downloadStatus = value;
                OnPropertyChanged();
            }
        }

        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public ICommand CancelCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => true,
                    CommandAction = () => { CancellationTokenSource?.Cancel(); }
                };
            }
        }

        public Task<string> Content { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
