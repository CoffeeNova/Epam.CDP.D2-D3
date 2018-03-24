using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace V1.TaskAsync._02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        public ObservableCollection<UrlListViewItem> ItemList { get; set; } = new ObservableCollection<UrlListViewItem>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            FillItemListWithTestData();
        }

        private void FillItemListWithTestData()
        {
            ItemList.Add(new UrlListViewItem
            {
                Url = "http://ipv4.download.thinkbroadband.com:8080/200MB.zip"
            });
        }

        private void UrlTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ItemList.Add(new UrlListViewItem
                {
                    Url = ((TextBox)sender).Text
                });
                ((TextBox) sender).Clear();
            }
        }

        private async Task<string> DownloadPageAsync(UrlListViewItem item, CancellationToken cancellationToken)
        {
            using (var client = new WebClient())
            using (cancellationToken.Register(CancellationRegisterAction(client, item)))
            {
                try
                {
                    client.DownloadProgressChanged += (sender, e) => DownloadProgressChanged(e, item);
                    client.DownloadStringCompleted += async (sender, e) => await Client_DownloadStringCompleted(item);
                    item.DownloadStatus = DownloadStatus.Downloading;
                    return await client.DownloadStringTaskAsync(item.Url);
                }
                catch (Exception ex)
                {
                    if (ex is WebException == false)
                        item.DownloadStatus = DownloadStatus.Failed;
                    return null;
                }
            }
        }

        private Action CancellationRegisterAction(WebClient client, UrlListViewItem item)
        {
            return () =>
            {
                if (item.DownloadStatus == DownloadStatus.Downloading)
                {
                    item.DownloadStatus = DownloadStatus.Canceled;
                    client.CancelAsync();
                }
            };
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ItemList.Any())
                return;

            foreach (var item in ItemList)
            {
                if (item.DownloadStatus != DownloadStatus.Downloading &&
                    item.DownloadStatus != DownloadStatus.Downloaded)
                {
                    item.CancellationTokenSource = new CancellationTokenSource();
                    item.Content = DownloadPageAsync(item, item.CancellationTokenSource.Token);
                }
            }
        }

        private void DownloadProgressChanged(DownloadProgressChangedEventArgs e, UrlListViewItem item)
        {
            item.Progress = e.ProgressPercentage;
        }

        private async Task Client_DownloadStringCompleted(UrlListViewItem item)
        {
            item.Progress = 100;
            var content = await item.Content;
            if (content != null)
                item.DownloadStatus = DownloadStatus.Downloaded;
        }
    }
}
