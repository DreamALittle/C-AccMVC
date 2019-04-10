using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ReportEFCore.Models;

namespace ReportEFCore.ReportViewModel
{
    public class TicketView:Ticket
    {
        public TicketView()
        {
            this.Issuer = base.Issuer;
        }
        public Product Product { get; set; }
        public IssuerArea IssuerArea { get; set; }
    }
}
