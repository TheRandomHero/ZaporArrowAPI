using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZaporArrowAPI.DbContexts;
using ZaporArrowAPI.Entities;
using ZaporArrowAPI.ViewModels;

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

        /// <summary>
        /// Adding Image entity to database 
        /// </summary>
        /// <param name="image"></param>
        public void AddImage(Image image)
        {
            _zaporArrowContext.Images.Add(image);
            _zaporArrowContext.SaveChanges();
        }

        /// <summary>
        /// Adding Arrow entity to database
        /// </summary>
        /// <param name="arrow"></param>
        public void AddArrow(Arrow arrow)
        {
            _zaporArrowContext.Arrows.Add(arrow);
            _zaporArrowContext.SaveChanges();
        }

        /// <summary>
        /// Deleting Arrow from database with connected images
        /// </summary>
        /// <param name="arrow"></param>
        public void DeleteArrow(Arrow arrow)
        {
            if(arrow == null)
            {
                throw new ArgumentNullException(nameof(arrow));
            }
            else
            {
                var images = GetAllImageWithSameArrowId(arrow.ArrowId);
                foreach(var image in images)
                {
                    _zaporArrowContext.Images.Remove(image);
                }
                _zaporArrowContext.Arrows.Remove(arrow);
                _zaporArrowContext.SaveChanges();

            }
            
        }

        /// <summary>
        /// Deleting single image from database
        /// </summary>
        /// <param name="image"></param>
        public void DeleteImage(Image image)
        {
            _zaporArrowContext.Images.Remove(image);
            _zaporArrowContext.SaveChanges();
        }


        /// <summary>
        /// Filter all profile pictures for gallery
        /// </summary>
        /// <returns>A dictionary with ids. The keys are Images's Ids, the values are Arrow's Ids  </returns>
        public Dictionary<Guid, Guid> GetAllProfilePictures()
        {
            var profilePictures = _zaporArrowContext.Images.Where(t=> t.isProfilePicture == true).ToList();
            var ids = new Dictionary<Guid, Guid>();
            foreach(var image in profilePictures)
            {
                ids.Add(image.ImageId, image.ArrowId);
            }
            return ids;
        }

        /// <summary>
        /// Get all images for specific arrow 
        /// </summary>
        /// <param name="arrowId"></param>
        /// <returns>List of Images entities</returns>
        public List<Image> GetAllImageWithSameArrowId(Guid arrowId)
        {
            return _zaporArrowContext.Images.Where(i => i.ArrowId == arrowId).ToList();
        }

        /// <summary>
        /// Get specific Arrow
        /// </summary>
        /// <param name="arrowId"></param>
        /// <returns>Arrow entity</returns>
        public Arrow GetArrow(Guid arrowId)
        {
            if (arrowId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(arrowId));
            }

            return _zaporArrowContext.Arrows.Include(t => t.Images)
                .Where(t => t.ArrowId == arrowId).FirstOrDefault();
        }

        /// <summary>
        /// Get specific Image 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Image entity</returns>
        public Image GetImage(Guid id)
        {
            return _zaporArrowContext.Images.Where(t => t.ImageId == id).FirstOrDefault();
        }

        /// <summary>
        /// Update existing Arrow details
        /// </summary>
        /// <param name="arrowId">Specific arrow Id</param>
        /// <param name="model">Contains required changes for Arrow</param>
        public void UpdateArrowDetails(Guid arrowId, ArrowViewModel model)
        {
            var existingArrow = GetArrow(arrowId);

            if(existingArrow != null && model != null)
            {
                existingArrow.Description = model.Description;

                _zaporArrowContext.SaveChanges();
            }
        }
        /// <summary>
        /// Get all images ids for specific arrow
        /// </summary>
        /// <param name="arrowId"></param>
        /// <returns>List of Ids</returns>
        public List<Guid> GetAllConnectedImagesForArrow(Guid arrowId)
        {
            var images = _zaporArrowContext.Images.Where(t => t.ArrowId == arrowId).ToList();
            var ids = new List<Guid>();
            foreach (var image in images)
            {
                ids.Add(image.ImageId);
            }

            return ids;
        }


    }
}
