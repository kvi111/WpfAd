using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAd.common;

namespace WpfAd.model
{
    public class Ad
    {
        [Key]
        public int id { get; set; }

        [Required]
        public Int64 advertisement_id { get; set; }
        public string advertiser_name { get; set; }

        [Required]
        public int display_time { get; set; }

        [Required]
        public DateTime puton_time { get; set; }

        [Required]
        public DateTime putoff_time { get; set; }
        public string Title { get; set; }

        [Required]
        public string image_url { get; set; }

        public string sub_image_url { get; set; }

        [Required]
        public DateTime date_modified { get; set; }

        /// <summary>
        /// 1. 一级广告图片
        /// 2. 一二级广告图片
        /// 3. 一二级广告图片和信息录入
        /// 4. 进入dm
        /// </summary>
        [Required]
        public int show_type { get; set; }

        [Required]
        public string img_path { get; set; }
        public string sub_img_path { get; set; }
        public static string GetImgName(string imgUrl)
        {
            string[] strArr = imgUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return strArr.Length > 0 ? strArr[strArr.Length - 1] : "";
        }
    }
}
