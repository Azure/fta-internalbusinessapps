[CmdletBinding()]
Param(
  [Parameter(Mandatory=$True)]
  [string]$RG,

  [Parameter(Mandatory=$False)]
  [string]$Location="westeurope",

  [Parameter(Mandatory=$False)]
  [string]$ResourcesPrefix="aicorr2"
)

#az login


$StorageAccountNameForNestedTemplates = "$($ResourcesPrefix)storacct"
$NestedTemplatesStorageContainerName = "nestedtemplates"

# create RG
$rgExists = az group exists -n $RG
if ( $rgExists -eq $False ){
    Write-Output "Creating RG"
  az group create -n $RG -l $Location
}


# create storage account in RG to deploy nested templates towards
$storAccountStatus=az storage account check-name --name $StorageAccountNameForNestedTemplates | ConvertFrom-Json
if ( $storAccountStatus.nameAvailable -eq $true )
{
    Write-Output "Creating Storage Account for storing nested arm templates"
    az storage account create -g $RG -n $StorageAccountNameForNestedTemplates -l $Location --sku Standard_LRS
} elseif ( $storAccountStatus.reason -eq 'AlreadyExists' ) 
{
    write-host "storage account already exists, moving on" -ForegroundColor Yellow
} else {
    write-host "something went wrong, storage account name $StorageAccountNameForNestedTemplates unavailable?" -ForegroundColor Red
    exit
}

#fixup workbook references
$azsubscription= az account show | ConvertFrom-Json
$SubId = $azsubscription.Id
$workbookpath = ".\nestedTemplates\workbook.json"
$modifiedworkbookpath = ".\nestedTemplates\workbook-tmp.json"
Copy-Item -Path $workbookpath -Destination $modifiedworkbookpath -Force

#(Get-Content -path $modifiedworkbookpath -Raw) -replace `
#"/subscriptions/651dc44c-5d8e-48da-8cd3-cd79224ac290" ,"/subscriptions/$SubId" | Set-Content -Path $modifiedworkbookpath

#(Get-Content -path $modifiedworkbookpath -Raw) -replace `
#"(resourceGroups)(\/+)(\w+)-(\w+)" ,"resourceGroups/$RG" | Set-Content -Path $modifiedworkbookpath

# (Get-Content -path $modifiedworkbookpath -Raw) -replace `
# "(\/+)(\w+)(-fn-appinsights)" ,"/$resourcesprefix-fn-appinsights" | Set-Content -Path $modifiedworkbookpath
  

# upload nested templates

$containerExists = az storage container exists --account-name $StorageAccountNameForNestedTemplates --name $NestedTemplatesStorageContainerName | ConvertFrom-Json
if ( $containerExists.exists -eq $false ) {
    #remove existing so can upload updated
    write-host "removing existing container" -ForegroundColor Yellow
    az storage container delete --account-name $StorageAccountNameForNestedTemplates --name $NestedTemplatesStorageContainerName
}

Write-Output "Creating storage container for nested templates: '$NestedTemplatesStorageContainerName'"
az storage container create -n $NestedTemplatesStorageContainerName --account-name $StorageAccountNameForNestedTemplates

Write-Output "Uploading nested template to container"
az storage blob upload-batch --account-name $StorageAccountNameForNestedTemplates -d $NestedTemplatesStorageContainerName -s "./nestedTemplates" --pattern "functions.json"
az storage blob upload-batch --account-name $StorageAccountNameForNestedTemplates -d $NestedTemplatesStorageContainerName -s "./nestedTemplates" --pattern "service-bus-queue.json"
az storage blob upload-batch --account-name $StorageAccountNameForNestedTemplates -d $NestedTemplatesStorageContainerName -s "./nestedTemplates" --pattern "workbook-tmp.json"
az storage blob upload-batch --account-name $StorageAccountNameForNestedTemplates -d $NestedTemplatesStorageContainerName -s "./nestedTemplates" --pattern "container-instance.json"
az storage blob upload-batch --account-name $StorageAccountNameForNestedTemplates -d $NestedTemplatesStorageContainerName -s "../src/LogicAppA" --pattern "*.definition.json"
Write-Output "Templates uploaded"

remove-item $modifiedworkbookpath

# create sas token
$SasTokenForNestedTemplates = az storage container generate-sas --account-name $StorageAccountNameForNestedTemplates -n $NestedTemplatesStorageContainerName  --permissions r --expiry (Get-Date).AddMinutes(180).ToString("yyyy-MM-dTH:mZ")
Write-Output "Sas-token for accessing nested templates: $SasTokenForNestedTemplates"

$NestedTemplatesLocation = "https://$StorageAccountNameForNestedTemplates.blob.core.windows.net"

# deploy template
$templateFile = "deploy.json"

az group deployment create -n "appinsights-corrtest-deployment" -g $RG --template-file "$templateFile" --parameters _artifactsLocation=$NestedTemplatesLocation _artifactsLocationSasToken=$SasTokenForNestedTemplates resourcesPrefix=$ResourcesPrefix

write-host "Information: Run deploy-func-code.ps1 to deploy function app code" -ForegroundColor Green