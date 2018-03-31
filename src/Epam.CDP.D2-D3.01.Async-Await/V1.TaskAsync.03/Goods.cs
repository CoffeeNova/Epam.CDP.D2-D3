using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Common;
using V1.TaskAsync._03.Annotations;

namespace V1.TaskAsync._03
{
    public class Goods : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        private double _cost;

        public double Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                OnPropertyChanged();
            }
        }

        public string Image { get; set; }

        public ButtonAction ButtonContent { get; set; }

        public ICommand ButtonCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => true,
                    CommandAction = () => { ActionEvent?.Invoke(); }
                };
            }
        }

        public event Action ActionEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
