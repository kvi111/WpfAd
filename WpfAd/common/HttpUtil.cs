using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using WpfAd.dao;

namespace WpfAd.common
{
    public class HttpUtil
    {
        public static readonly ILog log = log4net.LogManager.GetLogger(typeof(HttpUtil));

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            string responseText = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);
                // 创建一个HTTP请求
                //request.Method="get";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader myreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                responseText = myreader.ReadToEnd();
                myreader.Close();
            }
            catch (Exception ex)
            {
                log.Error("HttpGet error:url=" + url, ex);
            }

            return responseText;
        }

        /// <summary>
        /// 从 url 下载图片到本地
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool DownloadImg(string url, string filePath)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    //webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);//不使用缓存
                    webClient.DownloadFile(url, filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("DownloadImg error:url=" + url + " filePath=" + filePath, ex);
                return false;
            }
        }
        public async static Task<string> DownloadString(string url)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Reload);//不使用缓存
                    return await webClient.DownloadStringTaskAsync(new Uri(url));
                }
            }
            catch (Exception ex)
            {
                log.Error("DownloadString error:url=" + url, ex);
                return "";
            }
        }
    }
}
