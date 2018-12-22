using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using WpfAd.common;
using WpfAd.model;
using WpfAd.dao;

namespace WpfAd.service
{
    public class All
    {
        public static readonly ILog log = log4net.LogManager.GetLogger(typeof(All));

        /// <summary>
        /// 从url获取ad信息
        /// </summary>
        public static void GetAdinfoByUrl()
        {
            try
            {
                List<Ad> listAD = new List<Ad>();
                List<Dm> listDm = new List<Dm>();
                List<Category> listCategory = new List<Category>();
                string jsonText = HttpUtil.HttpGet(Config.adUrl);
                if (String.IsNullOrEmpty(jsonText)) return;

                JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonText);
                if (jObject != null && jObject.ContainsKey("msg") && jObject["msg"].ToString() == "OK" && jObject.ContainsKey("data"))
                {
                    JObject jObjectData = (JObject)jObject["data"];
                    DoAdjson(listAD, jObjectData);
                    DoDmjson(listDm, jObjectData);
                    DoCategoryjson(listCategory, jObjectData);
                }
            }
            catch (Exception ex)
            {
                log.Error("GetAdinfoByUrl error:", ex);
            }
        }

        private static void DoAdjson(List<Ad> listAD, JObject jObjectData)
        {
            if (jObjectData.ContainsKey("ad")) //处理ad
            {
                JArray jObjectAd = (JArray)jObjectData["ad"];
                foreach (JToken jToken in jObjectAd.Children())
                {
                    Ad ad = new Ad();
                    ad.advertisement_id = long.Parse(jToken["advertisement_id"].ToString());
                    ad.advertiser_name = jToken["advertiser_name"].ToString();
                    ad.date_modified = DateTime.Parse(jToken["date_modified"].ToString());
                    ad.display_time = int.Parse(jToken["display_time"].ToString());
                    ad.image_url = jToken["image_url"].ToString();
                    ad.sub_image_url = jToken["sub_image_url"].ToString();
                    ad.Title = jToken["title"].ToString();
                    ad.puton_time = DateTime.Parse(jToken["puton_time"].ToString());
                    ad.putoff_time = DateTime.Parse(jToken["putoff_time"].ToString());
                    ad.show_type = int.Parse(jToken["show_type"].ToString());
                    ad.img_path = Config.adImgRoot + "\\" + Ad.GetImgName(ad.image_url);

                    Ad oldad = AdDao.GetAdById(ad.advertisement_id);
                    if (oldad == null) //数据库中不存在
                    {
                        bool url1 = true, url2 = true;
                        url1 = HttpUtil.DownloadImg(ad.image_url, ad.img_path);
                        if ((ad.show_type == 2 || ad.show_type == 3) && String.IsNullOrEmpty(ad.sub_image_url) == false)
                        {
                            ad.sub_img_path = Config.adImgRoot + "\\" + Ad.GetImgName(ad.sub_image_url);
                            url2 = HttpUtil.DownloadImg(ad.sub_image_url, ad.sub_img_path);
                        }
                        else
                        {
                            ad.sub_img_path = "";
                        }
                        if (url1 && url2)
                        {
                            AdDao.InsertAd(ad);
                            listAD.Add(ad);
                        }
                    }
                    else //数据库中存在
                    {
                        if (oldad.date_modified != ad.date_modified)//需要更新
                        {
                            FileInfo fileInfo = new FileInfo(ad.img_path);
                            //if (fileInfo.Directory.Exists == false) {
                            //    fileInfo.Directory.Create();
                            //}
                            bool url1 = true, url2 = true;
                            if (fileInfo.Exists == false || fileInfo.Length <= 0)
                            {
                                url1 = HttpUtil.DownloadImg(ad.image_url, ad.img_path);
                            }
                            if ((ad.show_type == 2 || ad.show_type == 3) && String.IsNullOrEmpty(ad.sub_image_url) == false)
                            {
                                ad.sub_img_path = Config.adImgRoot + "\\" + Ad.GetImgName(ad.sub_image_url);
                                url2 = HttpUtil.DownloadImg(ad.sub_image_url, ad.sub_img_path);
                            }
                            else
                            {
                                ad.sub_img_path = "";
                            }
                            if (url1 && url2)
                            {
                                ad.id = oldad.id;
                                ad.sub_image_url = oldad.sub_image_url;
                                AdDao.UpdateAd(ad);
                                listAD.Add(ad);
                            }
                        }
                    }
                }
                MainWindow.listAd = AdDao.GetAds();
            }
        }

