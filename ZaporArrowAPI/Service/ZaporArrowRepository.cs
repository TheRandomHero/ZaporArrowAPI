﻿using Microsoft.EntityFrameworkCore;
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
        public void AddImage(Image image)
        {
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
            if(arrow == null)
            {
                throw new ArgumentNullException(nameof(arrow));
            }
            else
            {
                var images = GetAllImageIdsWithSameArrowId(arrow.ArrowId);
                foreach(var image in images)
                {
                    _zaporArrowContext.Images.Remove(image);
                }
                _zaporArrowContext.Arrows.Remove(arrow);
                _zaporArrowContext.SaveChanges();

            }
            
        }

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
        public List<Image> GetAllImageIdsWithSameArrowId(Guid arrowId)
        {
            return _zaporArrowContext.Images.Where(i => i.ArrowId == arrowId).ToList();
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
            return _zaporArrowContext.Images.Where(t => t.ImageId == id).FirstOrDefault();
        }

        public void UpdateArrowDetails(Guid arrowId, ArrowViewModel model)
        {
            var existingArrow = GetArrow(arrowId);

            if(existingArrow != null && model != null)
            {
                existingArrow.Description = model.Description;

                _zaporArrowContext.SaveChanges();
            }
        }
        public List<Guid> GetAllConnectedImagesForArrow(Guid arrowId)
        {
            var images = _zaporArrowContext.Images.Where(t => t.ArrowId == arrowId && t.isProfilePicture == false).ToList();
            var ids = new List<Guid>();
            foreach (var image in images)
            {
                ids.Add(image.ImageId);
            }

            return ids;
        }


    }
}
