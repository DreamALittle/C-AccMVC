using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportEFCore.SearchModel
{
    public class CrossSearch
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Area { get; set; }
        public string Office { get; set; }
        public string Product { get; set; }
    }
}
