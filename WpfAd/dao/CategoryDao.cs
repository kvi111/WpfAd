using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAd.model;

namespace WpfAd.dao
{
    public class CategoryDao
    {
        public static readonly ILog log = log4net.LogManager.GetLogger(typeof(CategoryDao));

        /// <summary>
        /// 插入一条Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static int InsertCategory(Category category)
        {
            using (SQLiteDb db = new SQLiteDb())
            {
                //db.Entry(category);
                db.Categories.Add(category);
                //db.Set<Category>().Add(category);
                int count = db.SaveChanges();
                return count;
            }
        }

        /// <summary>
        /// 通过category_id得到Category
        /// </summary>
        /// <param name="catId"></param>
        /// <returns></returns>
        public static Category GetCategoryBycatId(long catId)
        {
            using (var db = new SQLiteDb())
            {
                return db.Categories.FirstOrDefault(x => x.category_id == catId);
            }
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        public static List<Category> GetAllCategories()
        {
            using (var db = new SQLiteDb())
            {
                return db.Categories.ToList();
            }
        }
    }
}
