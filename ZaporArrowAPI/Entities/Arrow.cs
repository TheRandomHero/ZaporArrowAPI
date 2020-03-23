using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ZaporArrowAPI.Entities
{
    public class Arrow
    {
        [Key]
        public Guid ArrowId { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public ICollection<Image> Images { get; set; }
    }
}
