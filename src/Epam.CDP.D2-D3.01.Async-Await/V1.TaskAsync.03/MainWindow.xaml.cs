using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using V1.TaskAsync._03.Annotations;

namespace V1.TaskAsync._03
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged

    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            FillItemListWithTestData();
            BucketList.CollectionChanged += BucketList_CollectionChanged;
        }

        private void FillItemListWithTestData()
        {
            var bus = new Goods
            {
                Name = "Bus",
                Description = "School bus",
                Cost = 50000,
                Image = "/cars/icons8-bus-64.png",
                ButtonContent = ButtonAction.Buy
            };
            var limousine = new Goods
            {
                Name = "Limousine",
                Description = "Elite class",
                Cost = 10000,
                Image = "/cars/icons8-limousine-filled-50.png",
                ButtonContent = ButtonAction.Buy
            };
            var pickup = new Goods
            {
                Name = "Pickup",
                Description = "Workhorse",
                Cost = 20000,
                Image = "/cars/icons8-pickup-48.png",
                ButtonContent = ButtonAction.Buy
            };
            var suv = new Goods
            {
                Name = "Suv",
                Description = "Sport class",
                Cost = 35000,
                Image = "/cars/icons8-suv-48.png",
                ButtonContent = ButtonAction.Buy
            };
            var tesla = new Goods
            {
                Name = "Tesla",
                Description = "Electric car",
                Cost = 50000,
                Image = "/cars/icons8-tesla-model-x-filled-50.png",
                ButtonContent = ButtonAction.Buy
            };
            bus.ActionEvent += () => Bus_ActionEvent(bus);
            limousine.ActionEvent += () => Bus_ActionEvent(limousine);
            pickup.ActionEvent += () => Bus_ActionEvent(pickup);
            suv.ActionEvent += () => Bus_ActionEvent(suv);
            tesla.ActionEvent += () => Bus_ActionEvent(tesla);
            GoodsList.Add(bus);
            GoodsList.Add(limousine);
            GoodsList.Add(pickup);
            GoodsList.Add(suv);
            GoodsList.Add(tesla);
        }

        private void Bus_ActionEvent(Goods goods)
        {
            if (goods.ButtonContent == ButtonAction.Buy)
            {
                var buckeGoods = new Goods
                {
                    Name = goods.Name,
                    Cost = goods.Cost,
                    Description = goods.Description,
                    Image = goods.Image,
                    ButtonContent = ButtonAction.Remove
                };
                buckeGoods.ActionEvent += () => Bus_ActionEvent(buckeGoods);
                BucketList.Add(buckeGoods);
            }
            else if (goods.ButtonContent == ButtonAction.Remove)
            {
                // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
                goods.ActionEvent -= () => Bus_ActionEvent(goods);
                BucketList.Remove(goods);
            }
        }

        private void BucketList_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var totalCost = 0.0;
            foreach (var item in BucketList)
            {
                totalCost += item.Cost;
            }

            TotalCost = totalCost.ToString(CultureInfo.InvariantCulture);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Goods> GoodsList { get; set; } = new ObservableCollection<Goods>();
        public ObservableCollection<Goods> BucketList { get; set; } = new ObservableCollection<Goods>();

        private string _totalCost;

        public string TotalCost
        {
            get => _totalCost;
            set
            {
                _totalCost = value;
                OnPropertyChanged();
            }
        }
    }
}
