using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReportEFCore.Models
{
    public class Area
    {
        [Key]
        public int AreaId { get; set; }
        public string AreaName { get; set; }
    }
}
