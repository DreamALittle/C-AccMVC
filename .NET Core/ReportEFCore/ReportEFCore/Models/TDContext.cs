using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportEFCore.Models
{
    public class TDContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=192.168.20.159;Database=ReportDB; User=sa;Password=123456;");
        }

        public DbSet<Corp> Corp { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}
