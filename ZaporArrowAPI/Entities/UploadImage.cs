﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZaporArrowAPI.Entities
{
    public class UploadImage
    {
        public IFormFile file { get; set; }
    }
}
