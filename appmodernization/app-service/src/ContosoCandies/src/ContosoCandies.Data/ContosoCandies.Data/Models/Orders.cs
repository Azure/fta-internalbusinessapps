using System;
using System.Collections.Generic;

namespace ContosoCandies.Data.Models
{
    public partial class Orders
    {
        public Orders()
        {
            OrderLines = new HashSet<OrderLines>();
        }

        public int Id { get; set; }
        public DateTimeOffset? Date { get; set; }
        public double? Price { get; set; }
        public string Status { get; set; }
        public int? StoreId { get; set; }

        public virtual ICollection<OrderLines> OrderLines { get; set; }
        public virtual Stores Store { get; set; }
    }
}
