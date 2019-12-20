using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace FTA.AICorrelation
{
    public class FunctionAppB
    {
        private readonly HttpClient _httpClient;
        private readonly string _httpProxyBaseUrl;

        public FunctionAppB(IHttpClientFactory clientFactory){
            this._httpClient = clientFactory.CreateClient();

            // fetch url for external http req inspection service
            this._httpProxyBaseUrl = Environment.GetEnvironmentVariable("httpProxyBaseUrl", EnvironmentVariableTarget.Process);
        }

        [FunctionName("ReceiveFromSvcBus")]
        public async Task Run([ServiceBusTrigger("%serviceBusQueueName%", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            Activity currActivity = Activity.Current;
            
            DumpActivity(currActivity, log);

            // sending to http bin container which runs in ACI
            await _httpClient.GetAsync(_httpProxyBaseUrl);
        }

        private void DumpActivity(Activity act, ILogger log)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Activity id: {act.Id}");
            sb.AppendLine($"Activity operation name: {act.OperationName}");
            sb.AppendLine($"Activity parent: {act.Parent}");
            sb.AppendLine($"Activity parent id: {act.ParentId}");
            sb.AppendLine($"Activity root id: {act.RootId}");
            foreach(var tag in act.Tags){
                sb.AppendLine($"  - Activity tag: {tag.Key}: {tag.Value}");
            }
            foreach(var bag in act.Baggage){
                sb.AppendLine($"  - Activity baggage: {bag.Key}: {bag.Value}");
            }

            Console.WriteLine(sb.ToString());
            log.LogInformation(sb.ToString());
        }

        [FunctionName("SendBackHttpResponseFromB")]
        public async Task<IActionResult> RunSendBackRespFromB(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "http")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# SendBackHttpResponseFromB HTTP trigger function processed a request.");
            Console.WriteLine("C# SendBackHttpResponseFromB HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            DumpActivity(Activity.Current, log);
            
            // sending to http bin container which runs in ACI
            log.LogInformation($"Calling http proxy at: {_httpProxyBaseUrl}");
            Console.WriteLine($"Calling http proxy at: {_httpProxyBaseUrl}");
            await _httpClient.GetAsync(_httpProxyBaseUrl);
            log.LogInformation($"Called http proxy at: {_httpProxyBaseUrl}");
            Console.WriteLine($"Called http proxy at: {_httpProxyBaseUrl}");

            return (ActionResult)new OkObjectResult($"");
        }
    }
}
