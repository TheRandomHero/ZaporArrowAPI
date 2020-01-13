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

        public IEnumerable<Arrow> GetAllArrowsWithImages()
        {
            return _zaporArrowContext.Arrows
                .Include(t => t.Images).ToList();
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
    }
}
