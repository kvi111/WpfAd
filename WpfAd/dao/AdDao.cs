using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAd.model;

namespace WpfAd.dao
{
    public class AdDao
    {
        public static readonly ILog log = log4net.LogManager.GetLogger(typeof(AdDao));

        /// <summary>
        /// 插入一条ad
        /// </summary>
        /// <param name="ad"></param>
        /// <returns></returns>
        public static int InsertAd(Ad ad)
        {
            using (var db = new SQLiteDb())
            {
                db.Ads.Add(ad);
                int count = db.SaveChanges();
                return count;
            }
        }

        /// <summary>
        /// 插入多条ad
        /// </summary>
        /// <param name="listAd"></param>
        /// <returns></returns>

        public static int InsertAds(List<Ad> listAd)
        {
            using (var db = new SQLiteDb())
            {
                db.Ads.AddRange(listAd);
                int count = db.SaveChanges();
                return count;
            }
        }

        /// <summary>
        /// 更新ad
        /// </summary>
        /// <param name="ad"></param>
        /// <returns></returns>
        public static int UpdateAd(Ad ad)
        {
            using (var db = new SQLiteDb())
            {
                db.Ads.Add(ad);
                db.Entry<Ad>(ad).State = System.Data.Entity.EntityState.Modified;
                int count = db.SaveChanges();
                return count;
            }
        }

        /// <summary>
        /// 通过advertisement_id查找ad
        /// </summary>
        /// <param name="adId"></param>
        /// <returns></returns>
        public static Ad GetAdById(long adId)
        {
            using (var db = new SQLiteDb())
            {
                return db.Ads.FirstOrDefault(x => x.advertisement_id == adId);
            }
        }

        /// <summary>
        /// 查找有效时间范围内的ad
        /// </summary>
        /// <returns></returns>
        public static List<Ad> GetAds()
        {
            try
            {
                DeleteAdByOutDate(); //先删除过期广告
                using (var db = new SQLiteDb())
                {
                    var ads = db.Ads.ToList().Where(x => x.puton_time <= DateTime.Now && DateTime.Now <= x.putoff_time).OrderBy(x => x.advertisement_id).ToList();
                    return ads;
                }
            }
            catch (Exception ex)
            {
                log.Error("GetAds error:", ex);
                return new List<Ad>();
            }
        }

        /// <summary>
        /// 查找过期的ad
        /// </summary>
        /// <returns></returns>
        public static List<Ad> GetAdsByOutDate()
        {
            try
            {
                using (var db = new SQLiteDb())
                {
                    var ads = db.Ads.ToList().Where(x => x.puton_time > DateTime.Now || DateTime.Now > x.putoff_time).ToList();
                    return ads;
                }
            }
            catch (Exception ex)
            {
                log.Error("GetAdsByOutDate error:", ex);
                return new List<Ad>();
            }
        }

        /// <summary>
        /// 删除过期的ad
        /// </summary>
        /// <returns></returns>
        public static void DeleteAdByOutDate()
        {
            using (var db = new SQLiteDb())
            {
                List<Ad> list = GetAdsByOutDate();
                foreach (Ad ad in list) {
                    db.Ads.Attach(ad);
                    db.Ads.Remove(ad);
                }
                //db.Ads.RemoveRange(list);
                db.SaveChanges();
            }
        }
    }
}
