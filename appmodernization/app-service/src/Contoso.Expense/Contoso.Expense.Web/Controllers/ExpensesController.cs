using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Contoso.Expense.Entities.Models;
using System.Security.Claims;
using Contoso.Expense.Web.Models;
using Microsoft.Rest;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Contoso.Expense.Web.Controllers
{
    [Authorize]
    public class ExpensesController : Controller
    {
        private ContosoExpenseDataEntities db = new ContosoExpenseDataEntities();

        // GET: Expenses
        public ActionResult Index()
        {
            return View(db.Expenses.ToList());
        }

        // GET: Expenses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contoso.Expense.Entities.Models.Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }

        // GET: Expenses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExpenseId,Purpose,ExpenseDate,Amount,EmployeeName,EmployeeEmail,ApproverName,ApproverEmail,Receipt")] Contoso.Expense.Entities.Models.Expense expense)
        {
            if (ModelState.IsValid)
            {
                // Get the logged in Identity
                var identity = (ClaimsIdentity)User.Identity;
                IEnumerable<Claim> claims = identity.Claims;

                // As the Name claim returns Email address, using Surname & Givenname to get full name
                var fullName = claims.Where(c => c.Type == ClaimTypes.GivenName).FirstOrDefault().Value + " " + 
                               claims.Where(c => c.Type == ClaimTypes.Surname).FirstOrDefault().Value;

                // Call Azure API to get Manager details based on employee name
                Manager manager = GetManagerDetails(fullName);

                // Update the Expense instance with Manager details
                expense.EmployeeName = fullName;
                expense.EmployeeEmail = manager.EmployeeEmailAddress;
                expense.ApproverName = manager.ManagerName;
                expense.ApproverEmail = manager.ManagerEmailAddress;

                // Send the Expense to Azure Storage Queue so that Azure Function can pick up and email to appover(manager)
                WriteToExpenseQueue(expense);

                // Save the Expense to database
                db.Expenses.Add(expense);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(expense);
        }

        private void WriteToExpenseQueue(Entities.Models.Expense expense)
        {
            const string queueName = "contosoexpense";
            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            var expenseJson = JsonConvert.SerializeObject(expense);

            //Write to Azure Storage Queue
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();
            CloudQueueMessage queueMessage = new CloudQueueMessage(expenseJson);
            queue.AddMessage(queueMessage);
        }

        private Manager GetManagerDetails(string employeeName)
        {
            var employeeUri = ConfigurationManager.AppSettings["EmployeeApiUri"];
            ContosoExpenseAPI apiClient = new ContosoExpenseAPI(new CustomLoginCredentials());
            apiClient.BaseUri = new Uri(employeeUri);
            return (apiClient.GetManager(employeeName));
        }
        public class CustomLoginCredentials : ServiceClientCredentials
        {
            private string AuthenticationToken { get; set; }
            public override void InitializeServiceClient<T>(ServiceClient<T> client)
            {
            }
            public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
            }
        }

        // GET: Expenses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contoso.Expense.Entities.Models.Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExpenseId,Purpose,ExpenseDate,Amount,EmployeeName,EmployeeEmail,ApproverName,ApproverEmail,Receipt")] Contoso.Expense.Entities.Models.Expense expense)
        {
            if (ModelState.IsValid)
            {
                db.Entry(expense).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contoso.Expense.Entities.Models.Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contoso.Expense.Entities.Models.Expense expense = db.Expenses.Find(id);
            db.Expenses.Remove(expense);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
