using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAd.model;

namespace WpfAd.dao
{
    public class DmDao
    {
        public static readonly ILog log = log4net.LogManager.GetLogger(typeof(DmDao));

        /// <summary>
        /// 插入一条dm
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public static int InsertDm(Dm dm)
        {
            using (var db = new SQLiteDb())
            {
                db.Dms.Add(dm);
                int count = db.SaveChanges();
                return count;
            }
        }

        /// <summary>
        /// 插入多条dm
        /// </summary>
        /// <param name="listDm"></param>
        /// <returns></returns>

        public static int InsertDms(List<Dm> listDm)
        {
            using (var db = new SQLiteDb())
            {
                db.Dms.AddRange(listDm);
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新dm
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public static int UpdateDm(Dm dm)
        {
            using (var db = new SQLiteDb())
            {
                db.Dms.Add(dm);
                db.Entry<Dm>(dm).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 通过id查找dm
        /// </summary>
        /// <param name="dmId"></param>
        /// <returns></returns>
        public static Dm GetDmById(long dmId)
        {
            using (var db = new SQLiteDb())
            {
                return db.Dms.FirstOrDefault(x => x.advertisement_id == dmId);
            }
        }

        /// <summary>
        /// 查找有效时间范围内的dm
        /// </summary>
        /// <returns></returns>
        public static List<Dm> GetDms()
        {
            try
            {
                using (var db = new SQLiteDb())
                {
                    return db.Dms.Where(x => x.puton_time <= DateTime.Now && DateTime.Now <= x.putoff_time).OrderByDescending(x => x.advertisement_id).ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error("GetDms error:", ex);
                return new List<Dm>();
            }
        }

        /// <summary>
        /// 根据category_id查找有效时间范围内的dm
        /// </summary>
        /// <param name="catId"></param>
        /// <returns></returns>
        public static List<Dm> GetDmsBycatId(int catId)
        {
            try
            {
                using (var db = new SQLiteDb())
                {
                    return db.Dms.Where(x => x.puton_time <= DateTime.Now && DateTime.Now <= x.putoff_time && x.category_id==catId).OrderByDescending(x => x.advertisement_id).ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error("GetDmsBycatId error:", ex);
                return new List<Dm>();
            }
        }
    }
}
