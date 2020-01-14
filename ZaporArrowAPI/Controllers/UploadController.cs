using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZaporArrowAPI.Entities;
using ZaporArrowAPI.Services;

namespace ZaporArrowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        // private readonly IZaporArrowRepository _zaporArrowRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadController(IWebHostEnvironment env)
        {
            //_zaporArrowRepository = repository;
            _webHostEnvironment = env;
        }

        [HttpPost]
        public async Task<string> Post([FromForm]UploadImage imageFile)
        {
            try
            {
                if (imageFile.file.Length > 0)
                {
                    if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\Images\\"))
                    {
                        Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\Images\\");
                    }
                    using (FileStream fileStream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\Images\\" + imageFile.file.FileName))
                    {
                        imageFile.file.CopyTo(fileStream);
                        fileStream.Flush();
                        return "\\Upload\\" + imageFile.file.FileName;
                    }
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }
    }
}