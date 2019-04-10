using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportEFCore.ReportViewModel
{
    public class IssuerArea
    {
        public int IssuerID { get; set; }
        public string IssuerParentArea{ get; set; }
        public string IssuerParentName{ get; set; }
    }
}
