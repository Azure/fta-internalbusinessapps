using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FTA.AICorrelation
{
    public class FunctionAppA
    {

        private readonly HttpClient _httpClient;
        private readonly string _logicAppAUrl;
        private readonly string _functionAppBUrl;
        private readonly string _httpBinHost;
        private readonly string _httpBinUrl;

        private readonly string _httpProxyBaseUrl;

        public FunctionAppA(IHttpClientFactory clientFactory){
            this._httpClient = clientFactory.CreateClient();

            // fetch url for external http req inspection service
            this._httpProxyBaseUrl = Environment.GetEnvironmentVariable("httpProxyBaseUrl", EnvironmentVariableTarget.Process);

            // fetch url for logic app A
            this._logicAppAUrl = Environment.GetEnvironmentVariable("logicAppAUrl", EnvironmentVariableTarget.Process);

            //fetch url for function app B
            this._functionAppBUrl = Environment.GetEnvironmentVariable("functionAppBUrl", EnvironmentVariableTarget.Process);
        }

        [FunctionName("InitiateFlowToQueue")]
        public IActionResult RunQueue(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "queue/{submissionId}")] HttpRequest req,
            string submissionId,
            [ServiceBus("%serviceBusQueueName%", Connection = "ServiceBusConnection")] out Message msg,
            ILogger log)
        {
            log.LogInformation($"C# InitiateFlowToQueue HTTP trigger function processed a request with submission id: {submissionId}.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            var initialMessage = new {
                IntialSubmissionId = submissionId
            };

            var msgBody = JsonConvert.SerializeObject(initialMessage);

            msg = new Message(Encoding.UTF8.GetBytes(msgBody));
            
            Activity currActivity = Activity.Current;
            currActivity.AddTag("MySubmissionId", submissionId);

            return (ActionResult)new OkObjectResult($"");
        }

        [FunctionName("InitiateFlowToQueueWithCustomBagage")]
        public IActionResult RunQueueWithCustomBagage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "queue-with-bagage/{submissionId}")] HttpRequest req,
            string submissionId,
            [ServiceBus("%serviceBusQueueName%", Connection = "ServiceBusConnection")] out Message msg,
            ILogger log)
        {
            log.LogInformation($"C# InitiateFlowToQueueWithCustomBagage HTTP trigger function processed a request with submission id: {submissionId}.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            var initialMessage = new {
                IntialSubmissionId = submissionId
            };

            var msgBody = JsonConvert.SerializeObject(initialMessage);

            msg = new Message(Encoding.UTF8.GetBytes(msgBody));
            
            Activity currActivity = Activity.Current;

            currActivity.AddTag("MyCustomCorrId", submissionId);
            currActivity.AddBaggage("MyCustomCorrId", submissionId);

            // currActivity.AddTag("MySubmissionId", submissionId);

            DumpActivity(Activity.Current, log);

            return (ActionResult)new OkObjectResult($"");
        }

        [FunctionName("InitiateFlowToExternalHTTPUrl")]
        public async Task<IActionResult> RunHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "callexternalhttpurl")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# InitiateFlowToHTTP HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            // sending to reverse proxy which allows request inspection and runs in ACI:
            log.LogInformation($"Calling http proxy at: {_httpProxyBaseUrl}");
            Console.WriteLine($"Calling http proxy at: {_httpProxyBaseUrl}");
            await _httpClient.GetAsync(_httpProxyBaseUrl);
            log.LogInformation($"Called http proxy at: {_httpProxyBaseUrl}");
            Console.WriteLine($"Called http proxy at: {_httpProxyBaseUrl}");

            return (ActionResult)new OkObjectResult($"");
        }

        [FunctionName("InitiateFlowToHTTPWithResponseFromB")]
        public async Task<IActionResult> RunHttpWithResp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "callhttpwithrespfromb")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# InitiateFlowToHTTPWithResponse HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
 
            // Activity.Current.AddBaggage("MyCustomCorrIdInBAGGAGE", "baggagevalue");

            DumpActivity(Activity.Current, log);
            
            // send to the second function app, assuming this runs locally on port 7072
            // TODO: fix this, move this into app settings, populated by ARM template
            await _httpClient.GetAsync($"{this._functionAppBUrl}/api/http");
            log.LogInformation($"Called function B at: {this._functionAppBUrl}/api/http");

            return (ActionResult)new OkObjectResult($"");
        }

        [FunctionName("InitiateFlowToLogicAppA")]
        public async Task<IActionResult> RunCallToLogicAppA(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "calllogicappa")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# InitiateFlowToLogicAppA HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
 
            Activity.Current.AddBaggage("MyCustomCorrId", "baggagevalue");

            DumpActivity(Activity.Current, log);
            
            // call Logic App A
            await _httpClient.PostAsync(_logicAppAUrl, null);

            return (ActionResult)new OkObjectResult($"");
        }

        [FunctionName("InitiateFlowToLogicAppAWithBaggage")]
        public async Task<IActionResult> RunCallToLogicAppAWithBaggage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "calllogicappa/{submissionId}")] HttpRequest req,
            string submissionId,
            ILogger log)
        {
            log.LogInformation("C# InitiateFlowToLogicAppAWithBaggage HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
 
            Activity.Current.AddTag("MyCustomCorrId", submissionId);
            Activity.Current.AddBaggage("MyCustomCorrId", submissionId);
            Activity.Current.AddBaggage("SomethingElse", "test123");

            DumpActivity(Activity.Current, log);
            
            // call Logic App A - pass on the body content of what came in
            var objectContent = new StringContent(requestBody);
            objectContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            await _httpClient.PostAsync(_logicAppAUrl, objectContent);

            return (ActionResult)new OkObjectResult($"");
        }

        [FunctionName("ConsunmeEventGridEvent")]
        public void EventGridTest([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());

            DumpActivity(Activity.Current, log);
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
    }
}
