using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZaporArrowAPI.Entities;
using ZaporArrowAPI.ViewModels;

namespace ZaporArrowAPI.Services
{
    public interface IZaporArrowRepository
    {
        Dictionary<Guid, Guid> GetAllProfilePictures();
        Arrow GetArrow(Guid id);
        Image GetImage(Guid id);
        List<Image> GetAllImageIdsWithSameArrowId(Guid arrowId);
        void AddArrow(Arrow arrow);
        void AddImage(Image image);
        void DeleteArrow(Arrow arrow);
        void DeleteImage(Image image);
        void UpdateArrowDetails(Guid arrowId, ArrowViewModel model);
        public List<Guid> GetAllConnectedImagesForArrow(Guid arrowId);

    }
}
