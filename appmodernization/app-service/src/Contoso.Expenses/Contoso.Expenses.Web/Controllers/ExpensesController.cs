using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Contoso.Expenses.DataAccess.Models;
using Contoso.Expenses.Web.Models;
using Microsoft.Rest;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;

namespace Contoso.Expenses.Web.Controllers
{
    public class ExpensesController : Controller
    {
        private ContosoExpensesDataEntities db = new ContosoExpensesDataEntities();

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
            Expense expense = db.Expenses.Find(id);
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
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExpenseId,Purpose,Date,Cost_Center,Amount,Approver,Receipt")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                var employeeName = ConfigurationManager.AppSettings["EmployeeName"];

                //Call the Azure API App to get Manager details based on Employee Name
                var employeeManager = GetManagerDetails(employeeName);
                var approverEmail = employeeManager.ManagerEmailAddress;
                expense.Approver = employeeManager.ManagerName;

                // Send the expense to queue so that Azure Function can pick up and email to appover(manager)
                WriteToExpenseQueue(expense, approverEmail);

                db.Expenses.Add(expense);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(expense);
        }

        private EmployeeManager GetManagerDetails(string employeeName)
        {
            var employeeApiUri = ConfigurationManager.AppSettings["EmployeeApiUri"];
            ContosoExpensesAPI apiClient = new ContosoExpensesAPI(new CustomLoginCredentials());
            apiClient.BaseUri = new Uri(employeeApiUri);
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

        private void WriteToExpenseQueue(Expense expense, string approverEmail)
        {
            const string queueName = "contosoexpenses";
            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            ExpenseExtended expenseExtended = new ExpenseExtended
            {
                ExpenseId = expense.ExpenseId,
                Purpose = expense.Purpose,
                Date = expense.Date,
                Cost_Center = expense.Cost_Center,
                Amount = expense.Amount,
                Approver = expense.Approver,
                Receipt = expense.Receipt,
                ApproverEmail = approverEmail
            };
            var expenseJson = JsonConvert.SerializeObject(expenseExtended);

            // Write to Azure Storage Queue
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            //await queue.CreateIfNotExistsAsync();
            queue.CreateIfNotExists();
            CloudQueueMessage queueMessage = new CloudQueueMessage(expenseJson);
            //await queue.AddMessageAsync(queueMessage);
            queue.AddMessage(queueMessage);
        }


        public class ExpenseExtended : Expense
        {
            public string ApproverEmail { get; set; }
        }



        // GET: Expenses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expense expense = db.Expenses.Find(id);
            if (expense == null)
            {
                return HttpNotFound();
            }
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExpenseId,Purpose,Date,Cost_Center,Amount,Approver,Receipt")] Expense expense)
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
            Expense expense = db.Expenses.Find(id);
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
            Expense expense = db.Expenses.Find(id);
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
