using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class PostModel
    {
        [Key]
        public int Id { get; set; }
        public string Caption { get; set; }
        public List<TagModel> Tags { get; set; } = new List<TagModel>();
        public string ImgURL { get; set; }
        public string Description { get; set; }
    }
}