        private static void DoDmjson(List<Dm> listDm, JObject jObjectData)
        {
            if (jObjectData.ContainsKey("dm")) //处理dm
            {
                JArray jObjectDm = (JArray)jObjectData["dm"];
                foreach (JToken jToken in jObjectDm.Children())
                {
                    Dm dm = new Dm();
                    dm.advertisement_id = long.Parse(jToken["advertisement_id"].ToString());
                    dm.store_name = jToken["store_name"].ToString();
                    dm.display_time = int.Parse(jToken["display_time"].ToString());
                    dm.puton_time = DateTime.Parse(jToken["puton_time"].ToString());
                    dm.putoff_time = DateTime.Parse(jToken["putoff_time"].ToString());
                    dm.Title = jToken["title"].ToString();
                    dm.image_url = jToken["image_url"].ToString();
                    dm.category_id = int.Parse(jToken["category_id"].ToString());
                    dm.date_modified = DateTime.Parse(jToken["date_modified"].ToString());
                    dm.img_path = Config.adImgRoot + "\\" + Ad.GetImgName(dm.image_url);

                    Dm oldDm = DmDao.GetDmById(dm.advertisement_id);
                    if (oldDm == null)//数据库中不存在
                    {
                        if (HttpUtil.DownloadImg(dm.image_url, dm.img_path))
                        {
                            DmDao.InsertDm(dm);
                            listDm.Add(dm);
                        }
                    }
                    else //数据库中存在
                    {
                        if (oldDm.date_modified != dm.date_modified)//如果需要修改
                        {
                            FileInfo fileInfo = new FileInfo(dm.img_path);
                            if (fileInfo.Exists == false || fileInfo.Length <= 0)
                            {
                                if (HttpUtil.DownloadImg(dm.image_url, dm.img_path))
                                {
                                    dm.id = oldDm.id;
                                    DmDao.UpdateDm(dm);
                                    listDm.Add(dm);
                                }
                            }
                        }
                    }
                }
                DmWindow.listDm = DmDao.GetDms();
            }
        }

        private static void DoCategoryjson(List<Category> listCategory, JObject jObjectData)
        {
            if (jObjectData.ContainsKey("category")) //处理dm
            {
                JArray jObjectCate = (JArray)jObjectData["category"];
                foreach (JToken jToken in jObjectCate.Children())
                {
                    Category category = new Category();
                    category.category_id = int.Parse(jToken["category_id"].ToString());
                    category.name = jToken["name"].ToString();

                    Category oldCategory = CategoryDao.GetCategoryBycatId(category.category_id);
                    if (oldCategory == null)
                    {
                        CategoryDao.InsertCategory(category);
                        listCategory.Add(category);
                    }
                }
            }
        }

        /// <summary>
        /// 检查图片目录是否存在，不存在则创建
        /// </summary>
        public static void CheckImgDir()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Config.adImgRoot);
            if (dirInfo.Exists == false)
            {
                dirInfo.Create();
            }
        }

        /// <summary>
        /// 广告数据记录接口 /screen/record?advertisement_id=1&action_type=1&action_time=2018-12-08%2018:01:01
        /// action_type 记录类型   1: 广告刷新   2: 用户点击
        /// action_time 发生时间，格式yyyy-MM-dd HH:mm:ss(可为空)
        /// </summary>
        /// <param name="advertisement_id">广告id</param>
        /// <param name="action_type">记录类型  1: 广告刷新  2: 用户点击</param>
        public async static void LogAdData(long advertisement_id, int action_type)
        {
            string url = String.Format(Config.logAdDataUrl + "?advertisement_id={0}&action_type={1}&action_time={2}", advertisement_id, action_type, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await HttpUtil.DownloadString(url);
        }

        /// <summary>
        /// 提交用户信息 /screen/add-customer-info?advertisement_id={0}&name={1}&telephone={2}&area_code={3}
        /// </summary>
        /// <param name="advertisement_id"></param>
        /// <param name="action_type"></param>
        public async static void SubmitData(long advertisement_id, string name, string tel, string area_code)
        {
            string url = String.Format(Config.submitUrl + "?advertisement_id={0}&name={1}&telephone={2}&area_code={3}", advertisement_id, name, tel, area_code);
            await HttpUtil.DownloadString(url);
        }
    }
}
