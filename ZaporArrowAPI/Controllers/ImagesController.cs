using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZaporArrowAPI.Entities;
using ZaporArrowAPI.Services;
using ZaporArrowAPI.ViewModels;

namespace ZaporArrowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : Controller
    {
        private readonly IZaporArrowRepository _zaporArrowRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImagesController(IWebHostEnvironment env, IZaporArrowRepository repository)
        {
            _zaporArrowRepository = repository;
            _webHostEnvironment = env;
        }

        [HttpPost]
        public async Task<string> Post([FromForm]ArrowViewModel model)
        {
            try
            {
                if (model.PhotoFile.Length > 0)
                {
                    if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\images\\"))
                    {
                        Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\images\\");
                    }
                    string uniqueFileName = Guid.NewGuid().ToString()+ "_" + model.PhotoFile.FileName;
                    using FileStream fileStream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\images\\" + uniqueFileName );
                    model.PhotoFile.CopyTo(fileStream);
                    fileStream.Flush();

                    Arrow newArrow = new Arrow
                    {
                        ArrowId = Guid.NewGuid(),
                        Length = model.Length,
                        Description = model.Description,
                        Images = new List<Image>(),
                    };

                    return "\\Upload\\" + model.PhotoFile.FileName;
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

        [HttpGet("{arrowId:guid}")]
        public async Task<IActionResult> GetArrowImage([FromRoute]Guid arrowId)
        {
            try
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var path = _zaporArrowRepository.GetImage(arrowId).ImageSource;
                path = _webHostEnvironment.WebRootPath + path;

                var ext = System.IO.Path.GetExtension(path);

                var image = System.IO.File.OpenRead(path);

                return File(image, "image/jpeg");

            }
            catch (Exception)
            {

                return StatusCode(400);
            };

        }

        [HttpGet("arrow/{arrowId:guid}")]
        public JsonResult GetArrowDetails([FromRoute] Guid arrowId)
        {
            return Json(_zaporArrowRepository.GetArrow(arrowId));
        }
    }  
}