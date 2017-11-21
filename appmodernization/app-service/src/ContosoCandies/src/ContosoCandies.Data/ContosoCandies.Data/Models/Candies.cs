using System;
using System.Collections.Generic;

namespace ContosoCandies.Data.Models
{
    public partial class Candies
    {
        public Candies()
        {
            OrderLines = new HashSet<OrderLines>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public double? Price { get; set; }

        public virtual ICollection<OrderLines> OrderLines { get; set; }
    }
}
