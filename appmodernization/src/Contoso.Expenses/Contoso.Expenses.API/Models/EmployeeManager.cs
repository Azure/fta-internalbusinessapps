using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Expenses.API.Models
{
    public class EmployeeManager
    {
        public string EmployeeName { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmailAddress { get; set; }
    }
}