using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZaporArrowAPI.ViewModels
{
    public class ArrowViewModel
    {
        [MaxLength(200)]
        public string Description { get; set; }
        public double Length { get; set; }
        public IFormFile PhotoFile { get; set; }
    }
}
