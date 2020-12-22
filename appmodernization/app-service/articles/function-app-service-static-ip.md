# Abstract

Demonstrate how to assign a dedicated static out bound IP to a Function App hosted on Standard or Premimium App Service Plan and assign one dedicated static IP. At the moment this 
can done only when a Function App is delployed/hosted on an ASE. This is on the roadmap to be made avaialble as a feature in the Premimium Plan. Rapid scale out is often of importance to customers 
when hosting their Functions and at the moment on ASE V2, scaling out can take up to 45 minutes, which become a blocker for many customers. 
Thus they want to use App Service Plan instead. 

With App Services Plan, Function Apps or Web Apps get a list of possible outbound IP addresses. If they want external systems to consume their functions, they need to share the list of outbound IPs assigned.
At the moment, there is not a way to have just one dedicated static IP. Also, it should be noted that everytime a change in App Service Plan occurs from scale up and down perpsective,
the list of outbound IPs assigned changes.

If custoerms want to expose with security in place their Function Apps to other 3rd Party Vendors to consume, the third party consumers often require a single dedicated static IP,
that can be shared with them for them to whitelist. 

This pattern created below solves for this use case.

![Screenshot](media/app-service-function-apps/function-app-vnet-integration-dmz-reference-architectures.png)

## Create an HTTP Trigger Function
- Create an HTTP Triggger C# function with a nmae of your liking.
- Use an App Service Hosting Plan SKU or Standard or Premimium, both support VNET integration. You may use Standard plan for this set up, as it is cheaper.
- Replace the out of the box code for the HTTP Trigger with the folowing code.

```` C#
#r "Newtonsoft.Json"
#r "System.Text.Json"
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.Json.Serialization;
//using System.Exception;
public class Repository
{
[JsonPropertyName("name")]
public string Name { get; set; }
[JsonPropertyName("description")]
public string Description { get; set; }
[JsonPropertyName("html_url")]
public Uri GitHubHomeUrl { get; set; }
[JsonPropertyName("homepage")]
public Uri Homepage { get; set; }
[JsonPropertyName("watchers")]
public int Watchers { get; set; }
[JsonPropertyName("pushed_at")]
public string JsonDate { get; set; }
public DateTime LastPush =>
DateTime.ParseExact(JsonDate, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
}
public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{ 
log.LogInformation("C# HTTP trigger function processed a request.");
string name = req.Query["name"];
string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
dynamic data = JsonConvert.DeserializeObject(requestBody);
name = name ?? data?.name;
string responseMessage = string.IsNullOrEmpty(name)
? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
: $"Hello, {name}. This HTTP triggered function executed successfully.";
// Testing this...disregard for demo
// ReadTextFile();
log.LogInformation("Performing ProcessRepositories code section...");
var repositories = await ProcessRepositories();
foreach (var repo in repositories)
{
if (repo == null)
{
continue;
}
log.LogInformation($"Repo Name: {repo.Name}");
log.LogInformation($"Repo Description: {repo.Description}");
}
return new OkObjectResult(responseMessage);
}
// Ref URL: https://github.com/dotnet/samples/blob/master/csharp/getting-started/console-webapiclient/Program.cs
private static async Task<List<Repository>> ProcessRepositories()
{
var client = new HttpClient();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json") );
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
var streamTask = client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
var repositories = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Repository>>(await streamTask);
return repositories;
}
// Ref URL - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-from-a-text-file
private void ReadTextFile(ILogger log)
{
try
{
// Example #1
// Read the file as one string.
string text = System.IO.File.ReadAllText(@"\\10.10.2.5\test\testfile.txt");
// Display the file contents to the console. Variable text is a string.
log.LogInformation("Contents of WriteText.txt = {0}", text);
}
catch(Exception ex)
{
log.LogInformation("Exception = {0}", ex.Message);
}
}

````
 - Run the HTTP Trigger and see the Function run successfully

## Configure Function's Application Setting
- Create a Config Variable **WEBSITE_VNET_ROUTE_ALL** in Application settings for the function and set the value to **1**.

![Screenshot](media/app-service-function-apps/Set-website-vnet-route-all-to-1-function-app-configuration.png)

## Create a VNET and required subnets

![Screenshot](media/app-service-function-apps/VNET-with-AzureFirewallSubnet.png)

![Screenshot](media/app-service-function-apps/Subnet-for-VNET-integration-resize.png)

![Screenshot](media/app-service-function-apps/VNET-integrationsubnet-delegate-subnet-to-server-farmpng-resize.png)

- Ensure a subnet **AzureFirewallSubnet** is created, dedicated for the Azure Firewall.
- In the subnet for VNET integratin, set the **Delegate subnet to a service** to **Microsoft.Web/serverFarms** for 


