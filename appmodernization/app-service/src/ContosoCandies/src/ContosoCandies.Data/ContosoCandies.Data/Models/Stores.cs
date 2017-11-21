using System;
using System.Collections.Generic;

namespace ContosoCandies.Data.Models
{
    public partial class Stores
    {
        public Stores()
        {
            Orders = new HashSet<Orders>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
