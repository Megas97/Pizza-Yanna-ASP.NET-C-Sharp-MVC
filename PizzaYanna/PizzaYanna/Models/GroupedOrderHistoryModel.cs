using System.Collections.Generic;
using System.Linq;

namespace PizzaYanna.Models
{
    public class GroupedOrderHistoryModel
    {
        public IEnumerable<IGrouping<string, OrderHistory>> groupedOrderHistoryList { get; set; }
    }
}