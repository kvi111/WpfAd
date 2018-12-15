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
using WpfAd.model;
using WpfAd.service;

namespace WpfAd
{
    /// <summary>
    /// AdInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AdInfoWindow : Window
    {
        public static readonly ILog log = log4net.LogManager.GetLogger(typeof(MainWindow));
        public Ad ad;
        public int index = 0;
        DispatcherTimer timer = new DispatcherTimer();

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            ImgReturn_MouseLeftButtonDown(imgReturn, null);
        }

        public AdInfoWindow()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 90);   //间隔90秒
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();

            border1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/btnBkg.png")));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //btn1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/btnBkg.png")));
        }

        private void ImgSaleinfo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DmWindow window = new DmWindow();
            window.Show();

            timer.Stop();
            this.Close();
        }

        private void ImgReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            MainWindow.index = this.index;
            mainWindow.Show();

            timer.Stop();
            this.Close();
        }

        /// <summary>
        /// 设置图片和一级广告的图片index
        /// </summary>
        /// <param name="ad"></param>
        /// <param name="index"></param>
        public void SetImg(Ad ad, int index)
        {
            this.ad = ad;
            this.index = index;
            if (ad != null)
            {
                if (ad.show_type != 3)//不提交信息
                {
                    grid1.Visibility = Visibility.Hidden;
                    grid2.Visibility = Visibility.Hidden;
                }
                try
                {
                    imgAd.Source = new BitmapImage(new Uri(ad.sub_img_path, UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    log.Error("SetImg error:", ex);
                }
            }
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //todo:验证、提交
            if (String.IsNullOrEmpty(userName.Text))
            {
                userName.Background = new SolidColorBrush(Colors.SpringGreen);
                userName.Focus();
                return;
            }
            if (String.IsNullOrEmpty(userTel.Text))
            {
                userTel.Background = new SolidColorBrush(Colors.SpringGreen);
                userTel.Focus();
                return;
            }
            ComboBoxItem comboBoxItem = (ComboBoxItem)userAreaCode.SelectedItem;
            string name = userName.Text;
            string tel = userTel.Text;
            string area_Code = comboBoxItem.Tag.ToString();

            Task.Run(() =>
            {
                All.SubmitData(ad.advertisement_id, name, tel, area_Code); //记录用户点击
            });
            ImgReturn_MouseLeftButtonDown(sender, e);
        }
    }
}
