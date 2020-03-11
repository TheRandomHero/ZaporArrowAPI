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

        /// <summary>
        /// Create new Arrow object and a profile picture for beginning
        /// </summary>
        /// <param name="model">A view model with description, image,</param>
        /// <returns>The new arrow id and frontend redirects to update page</returns>
        [HttpPost]
        public async Task<string> PostNewArrow([FromForm]ArrowViewModel model)
        {
            try
            {
                if (model.PhotoFile.Length > 0)
                {
                    if (!Directory.Exists(_webHostEnvironment.WebRootPath + "\\images\\"))
                    {
                        Directory.CreateDirectory(_webHostEnvironment.WebRootPath + "\\images\\");
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PhotoFile.FileName;
                    using FileStream fileStream = System.IO.File.Create(_webHostEnvironment.WebRootPath + "\\images\\" + uniqueFileName);
                    model.PhotoFile.CopyTo(fileStream);
                    fileStream.Flush();

                    Arrow newArrow = new Arrow  /// New Arrow object
                    {
                        ArrowId = Guid.NewGuid(),
                        Length = model.Length,
                        Description = model.Description,
                        Images = new List<Image>(),
                    };

                    Image newImage = new Image  ///Image Object
                    {
                        ImageId = Guid.NewGuid(),
                        ArrowId = newArrow.ArrowId,
                        ImageSource = _webHostEnvironment.WebRootPath + "\\images\\" + uniqueFileName,
                        isProfilePicture = true
                    };

                    _zaporArrowRepository.AddArrow(newArrow);
                    _zaporArrowRepository.AddImage(newImage);

                    return newArrow.ArrowId.ToString();
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

        [HttpPost("{arrowId}")]
        public async Task<IActionResult> UploadImagesToExistingArrow(Guid arrowId, [FromForm]UploadImage image)
        {
            if(_zaporArrowRepository.GetArrow(arrowId) != null && image.file.Length > 0)
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
        /// Get description from specific arrow for detailed page from DB
        /// </summary>
        /// <param name="arrowId">Id of required arrow</param>
        /// <returns>JSON response with description and length about required arrow</returns>
        [HttpGet("arrow/{arrowId:guid}")]
        public JsonResult GetArrowDetails([FromRoute] Guid arrowId)
        {
            return Json(_zaporArrowRepository.GetArrow(arrowId));
        }

        //[HttpGet("image/{arrowId:guid}")]
        //public JsonResult GetAllImageForArrow([FromRoute] Guid arrowId)
        //{
        //    return Json(_zaporArrowRepository.GetAllConnectedImagesForArrow(arrowId));
        //}

        /// <summary>
        /// Delete specific Arrow object and all the associated images
        /// </summary>
        /// <param name="arrowId">Id of required Arrow object</param>
        /// <returns>200 OK if it was successfull or 404 Not found if Id doesn't exist. Otherwise exception thrown</returns>
        [HttpDelete("{arrowId}")]
        public async Task<IActionResult> Delete(Guid arrowId)
        {
            try
            {
                var arrowEntity = _zaporArrowRepository.GetArrow(arrowId);
                if (arrowEntity == null)
                {
                    return StatusCode(404, Json("Arrow under " + arrowId.ToString() + " was not found"));
                }
                else
                {
                    var images = _zaporArrowRepository.GetAllImagesWithSameArrowId(arrowId);
                    foreach (var image in images)
                    {
                        if (System.IO.File.Exists(image.ImageSource))
                        {
                            System.IO.File.Delete(image.ImageSource);
                        }
                    }
                    _zaporArrowRepository.DeleteArrow(arrowEntity);

                    return StatusCode(200);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(400, Json(ex));
            };
        }

        /// <summary>
        /// For updating existing arrow details
        /// </summary>
        /// <param name="arrowId">Required arrow's Id</param>
        /// <param name="model">Required changes</param>
        /// <returns></returns>
        [HttpPut("{arrowId}")]
        public async Task<IActionResult> UpdateArrowDetails(Guid arrowId,[FromForm]ArrowViewModel model)
        {
            if (!ModelState.IsValid) return StatusCode(400, Json("Model is not valid"));

            _zaporArrowRepository.UpdateArrowDetails(arrowId, model);

            return StatusCode(200, Json("Update was succesful"));
           

        }
    }
}