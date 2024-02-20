using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APP_HotelBeachSA.Models;

namespace APP_HotelBeachSA.Data
{
    public class APP_HotelBeachSAContext : DbContext
    {
        public APP_HotelBeachSAContext (DbContextOptions<APP_HotelBeachSAContext> options)
            : base(options)
        {
        }

        public DbSet<APP_HotelBeachSA.Models.Discount> Discount { get; set; } = default!;
    }
}
