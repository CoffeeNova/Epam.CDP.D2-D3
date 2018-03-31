using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
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
                Id = 1,
                Name = "Bus",
                Description = "School bus",
                Cost = 50000,
                Image = "/cars/icons8-bus-64.png",
                ButtonContent = ButtonAction.Buy
            };
            var limousine = new Goods
            {
                Id = 2,
                Name = "Limousine",
                Description = "Elite class",
                Cost = 10000,
                Image = "/cars/icons8-limousine-filled-50.png",
                ButtonContent = ButtonAction.Buy
            };
            var pickup = new Goods
            {
                Id = 3,
                Name = "Pickup",
                Description = "Workhorse",
                Cost = 20000,
                Image = "/cars/icons8-pickup-48.png",
                ButtonContent = ButtonAction.Buy
            };
            var suv = new Goods
            {
                Id = 4,
                Name = "Suv",
                Description = "Sport class",
                Cost = 35000,
                Image = "/cars/icons8-suv-48.png",
                ButtonContent = ButtonAction.Buy
            };
            var tesla = new Goods
            {
                Id = 5,
                Name = "Tesla",
                Description = "Electric car",
                Cost = 50000,
                Image = "/cars/icons8-tesla-model-x-filled-50.png",
                ButtonContent = ButtonAction.Buy
            };
            GoodsList.Add(bus);
            GoodsList.Add(limousine);
            GoodsList.Add(pickup);
            GoodsList.Add(suv);
            GoodsList.Add(tesla);

            foreach (var goods in GoodsList)
            {
                goods.ActionEvent += () => ActionEvent(goods);
                goods.PropertyChanged += GoodsPropertyChanged;
            }
        }

        private void ActionEvent(Goods goods)
        {
            if (goods.ButtonContent == ButtonAction.Buy)
            {
                var bucketGoods = new Goods
                {
                    Id = goods.Id,
                    Name = goods.Name,
                    Cost = goods.Cost,
                    Description = goods.Description,
                    Image = goods.Image,
                    ButtonContent = ButtonAction.Remove
                };
                bucketGoods.ActionEvent += () => ActionEvent(bucketGoods);
                BucketList.Add(bucketGoods);
            }
            else if (goods.ButtonContent == ButtonAction.Remove)
            {
                // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
                goods.ActionEvent -= () => ActionEvent(goods);
                BucketList.Remove(goods);
            }
        }

        private async void BucketList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await RecalculateTotalCost();
        }

        private async Task RecalculateTotalCost()
        {
            var totalCost = 0.0;
            await Task.Run(() =>
            {
                foreach (var item in BucketList)
                {
                    totalCost += item.Cost;
                }
            });

            TotalCost = totalCost.ToString(CultureInfo.InvariantCulture);
        }

        private async void GoodsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is Goods goods))
                return;
            await Task.Run(() =>
            {
                if (e.PropertyName == "Cost")
                {
                    foreach (var b in BucketList)
                    {
                        if (b.Id == goods.Id)
                            b.Cost = goods.Cost;
                    }
                }
            });

            await RecalculateTotalCost();
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                var newBucket = new ObservableCollection<Goods>();
                for (var i = 0; i < 5000000; i++)
                {
                    foreach (var g in GoodsList)
                    {
                        var bucketGoods = new Goods
                        {
                            Id = g.Id,
                            Name = g.Name,
                            Cost = g.Cost,
                            Description = g.Description,
                            Image = g.Image,
                            ButtonContent = ButtonAction.Remove
                        };
                        bucketGoods.ActionEvent += () => ActionEvent(bucketGoods);
                        newBucket.Add(bucketGoods);
                    }
                }

                BucketList = newBucket;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Goods> GoodsList { get; set; } = new ObservableCollection<Goods>();

        private ObservableCollection<Goods> _bucketList = new ObservableCollection<Goods>();

        public ObservableCollection<Goods> BucketList
        {
            get => _bucketList;
            set
            {
                _bucketList = value;
                OnPropertyChanged();
            }
        }
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
