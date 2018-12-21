using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAd.model;
using WpfAd.dao;
using log4net;
using System.IO;
using WpfAd.service;
using System.Windows.Threading;

namespace WpfAd
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        public static readonly ILog log = log4net.LogManager.GetLogger(typeof(MainWindow));

        DispatcherTimer timer = new DispatcherTimer();

        public static MainWindow mainWindow;
        public static List<Ad> listAd = new List<Ad>();

        public static int index = 0;
        public static Ad ad;
        public bool isClose = false;
        public bool isStop = false;

        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 30);   //间隔30秒
            timer.Tick += new EventHandler(Timer_Tick);

            mainWindow = this;
            var ls = DmDao.GetDms();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(ShowAd, new object[] { this.imgAd, this.lblDebug });

            #region tp
            //ThreadPool.QueueUserWorkItem((o) =>
            //{
            //    for (long i = 1; i < 1000000; i++)
            //    {
            //        t1.Dispatcher.Invoke(new Action(() =>
            //        {
            //            t1.Text = i.ToString();
            //        }));
            //    }
            //});


            //WebClient wc = new WebClient();
            //using (var ms = new MemoryStream(wc.DownloadData("https://avatar.csdn.net/7/C/5/3_luozirong.jpg")))
            //{
            //    BitmapImage image = new BitmapImage();
            //    image.BeginInit();
            //    image.CacheOption = BitmapCacheOption.OnLoad;
            //    image.StreamSource = ms;
            //    image.EndInit();

            //    imgAd.Source = image;
            //}
            #endregion
        }

        private async void ShowAd(object obj)
        {
            object[] objarr = (object[])obj;
            Image img = (Image)objarr[0];
            Label lbl = (Label)objarr[1];
            //await Task.Factory.StartNew(Begin, this.imgAd);
            listAd = AdDao.GetAds();
            while (isClose == false)
            {
                try
                {
                    if (isStop == false)
                    {
                        var ad = await GetAdByIndex(lbl);
                        if (ad != null)
                        {
                            await Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    img.Source = new BitmapImage(new Uri(ad.img_path, UriKind.Absolute));
                                }
                                catch (Exception ex)
                                {
                                    log.Error("ShowAd img.Source error:", ex);
                                }
                            }, new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler);
                            await Task.Factory.StartNew(() =>
                            {
                                Thread.Sleep(ad.display_time * 1000);
                            });
                        }
                        index++;
                    }
                    await Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(1000);
                    });

                }
                catch (Exception ex)
                {
                    log.Error("ShowAd while error:", ex);
                }
            }
        }

        private async Task<Ad> GetAdByIndex(Label lbl)
        {
            if (listAd.Count <= 0) return null;

            if (index < 0)
            {
                index = listAd.Count - 1;
            }
            if (index > (listAd.Count - 1))
            {
                index = 0;
            }
            await Task.Factory.StartNew(() =>
            {
                lbl.Content = index;
            }, new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler);
            ad = listAd[index];
            return listAd[index];
        }

        /// <summary>
        /// 促销信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgSaleinfo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.isClose = true;

            DmWindow dmWindow = new DmWindow();
            dmWindow.Show();

            this.Close();
        }

        /// <summary>
        /// 广告详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgAdinfo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ad != null)
            {
                Task.Run(() =>
                {
                    All.LogAdData(ad.advertisement_id, 2); //记录用户点击
                });

                if (ad.show_type == 2 || ad.show_type == 3)
                {
                    this.isClose = true;

                    AdInfoWindow adInfo = new AdInfoWindow();
                    adInfo.SetImg(ad, index);
                    adInfo.Show();

                    this.Close();
                }
                if (ad.show_type == 4)
                {
                    this.isClose = true;

                    DmWindow dmWindow = new DmWindow();
                    dmWindow.Show();

                    this.Close();
                }
            }
        }

        private async void ImgLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            index--;
            await NextImg();
        }

        private async void ImgRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            index++;
            await NextImg();
        }
        private async Task NextImg()
        {
            try
            {
                Ad ad = await GetAdByIndex(this.lblDebug);
                if (ad != null && File.Exists(ad.img_path))
                {
                    imgAd.Source = new BitmapImage(new Uri(ad.img_path, UriKind.Absolute));
                }
            }
            catch (Exception ex) {
                log.Error("NextImg error:", ex);
            }
            Stop30Second();
            
        }

        private void ImgAd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImgAdinfo_MouseLeftButtonDown(sender, e);
        }

        private void LblDebug_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.Shutdown();
        }

        Point start;
        private void ImgAd_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            start = e.ManipulationOrigin;
        }

        private void ImgAd_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Point end = e.ManipulationOrigin;
            if (end.X > start.X) //从左到右 →
            {
                Stop30Second();

                ImgLeft_MouseLeftButtonDown(sender, null);
            }
            else if (end.X < start.X) //从右到左 ←
            {
                Stop30Second();

                ImgRight_MouseLeftButtonDown(sender, null);
            }
            else  //点击事件
            {
                ImgAd_MouseLeftButtonDown(sender, null);
            }
        }
        private void Stop30Second()
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
            isStop = true;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            isStop = false;
        }
    }
}
