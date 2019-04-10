using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.Model
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Des { get; set; }
        public decimal Price { get; set; }
        public int CategoryID { get; set; }
    }
}
