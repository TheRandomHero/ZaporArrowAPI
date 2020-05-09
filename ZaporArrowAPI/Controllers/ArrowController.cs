using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ZaporArrowAPI.Entities;
using ZaporArrowAPI.ViewModels;
using ZaporArrowAPI.Services;
using Microsoft.AspNetCore.Hosting;


namespace ZaporArrowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArrowController : Controller
    {
        private readonly IZaporArrowRepository _zaporArrowRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArrowController(IWebHostEnvironment env, IZaporArrowRepository repository)
        {
            _zaporArrowRepository = repository;
            _webHostEnvironment = env;
        }

        ///------------------------------------------------------------------------------------
        ////// <summary>
        /// Get description from specific arrow for detailed page from DB
        /// </summary>
        /// <param name="arrowId">Id of required arrow</param>
        /// <returns>JSON response with description and length about required arrow</returns>
        [HttpGet("arrowDescription/{arrowId:guid}")]
        public JsonResult GetArrowDetails([FromRoute] Guid arrowId)
        {
            return Json(_zaporArrowRepository.GetArrow(arrowId));
        }

        /// <summary>
        /// Create new Arrow object and a profile picture for beginning
        /// </summary>
        /// <param name="model">A view model with description, image,</param>
        /// <returns>The new arrow id and frontend redirects to update page</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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





        /// <summary>
        /// Delete specific Arrow object and all the associated images
        /// </summary>
        /// <param name="arrowId">Id of required Arrow object</param>
        /// <returns>200 OK if it was successfull or 404 Not found if Id doesn't exist. Otherwise exception thrown</returns>
        [HttpDelete]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> Delete([FromQuery]Guid arrowId)
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
                    var images = _zaporArrowRepository.GetAllImageWithSameArrowId(arrowId);
                    if(images != null)
                    {
                        foreach (var image in images)
                        {
                            if (System.IO.File.Exists(image.ImageSource))
                            {
                                System.IO.File.Delete(image.ImageSource);
                            }
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
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> UpdateArrowDetails(Guid arrowId, [FromForm]ArrowViewModel model)
        {
            if (!ModelState.IsValid) return StatusCode(400, Json("Model is not valid"));

            _zaporArrowRepository.UpdateArrowDetails(arrowId, model);

            return StatusCode(200, Json("Update was succesful"));


        }

    }
}
