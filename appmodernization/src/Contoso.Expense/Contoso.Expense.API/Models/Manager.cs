using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Expense.API.Models
{
    public class Manager
    {
        public string EmployeeEmailAddress { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmailAddress { get; set; }
    }
}