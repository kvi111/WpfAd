using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfAd.service;

namespace WpfAd
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static readonly ILog log = log4net.LogManager.GetLogger(typeof(App));
        Timer timer = new Timer(new TimerCallback(time_event), null, 0, 3 * 60 * 1000);

        private static void time_event(object state)
        {
            try
            {
                All.GetAdinfoByUrl();
            }
            catch (Exception ex)
            {
                log.Error("time_event Error:", ex);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            All.CheckImgDir();
        }
    }
}
