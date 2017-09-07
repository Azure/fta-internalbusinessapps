using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace Contoso.Expenses.Functions
{

       public static class Expenses
    {

        public class ExpenseExtended
        {
            public int ExpenseId { get; set; }
            public string Purpose { get; set; }
            public Nullable<System.DateTime> Date { get; set; }
            public string Cost_Center { get; set; }
            public Nullable<double> Amount { get; set; }
            public string Approver { get; set; }
            public string Receipt { get; set; }
            public string ApproverEmail { get; set; }
        }


        [FunctionName("ExpenseEmailFunction")]        
        public static async void Run([QueueTrigger("contosoexpenses", Connection = "StorageConnectionString")]string expenseItem, TraceWriter log, 
                                      IAsyncCollector<SendGridMessage> message)
        {
            var expense = JsonConvert.DeserializeObject<ExpenseExtended>(expenseItem);
            var emailFrom = "Expense@ContosoExpenses.com";
            var emailTo = expense.ApproverEmail;
            var emailSubject = $"New Expense for the amount of ${expense.Amount} submitted";
            var emailBody = $"Hello {expense.Approver}, <br/> New Expense report submitted for the purpose of: {expense.Purpose}. <br/> Please review as soon as possible. <br/> <br/> <br/> This is a auto generated email, please do not reply to this email";

            log.Info($"Email Subject: {emailSubject}");
            log.Info($"Email body: {emailBody}");
            
            SendGridMessage expenseMessage = new SendGridMessage();
            expenseMessage.From = new EmailAddress(emailFrom, "Contoso Expenses");
            expenseMessage.AddTo(emailTo, expense.Approver);
            expenseMessage.Subject = emailSubject;
            expenseMessage.AddContent(MimeType.Html, emailBody);

            await message.AddAsync(expenseMessage);
        }
    }
}
