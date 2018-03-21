using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestOne.Model;

namespace TestOne
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<FuelPrice> FuelPrices { get; set; }

        public ApplicationDbContext() : base("Server=(localdb)\\mssqllocaldb;Database=testDBOne;Trusted_Connection=True;")
        {
        }
    }
}
