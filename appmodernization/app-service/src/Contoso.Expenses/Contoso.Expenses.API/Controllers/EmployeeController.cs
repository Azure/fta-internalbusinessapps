using Contoso.Expenses.API.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Contoso.Expenses.API.Controllers
{
    /// <summary>
    /// Employee Controller to look up manager
    /// </summary>
    public class EmployeeController : ApiController
    {
        /// <summary>
        /// Returns Manager details based on employee name
        /// </summary>
        /// <param name="employeeName"></param>
        /// <returns></returns>
        [SwaggerOperation("GetManager")]
        [Route ("api/employee/getmanager/{employeeName}")]
        public EmployeeManager GetManager(string employeeName)
        {
            var employeeManager = new EmployeeManager();
            switch (employeeName)
            {
                case "Umar":
                case "Faisal":
                case "Abhishek":
                    employeeManager.ManagerName = "John Doe";
                    employeeManager.ManagerEmailAddress = "umarm@microsoft.com";
                    break;
                case "Randy":
                case "Sam":
                case "Anuja":
                    employeeManager.ManagerName = "Jane Doe";
                    employeeManager.ManagerEmailAddress = "faisalm@microsoft.com";
                    break;
                default:
                    employeeManager.ManagerName = "unknown";
                    break;
            }
            return employeeManager;
        }

    }
}
