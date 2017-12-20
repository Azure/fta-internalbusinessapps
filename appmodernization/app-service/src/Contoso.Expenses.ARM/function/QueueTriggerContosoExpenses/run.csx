#r "Newtonsoft.Json"
#r "SendGrid"

using System;
using System.Net;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

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

public static async Task Run(string expenseItem, TraceWriter log, IAsyncCollector<Mail> emailMessage)
{
   var expense = JsonConvert.DeserializeObject<ExpenseExtended>(expenseItem);

   var emailFrom = "expense@contoso.com";

   var emailTo = expense.ApproverEmail;
   var emailBody = $"Hello {expense.Approver}, \r\n New Expense report submitted for the purpose of: {expense.Purpose}. \r\n Please review as soon as possible.\r\n This is a auto generated email, please do not reply to this email";
   var emailSubject = $"New Expense for the amount of ${expense.Amount} submitted";

   log.Info($"Email To: {emailTo}");
   log.Info($"Email Subject: {emailSubject}");
   log.Info($"Email Body: {emailBody}");

   Mail expenseMessage = new Mail();
   var personalization = new Personalization();
   personalization.AddTo(new Email(emailTo));
   expenseMessage.AddPersonalization(personalization);

   var messageContent = new Content("text/html", emailBody);
   expenseMessage.AddContent(messageContent);
   expenseMessage.Subject = emailSubject;
   expenseMessage.From = new Email(emailFrom);

   await emailMessage.AddAsync(expenseMessage);
}