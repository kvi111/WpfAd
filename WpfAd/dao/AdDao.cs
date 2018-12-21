using log4net;
using System;
using System.Collections.Generic;
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
                using (var db = new SQLiteDb())
                {
                    var ads = db.Ads.Where(x => x.puton_time <= DateTime.Now && DateTime.Now <= x.putoff_time).OrderBy(x => x.advertisement_id).ToList();
                    return ads;
                }
            }
            catch (Exception ex)
            {
                log.Error("GetAds error:", ex);
                return new List<Ad>();
            }
        }
    }
}
