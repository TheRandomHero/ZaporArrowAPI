using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZaporArrowAPI.Entities;

namespace ZaporArrowAPI.Services
{
    public interface IZaporArrowRepository
    {
        List<Guid> GetAllProfilePictures();
        Arrow GetArrow(Guid id);
        Image GetImage(Guid id);
        void AddArrow(Arrow arrow);
        void AddImage(Image image);
        void AddImageToArrow(Guid arrowId, Image image);
        void DeleteArrow(Arrow arrow);
        void UpdateArrowDetails(Arrow arrow);
    }
}
