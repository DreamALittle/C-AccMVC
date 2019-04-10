using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportEFCore.Models
{
    public class HaiwoContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=192.168.20.189;Database=acctrueHaiwo; User=sa;Password=123;");
        }

        public DbSet<Corps> Corps { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
