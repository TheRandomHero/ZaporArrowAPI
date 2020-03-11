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
        Dictionary<Guid, Guid> GetAllProfilePicturesIds();
        List<Guid> GetAllConnectedImagesForArrow(Guid arrowId);
        Arrow GetArrow(Guid id);
        Image GetImage(Guid id);
        List<Image> GetAllImagesWithSameArrowId(Guid arrowId);
        void AddArrow(Arrow arrow);
        void AddImage(Image image);
        void DeleteArrow(Arrow arrow);
        void UpdateArrowDetails(Guid arrowId, ArrowViewModel model);

    }
}
