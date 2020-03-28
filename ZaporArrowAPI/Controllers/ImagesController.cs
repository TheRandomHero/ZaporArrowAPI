using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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


        [Authorize(Roles = "Admin")]
        [HttpPost("{arrowId}")]
        public async Task<IActionResult> UploadImagesToExistingArrow(Guid arrowId, [FromForm]UploadImage image)
        {
            if (_zaporArrowRepository.GetArrow(arrowId) != null && image.file.Length > 0)
            {

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.file.FileName;
                using FileStream fileStream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\images\\" + uniqueFileName);
                image.file.CopyTo(fileStream);
                fileStream.Flush();

                var newImage = new Image
                {
                    ImageId = Guid.NewGuid(),
                    ArrowId = arrowId,
                    ImageSource = _webHostEnvironment.WebRootPath + "\\images\\" + uniqueFileName,
                    isProfilePicture = false,
                };

                _zaporArrowRepository.AddImage(newImage);


                return StatusCode(200, Json("Upload was successful"));
            }
            else
            {
                return StatusCode(400, Json("Arrow does not exist under Id " + arrowId.ToString()));
            }
        }

        /// <summary>
        /// Retriever for static files in ZaporArrow web API
        /// </summary>
        /// <param name="imgId">Id of required image</param>
        /// <returns>Image file</returns>
        [HttpGet("{imgId:guid}")]
        public async Task<IActionResult> GetArrowImage([FromRoute]Guid imgId)
        {
            try
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                var path = _zaporArrowRepository.GetImage(imgId).ImageSource;

                var ext = "image/" + System.IO.Path.GetExtension(path);

                FileStream image = System.IO.File.OpenRead(path);

                return File(image, ext);

            }
            catch (Exception ex)
            {

                return StatusCode(400, Json(ex));
            };

        }


        /// <summary>
        /// Get all existing images ids for specific arrow.
        /// </summary>
        /// <param name="arrowId">Id of the required arrow</param>
        /// <returns>Images Ids</returns>
        [HttpGet("image/{arrowId:guid}")]
        public JsonResult GetAllImageForArrow([FromRoute] Guid arrowId)
        {
            return Json(_zaporArrowRepository.GetAllConnectedImagesForArrow(arrowId));
        }



        /// <summary>
        /// Delete single Image
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [HttpDelete("{imageId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteImage([FromBody] Guid imageId)
        {
            var imageToDelete = _zaporArrowRepository.GetImage(imageId);

            if (System.IO.File.Exists(imageToDelete.ImageSource))
            {
                System.IO.File.Delete(imageToDelete.ImageSource);

                _zaporArrowRepository.DeleteImage(imageToDelete);

                return StatusCode(200);
            }
            else
            {
                return StatusCode(400, Json("Can't find the image"));
            }
        }
    }

}
