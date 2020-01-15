using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZaporArrowAPI.DbContexts;
using ZaporArrowAPI.Entities;

namespace ZaporArrowAPI.Services
{
    public class ZaporArrowRepository : IZaporArrowRepository
    {
        private readonly ZaporArrowContext _zaporArrowContext;

        public ZaporArrowRepository(ZaporArrowContext context)
        {
            _zaporArrowContext = context ??
                throw new ArgumentNullException(nameof(context));
        }
        public void AddImage(Image image)
        {
            _zaporArrowContext.Images.Add(image);
            _zaporArrowContext.SaveChanges();
        }

        public void AddImageToArrow(Guid arrowId, Image image)
        {
            var theArrow = GetArrow(arrowId);
            theArrow.Images.Add(image);
            _zaporArrowContext.Images.Add(image);
            _zaporArrowContext.SaveChanges();
        }

        public void AddArrow(Arrow arrow)
        {
            _zaporArrowContext.Arrows.Add(arrow);
            _zaporArrowContext.SaveChanges();
        }

        public void DeleteArrow(Arrow arrow)
        {
            throw new NotImplementedException();
        }

        public List<Guid> GetAllArrowsIds()
        {
            var allArrows = _zaporArrowContext.Arrows.ToList();
            List<Guid> ids = new List<Guid>();
            foreach(var arrow in allArrows)
            {
                ids.Add(arrow.ArrowId);
            }
            return ids;
        }

        public Arrow GetArrow(Guid arrowId)
        {
            if (arrowId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(arrowId));
            }

            return _zaporArrowContext.Arrows.Include(t => t.Images)
                .Where(t => t.ArrowId == arrowId).FirstOrDefault();
        }

        public Image GetImage(Guid id)
        {
            return _zaporArrowContext.Images.Where(t => t.ArrowId == id).FirstOrDefault();
        }
    }
}
