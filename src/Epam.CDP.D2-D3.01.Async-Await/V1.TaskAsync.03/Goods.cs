using System;
using System.Windows.Input;
using Common;

namespace V1.TaskAsync._03
{
    public class Goods
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public double Cost { get; set; }

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
    }
}
