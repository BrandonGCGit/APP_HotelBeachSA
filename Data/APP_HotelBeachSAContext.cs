using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APP_HotelBeachSA.Models;
using APP_HotelBeachSA.Model;

namespace APP_HotelBeachSA.Data
{
    public class APP_HotelBeachSAContext : DbContext
    {
        public APP_HotelBeachSAContext (DbContextOptions<APP_HotelBeachSAContext> options)
            : base(options)
        {
        }

        public DbSet<APP_HotelBeachSA.Models.Discount> Discount { get; set; } = default!;
        public DbSet<APP_HotelBeachSA.Models.Paquete> Paquete { get; set; } = default!;
        public DbSet<APP_HotelBeachSA.Models.Pago> Pago { get; set; } = default!;
        public DbSet<APP_HotelBeachSA.Models.Cheque> Cheque { get; set; } = default!;
        public DbSet<APP_HotelBeachSA.Models.Reservacion> Reservacion { get; set; } = default!;
        public DbSet<APP_HotelBeachSA.Model.Usuario> Usuario { get; set; } = default!;

    }
}
