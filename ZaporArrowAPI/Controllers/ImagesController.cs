using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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
    public class ImagesController : ControllerBase
    {
        private readonly IZaporArrowRepository _zaporArrowRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImagesController(IWebHostEnvironment env, IZaporArrowRepository repository)
        {
            _zaporArrowRepository = repository;
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

        [HttpGet]
        public async Task<IActionResult> GetImage([FromQuery]string imageName)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var path = "\\Images\\" + imageName;
            path = _webHostEnvironment.WebRootPath + path;

            var ext = System.IO.Path.GetExtension(path);

            var image = System.IO.File.OpenRead(path);
            return File(image, "image/jpeg");

        }
    }
}