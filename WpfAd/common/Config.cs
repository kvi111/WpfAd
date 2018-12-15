using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAd.common
{
    public class Config
    {
        public static string siteRoot = ConfigurationManager.AppSettings["siteRoot"];
        public static string adUrl = siteRoot + ConfigurationManager.AppSettings["adUrl"];
        public static string logAdDataUrl = siteRoot + ConfigurationManager.AppSettings["logAdDataUrl"];
        public static string submitUrl = siteRoot + ConfigurationManager.AppSettings["submitUrl"];
        public static string adImgRoot = ConfigurationManager.AppSettings["adImgRoot"];
    }
}
