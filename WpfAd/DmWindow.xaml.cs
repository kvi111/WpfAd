using log4net;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
using WpfAd.dao;
using WpfAd.model;

namespace WpfAd
{
    /// <summary>
    /// DmWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DmWindow : Window
    {
        private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        public static readonly ILog log = log4net.LogManager.GetLogger(typeof(MainWindow));
        DispatcherTimer timer = new DispatcherTimer();

        public static List<Dm> listDm = new List<Dm>();
        public static int index = 0;
        public static Dm dm;
        public bool isClose = false;
        public bool isStop = false;

        public DmWindow()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 30);   //间隔30秒
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(ShowDM, new object[] { this.imgAd, this.lblDebug });

            ShowCategories();
        }

        private void ShowCategories()
        {
            List<Category> categories = CategoryDao.GetAllCategories();
            ColumnDefinition col1 = new ColumnDefinition();
            //ColumnDefinition col2 = new ColumnDefinition();
            grid1.ColumnDefinitions.Add(col1);
            //grid1.ColumnDefinitions.Add(col2);
            for (int i = 0; i < categories.Count; i++)
            {
                try
                {
                    grid1.RowDefinitions.Add(new RowDefinition());

                    Border border = new Border();
                    border.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/btnBkg.png")));

                    TextBlock tb = new TextBlock();
                    tb.Margin = new Thickness(-10, -10, -10, -10);
                    tb.Text = categories[i].name;
                    tb.Tag = categories[i].category_id;
                    tb.FontSize = tb.FontSize + 12;
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    //tb.TouchDown += Tb_TouchDown;
                    tb.MouseLeftButtonDown += Tb_MouseLeftButtonDown;

                    border.Child = tb;

                    grid1.Children.Add(border);
                    Grid.SetColumn(border, 0);
                    Grid.SetRow(border, i);

                    //grid1.RowDefinitions.Add(new RowDefinition());

                    //TextBlock tb = new TextBlock();
                    //tb.Text = categories[i * 2].name;
                    //tb.Tag = categories[i * 2].category_id;
                    //tb.FontSize = tb.FontSize + 25;
                    //tb.HorizontalAlignment = HorizontalAlignment.Center;
                    //tb.VerticalAlignment = VerticalAlignment.Center;
                    ////tb.TouchDown += Tb_TouchDown;
                    //tb.MouseLeftButtonDown += Tb_MouseLeftButtonDown;

                    //grid1.Children.Add(tb);
                    //Grid.SetColumn(tb, 0);
                    //Grid.SetRow(tb, i);

                    //TextBlock tb1 = new TextBlock();
                    //tb1.Text = categories[i * 2 + 1].name;
                    //tb1.Tag = categories[i * 2 + 1].category_id;
                    //tb1.FontSize = tb1.FontSize + 25;
                    //tb1.HorizontalAlignment = HorizontalAlignment.Center;
                    //tb1.VerticalAlignment = VerticalAlignment.Center;
                    //tb1.MouseLeftButtonDown += Tb_MouseLeftButtonDown;

                    //grid1.Children.Add(tb1);
                    //Grid.SetColumn(tb1, 1);
                    //Grid.SetRow(tb1, i);
                }
                catch (Exception ex)
                {
                    log.Error("ShowCategories error:", ex);
                }
            }
            //grid1.ShowGridLines = true;
        }

        private void Tb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TextBlock tb = (TextBlock)sender;
                int catId = int.Parse(tb.Tag.ToString());
                listDm = DmDao.GetDmsBycatId(catId);
                Stop30Second();
            }
            catch (Exception ex)
            {
                log.Error("Tb_MouseLeftButtonDown error:", ex);
            }
        }

        private async void ShowDM(object obj)
        {
            object[] objarr = (object[])obj;
            Image img = (Image)objarr[0];
            Label lbl = (Label)objarr[1];
            listDm = DmDao.GetDms();
            while (isClose == false)
            {
                if (isStop == false)
                {
                    var dm = await GetDmByIndex(lbl);
                    if (dm != null)
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                img.Source = new BitmapImage(new Uri(dm.img_path, UriKind.Absolute));
                            }
                            catch (Exception ex)
                            {
                                log.Error("ShowDM img.Source error:", ex);
                            }
                        }, new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler);
                        await Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(dm.display_time * 1000);
                        });
                    }
                    index++;
                }
                await Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                });
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
            Dm dm = await GetDmByIndex(this.lblDebug);
            if (dm != null && File.Exists(dm.img_path))
            {
                imgAd.Source = new BitmapImage(new Uri(dm.img_path, UriKind.Absolute));
            }
            Stop30Second();
        }

        private void ImgReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.isClose = true;
            timer.Stop();
            MainWindow window = new MainWindow();
            window.Show();

            this.Close();
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
            else //从右到左 ←
            {
                Stop30Second();
                ImgRight_MouseLeftButtonDown(sender, null);
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
            ImgReturn_MouseLeftButtonDown(sender, null);
        }
    }
}
