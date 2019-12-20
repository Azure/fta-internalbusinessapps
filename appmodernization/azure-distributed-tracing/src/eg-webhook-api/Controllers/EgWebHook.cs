using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using eg_webhook_api;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace eg_webhook_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EgWebHookController : ControllerBase
    {
        private readonly ILogger<EgWebHookController> _logger;
        private readonly TelemetryClient _telemClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        private bool EventTypeSubcriptionValidation
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
            "SubscriptionValidation";

        private bool EventTypeNotification
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
            "Notification";

         private string AEGSubscriptionName
            => HttpContext.Request.Headers["aeg-subscription-name"].FirstOrDefault() ;

        // correct way to obtain a AI telem client in ASP NET CORE is via the DI 
        public EgWebHookController(ILogger<EgWebHookController> logger, TelemetryClient telemClient, IHttpClientFactory httpClient, IConfiguration config)
        {
            _config = config;
            _logger = logger;
            _telemClient = telemClient;
            _httpClientFactory = httpClient;
        }

        [HttpOptions]
        public async Task<IActionResult> Options()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                var webhookRequestCallback = HttpContext.Request.Headers["WebHook-Request-Callback"];
                var webhookRequestRate = HttpContext.Request.Headers["WebHook-Request-Rate"];
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);

                _logger.LogInformation("Options called");
            }

            return Ok();
        }

        // send an event grid message?
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("GET called");
            return Ok("hello world");
        }

        private async void LogHeaders(){

            if ( HttpContext.Request.Headers.Count == 0){
                _logger.LogInformation($"NO Headers found");
                return;
            }
       
            foreach (var header in HttpContext.Request.Headers) {
  
                    _logger.LogInformation($"HEADERS: Header {header.Key} : {header.Value}");
                }
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            LogHeaders();

            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var jsonContent = await reader.ReadToEndAsync();

                // Check the event type.
                // Return the validation code if it's 
                // a subscription validation request. 
                if (EventTypeSubcriptionValidation)
                {
                    return await HandleValidation(jsonContent);
                }
                else if (IsCloudEvent(jsonContent, out CloudEvent<dynamic> cloudEvent))
                {
                    return await HandleCloudEvent(cloudEvent);
                }

                return BadRequest();                
            }
        }

        private async Task<JsonResult> HandleValidation(string jsonContent)
        {
            var gridEvent =
                JsonSerializer.Deserialize<List<GridEvent<Dictionary<string, string>>>>(jsonContent)
                    .First();

            // Retrieve the validation code and echo back.
            var validationCode = gridEvent.Data["validationCode"];
            return new JsonResult(new
            {
                validationResponse = validationCode
            });
        }

        private async Task<IActionResult> HandleCloudEvent(CloudEvent<dynamic> details)
        {
            if (null == details){

                _logger.LogInformation("no cloud event | null");
                return BadRequest();
            }
            var sb = new StringBuilder();
            sb.AppendLine("cloud event received");
            sb.AppendLine(details.Id);
            sb.AppendLine(details.Type);
            sb.AppendLine(details.Subject);
            sb.AppendLine(details.Time);
 
            // we received cloud event - need to create request telemetry to record the start of new activity with the incoming traceparent as our new parent activity
            // var r = new RequestTelemetry(
            //     name: $"HandleCloudEvent {AEGSubscriptionName}",
            //     startTime: DateTimeOffset.Now,
            //     duration: TimeSpan.FromSeconds(1),
            //     responseCode: "200",
            //     success: true)
            // {
            //     Source = "" //no source specified
            // };
            //_telemClient.TrackRequest(r);

            // r.Context.Operation.Id = details.TraceParent.Split('-')[1];             // initiate the logical operation ID (trace id)
            // r.Context.Operation.ParentId = details.TraceParent.Split('-')[2];       // this is the first span in a trace

            // Start Operation and add baggage from incoming event
            var eventGridReceivalActivity = new Activity("EventGridHandling");
            var baggage = parseBaggage(details.TraceState);

            foreach(var item in baggage){
                eventGridReceivalActivity.AddBaggage(item.Key, item.Value);
            }
            
            eventGridReceivalActivity.SetParentId(details.TraceParent);
            //eventGridReceivalActivity.SetParentId(details.TraceParent.Split('-')[2]);
            //eventGridReceivalActivity.=  details.TraceParent.Split('-')[1];
            
            //var op = _telemClient.StartOperation<RequestTelemetry>($"HandleCloudEvent {AEGSubscriptionName}", details.TraceParent.Split('-')[1], details.TraceParent.Split('-')[2]);
            var op = _telemClient.StartOperation<RequestTelemetry>(eventGridReceivalActivity);

            _logger.LogInformation(sb.ToString());
            // TODO: do some stuff in between to see if correlations are being passed on

            // Stop Operation
            _telemClient.StopOperation(op);

            return Ok();
        }

        private List<KeyValuePair<string, string>> parseBaggage(string baggage){
            var splittedBaggage = baggage.Split(',');
            var parsedBaggage = new List<KeyValuePair<string, string>>();

            foreach(string item in splittedBaggage){
                var kvp = item.Split('=');
                if (kvp.Length == 2){
                    parsedBaggage.Add(new KeyValuePair<string,string>(kvp[0], kvp[1]));
                }
            }

            return parsedBaggage;
        }

        private bool IsCloudEvent(string jsonContent, out CloudEvent<dynamic> cloudEvent)
        {
            cloudEvent=null;

            try
            {
                // Attempt to read one JSON object. 
                var details = JsonSerializer.Deserialize<CloudEvent<dynamic>>(jsonContent);

                // Check for the spec version property.
                var version = details.SpecVersion;
                if (!string.IsNullOrEmpty(version)) {
                    cloudEvent=details;
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }


        private void AddAnyActivityBaggage(Activity curActivity,string currentBaggage){
            if (string.IsNullOrWhiteSpace(currentBaggage)){
                return;
            }
            var baggageList=currentBaggage.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach(var baggageItem in baggageList){
                if ( baggageItem.Contains("=")){
                    var tmp = baggageItem.Split("=");
                    curActivity.AddBaggage(tmp[0],tmp[1]);
                }
            }

        }
    }
}
