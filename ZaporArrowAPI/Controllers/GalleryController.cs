using Microsoft.AspNetCore.Mvc;
using ZaporArrowAPI.Services;

namespace ZaporArrowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryController : Controller
    {
        private readonly IZaporArrowRepository _zaporArrowRepository;
        public GalleryController(IZaporArrowRepository repository)
        {
            _zaporArrowRepository = repository;
        }

        /// <summary>
        /// For Gallery page collects all the Ids where Images isProfilePicture field is true
        /// </summary>
        /// <returns>JSON of ids</returns>
        [HttpGet]
        public JsonResult GetAllProfilePicturesIds()
        {
            var result = _zaporArrowRepository.GetAllProfilePictures();
            return Json(result);
        }

    }
}