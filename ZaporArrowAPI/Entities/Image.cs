using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZaporArrowAPI.Entities
{
    public class Image
    {
        [Key]
        public Guid ImageId { get; set; }

        [ForeignKey("ArrowId")]
        public Guid ArrowId { get; set; }

        [Required]
        public string ImageSource { get; set; }
    }
}
