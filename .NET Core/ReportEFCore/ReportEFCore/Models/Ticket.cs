using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReportEFCore.Models
{
    public class Ticket
    {
        [Key]
        public int TicketID { get; set; }
        public DateTime TicketDate { get; set; }
        public string TicketLevel { get; set; }
        public int IssuerID { get; set; }
        public int ReceiverID { get; set; }
        public int ProductID { get; set; }
        public int ProductQuantity { get; set; }
        public string Commnet { get; set; }
        public string Issuer { get; set; }
        public string Receiver { get; set; }
    }
}
