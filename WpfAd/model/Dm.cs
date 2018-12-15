using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAd.model
{
    [Table("dms")]
    public class Dm
    {
        [Key]
        public int id { get; set; }

        [Required]
        public Int64 advertisement_id { get; set; }
        public string store_name { get; set; }

        [Required]
        public int display_time { get; set; }

        [Required]
        public DateTime puton_time { get; set; }

        [Required]
        public DateTime putoff_time { get; set; }
        public string Title { get; set; }

        [Required]
        public string image_url { get; set; }

        [Required]
        public int category_id { get; set; }

        [Required]
        public DateTime date_modified { get; set; }
        public string img_path { get; set; }
    }
}