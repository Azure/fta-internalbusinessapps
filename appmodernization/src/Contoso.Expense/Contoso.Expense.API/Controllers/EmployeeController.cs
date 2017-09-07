using Contoso.Expense.API.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Contoso.Expense.API.Controllers
{
    /// <summary>
    /// Employee Controller to look up Manager
    /// </summary>
    public class EmployeeController : ApiController
    {
        /// <summary>
        /// Returns Manager details based on Employee Email Address
        /// </summary>
        /// <param name="employeeName"></param>
        /// <returns></returns>
        [SwaggerOperation("GetManager")]
        [Route("api/employee/getmanager/{employeeName}")]
        public Manager GetManager(string employeeName)
        {
            var manager = new Manager();

            switch (employeeName)
            {
                case "Umar Mohamed Usman":
                case "Faisal Mustafa":
                    manager.EmployeeEmailAddress = employeeName;
                    manager.ManagerName = "Igor Katic";
                    manager.ManagerEmailAddress = "umarm@microsoft.com";
                    break;
                case "Randy Pagels":
                case "Sam Portelli":
                    manager.EmployeeEmailAddress = employeeName;
                    manager.ManagerName = "Anusha Rangaswamy";
                    manager.ManagerEmailAddress = "faisalm@microsoft.com";
                    break;
                case "Jelle Druyts":
                case "Lara Leite":
                    manager.EmployeeEmailAddress = employeeName;
                    manager.ManagerName = "Kelly Mondloch";
                    manager.ManagerEmailAddress = "jelled@microsoft.com";
                    break;
                default:
                    manager.EmployeeEmailAddress = employeeName;
                    manager.ManagerName = "unknown";
                    manager.ManagerEmailAddress = "umarm@microsoft.com";
                    break;
            }

            return manager;
        }
    }
}
