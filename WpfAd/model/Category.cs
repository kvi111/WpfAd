using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAd.model
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public long id { get; set; }

        [Required]
        public int category_id { get; set; }

        [Required]
        public string name { get; set; }
    }
}
