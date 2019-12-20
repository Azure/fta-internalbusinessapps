[CmdletBinding()]
Param(
  [Parameter(Mandatory=$True)]
  [string]$RG,

  [Parameter(Mandatory=$False)]
  [string]$Location="westeurope",

  [Parameter(Mandatory=$False)]
  [string]$ResourcesPrefix="aicorr4"
)

#az login

## function App SA

## build, publish, zip and deploy functions via the cli - function name hard coded here and in the function nested ARM deploy template
dotnet publish "..\src\functionAppA\FunctionAppA.csproj"
$compress = @{
  Path= "..\src\FunctionAppA\bin\Debug\netcoreapp2.2\publish\*"
  CompressionLevel = "Fastest"
  DestinationPath = "FunctionAppA.zip"
}
Compress-Archive @compress -Force
$funcAname = "$ResourcesPrefix" + "-fn-a"
az functionapp deployment source config-zip  -g $RG -n $funcAname --src "FunctionAppA.zip"

write-host "published function app A source" -ForegroundColor Green

## function App B

dotnet publish "..\src\functionAppB\FunctionAppB.csproj"
$compress = @{
  Path= "..\src\FunctionAppB\bin\Debug\netcoreapp2.2\publish\*"
  CompressionLevel = "Fastest"
  DestinationPath = "FunctionAppB.zip"
}
Compress-Archive @compress -Force
$funcBname = $ResourcesPrefix + "-fn-b"
az functionapp deployment source config-zip  -g $RG -n $funcBname --src "FunctionAppB.zip"

write-host "published function app B source" -ForegroundColor Green

# Web App
dotnet publish "..\src\eg-webhook-api\eg-webhook-api.csproj"
$compress = @{
  Path= "..\src\eg-webhook-api\bin\Debug\netcoreapp3.0\publish\*"
  CompressionLevel = "Fastest"
  DestinationPath = "eg-webhook-api.zip"
}
Compress-Archive @compress -Force
$webappname="$ResourcesPrefix-egsubscriber-webapp"
az webapp deployment source config-zip  -g $RG -n $webappname --src "eg-webhook-api.zip"

write-host "published web app source" -ForegroundColor Green

# Event Grid

## create a subscription for the demo event grid and functionA => ConsunmeEventGridEvent function
##get a key
$topicName = $ResourcesPrefix + "-egtopic-cs"
$egsubname = "eg-cs-sub"
$azsubscription= az account show | ConvertFrom-Json
$SubId = $azsubscription.Id

# check cli extension
$aegExt = az extension show -n eventgrid | ConvertFrom-Json
if ( $aegExt ){
  write-host "removing old cli eventgrid extension"
  az extension remove -n eventgrid
}
write-host "adding cli eventgrid extension"
az extension add -n eventgrid
# create topic (see note below)

$topicDetails = az eventgrid topic show --name $topicName --resource-group $RG --subscription $SubId | ConvertFrom-Json

if (! $topicDetails){
  write-host "no topic found"
  az eventgrid topic create -n $topicName -l $Location -g $RG --input-schema cloudeventschemav1_0
}

$checkExistingSub = az eventgrid event-subscription show --name $egsubname --source-resource-id /subscriptions/$SubId/resourceGroups/$RG/providers/Microsoft.EventGrid/topics/$topicName | ConvertFrom-Json
if ( $checkExistingSub.name -eq $egsubname ) {
    Write-Host "creating EG subscription " -ForegroundColor Yellow

} else {
    Write-Host "creating EG subscription "
    # if exchanging web app for a func endpoint dont forget to escape "&" (query string) with "^^^&"
    $endpoint="https://$webappname.azurewebsites.net/api/egwebhook"

    # IMPORTANT NOTE: Using Cloud Schema requires an AZ extension (for BOTH topic AND subscription)
    # https://docs.microsoft.com/en-us/azure/event-grid/cloudevents-schema
    # handshake is also different , uses OPTIONS verb as decribed here: https://github.com/cloudevents/spec/blob/v1.0/http-webhook.md#4-abuse-protection

    #setup for a web app custom webhook endpoint (cloud event schema requires custom endpoint)
    az eventgrid event-subscription create --name $egsubname --source-resource-id "/subscriptions/$SubId/resourceGroups/$RG/providers/Microsoft.EventGrid/topics/$topicName" --endpoint $endpoint  --event-delivery-schema cloudeventschemav1_0
}

# write config for console app
# e.g. {   "aegTopicUrl": "https://begim-egtopic-cs.westeurope-1.eventgrid.azure.net/api/events",   "aegTopicKey":"7dCVcy0te2hoXEb4lAc2UbUhEVL6RKgQPVqzEdDFqTA=" }
$key= az eventgrid topic key list --name $topicName -g $RG --query "key1" --output tsv 
$endpoint=    az eventgrid topic show --name $topicName -g $RG --query "endpoint" --output tsv
az extension add --name application-insights
$iKey = (az monitor app-insights component show --app "$ResourcesPrefix-egsubscriber-webapp-ai" -g $RG | ConvertFrom-Json).instrumentationKey
$json="{ ""aegTopicUrl"": ""$endpoint"", ""aegTopicKey"": ""$key"" , ""iKey"": ""$iKey""}"
$json | Out-File -FilePath ../src/egconsole/appsettings.json -Force