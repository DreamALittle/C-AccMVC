using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReportEFCore.Models
{
    public class Corp
    {
        [Key]
        public int CorpID { get; set; }
        public string CorpName { get; set; }
        public string CorpLevel { get; set; }
        public int? AreaID { get; set; }
        public string AreaName { get; set; }
        public int? ParentCorpID { get; set; }
    }
}
