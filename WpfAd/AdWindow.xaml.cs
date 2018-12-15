using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAd.dao;
using WpfAd.model;

namespace WpfAd
{
    /// <summary>
    /// DmWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdWindow : Window
    {
        private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        public static List<Dm> listDm = new List<Dm>();
        public static int index = 0;
        public static Dm dm;

        public AdWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(ShowAd, new object[] { this.imgAd, this.lblDebug });
        }

        private async void ShowAd(object obj)
        {
            object[] objarr = (object[])obj;
            Image img = (Image)objarr[0];
            Label lbl = (Label)objarr[1];
            //await Task.Factory.StartNew(Begin, this.imgAd);
            listDm = DmDao.GetDms();
            while (true)
            {
                
                    var dm = await GetDmByIndex(lbl);
                    if (dm != null)
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            img.Source = new BitmapImage(new Uri(dm.img_path, UriKind.Absolute));
                        }, new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler);
                        await Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(dm.display_time * 1000);
                        });
                    }
                
                await Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                });
                index++;
            }
        }

        private async Task<Dm> GetDmByIndex(Label lbl)
        {
            if (listDm.Count <= 0) return null;

            if (index < 0)
            {
                index = listDm.Count - 1;
            }
            if (index > (listDm.Count - 1))
            {
                index = 0;
            }
            await Task.Factory.StartNew(() =>
            {
                lbl.Content = index;
            }, new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler);
            dm = listDm[index];
            return listDm[index];
        }

        private void LblDebug_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ImgLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ImgRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ImgReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
