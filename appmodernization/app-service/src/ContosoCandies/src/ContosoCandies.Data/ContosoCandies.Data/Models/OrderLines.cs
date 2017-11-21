using System;
using System.Collections.Generic;

namespace ContosoCandies.Data.Models
{
    public partial class OrderLines
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public int? CandieId { get; set; }
        public int? OrderId { get; set; }

        public virtual Candies Candie { get; set; }
        public virtual Orders Order { get; set; }
    }
}
