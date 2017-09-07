using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Contoso.Expense.Functions
{
    public static class ExpenseEmailFn
    {

        public partial class Expense
        {
            public int ExpenseId { get; set; }
            public string Purpose { get; set; }
            public Nullable<System.DateTime> ExpenseDate { get; set; }
            public Nullable<double> Amount { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeEmail { get; set; }
            public string ApproverName { get; set; }
            public string ApproverEmail { get; set; }
            public string Receipt { get; set; }
        }

        [FunctionName("ExpenseEmailFn")]        
        public static async void Run([QueueTrigger("contosoexpense", Connection = "StorageConnectionString")]string expenseItem, TraceWriter log,
            IAsyncCollector<SendGridMessage> message)
        {
            var expense = JsonConvert.DeserializeObject<Expense>(expenseItem);

            var emailFrom = "Expense@ContosoExpense.com";
            var emailTo = expense.ApproverEmail;
            var emailSubject = $"New Expense for the amount of ${expense.Amount} submitted";

            var emailBody = $"Hello {expense.ApproverName}, <br/> New Expense report submitted by {expense.EmployeeName} for the purpose of: {expense.Purpose}. <br/> Please review as soon as possible. <br/> <br/> <br/> This is a auto generated email, please do not reply to this email";

            log.Info($"Email Subject: {emailSubject}");
            log.Info($"Email body: {emailBody}");

            SendGridMessage expenseMessage = new SendGridMessage();
            expenseMessage.From = new EmailAddress(emailFrom, "Contoso Expense");
            expenseMessage.AddTo(emailTo, expense.ApproverName);
            expenseMessage.Subject = emailSubject;
            expenseMessage.AddContent(MimeType.Html, emailBody);

            await message.AddAsync(expenseMessage);
        }
    }
}
