Param(
    [Parameter(Mandatory=$true)]
    [string]$subscriptionName,

    [Parameter(Mandatory=$false)]
    [string]$location = "westeurope"
)
# VARIABLES
$resourcegroupname = "ContosoExpensesARM$(Get-Random)"
$storageaccountname = "sqlimport$(Get-Random)"
$storagecontainername = "importcontainer"
$bacpacfilename = "ContosoExpensesDB.bacpac"

# Login in Azure
if($subscriptionName){
    Login-AzureRmAccount -Subscription $subscriptionName
}
else{
    Login-AzureRmAccount
}
Write-host "Successfully Signed-in" -f Cyan

# Create a resource group
$resourcegroup = New-AzureRmResourceGroup -Name $resourcegroupname -Location $location
Write-host "Created resource group" -f Cyan

# Create a storage account 
$storageaccount = New-AzureRmStorageAccount -ResourceGroupName $resourcegroupname `
    -AccountName $storageaccountname `
    -Location $location `
    -Type "Standard_LRS"

Write-host "Created storage account" -f Cyan

# Create a storage container
$storagecontainer = New-AzureStorageContainer -Name $storagecontainername `
-Context $(New-AzureStorageContext -StorageAccountName $storageaccountname `
    -StorageAccountKey $(Get-AzureRmStorageAccountKey -ResourceGroupName $resourcegroupname -StorageAccountName $storageaccountname).Value[0])

Write-host "Created storage container" -f Cyan

# Download sample database from Github

Invoke-WebRequest -Uri "https://github.com/Azure/fta-internalbusinessapps/raw/master/appmodernization/app-service/src/Contoso.Expenses.ARM/database/ContosoExpensesDB.bacpac" -OutFile $bacpacfilename

# Upload sample database into storage container
$storageaccountkey = $(Get-AzureRmStorageAccountKey -ResourceGroupName $resourcegroupname -StorageAccountName $storageaccountname).Value[0]
Set-AzureStorageBlobContent -Container $storagecontainername `
-File $bacpacfilename `
-Context $(New-AzureStorageContext -StorageAccountName $storageaccountname `
    -StorageAccountKey $storageaccountkey)

Write-host "Successfully uploaded .bacpac to storage account" -f Cyan

# Delete temporary .bacpac file
Remove-Item .\ContosoExpensesDB.bacpac

# Output storage account name and key
Write-Host ""
Write-Host " ----                  OUTPUT " -f Yellow
Write-Host " - RESOURCE GROUP NAME: " -NoNewline; Write-Host "$resourcegroupname" -f Red
Write-Host " - STORAGE ACCOUNT NAME: " -NoNewline; Write-Host "$storageaccountname" -f Red
Write-Host " - STORAGE ACCOUNT KEY: " -NoNewline; Write-Host "$storageaccountkey" -f Red
Write-Host " - KEEP THIS INFORMATION AS IT WILL BE REQUIRED FOR DEPLOYING - " -f Yellow
