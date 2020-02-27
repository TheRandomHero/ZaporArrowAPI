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

        [HttpGet]
        public JsonResult GetAllArrowsIds()
        {
            var result = _zaporArrowRepository.GetAllProfilePictures();
            return Json(result);
        }

    }
}