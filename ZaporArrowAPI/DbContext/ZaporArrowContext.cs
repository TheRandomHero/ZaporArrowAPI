using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZaporArrowAPI.Entities;

namespace ZaporArrowAPI.DbContexts
{
    public class ZaporArrowContext : IdentityDbContext
    {
        public ZaporArrowContext(DbContextOptions<ZaporArrowContext> options)
            : base(options)
        {
        }

        public DbSet<Arrow> Arrows { get; set; }

        public DbSet<Image> Images { get; set; }
    }
}