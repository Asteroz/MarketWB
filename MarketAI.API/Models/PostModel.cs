using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Models
{
    public class PostModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Tags { get; set; }
        [NotMapped]
        public List<string> TagsList => Tags.Split(',').ToList();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string ImgURL { get; set; }
        public string Description { get; set; }

        public bool PublishAfter { get; set; }
        public DateTime PublishAfterDate { get; set; }
    }
}
