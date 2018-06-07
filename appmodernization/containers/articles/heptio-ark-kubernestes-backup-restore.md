# POC Walkthrough - Heptio Ark: Setting up Backup and Restore for a Kubernetes Cluster - Part II

## Introduction
In this Part II walkthrough, we will download the Heptio Ark client and create sample Kubernetes objects, such as a Deployment, Pods, and a Service to simulate a disaster event (deletion) of the Kubernetes objects. We will showcase Heptio Ark's ability to backup and restore all of the lost Kubernetes objects.

This walkthrough leverages and references Heptio's own Ark documentation for Azure hosted on GitHub [here](https://heptio.github.io/ark/v0.8.1/index.html), to help facilitate a POC. The walkthrough will also build on this with other Azure services.

## Prerequisites
This POC will utilize the Azure CLI to make the experience as similar as possible whether you are using a Windows or Linux system. For Windows 10 systems, the Microsoft Windows Subsystem for Linux (WSL) will need to be installed. 
* [Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install-win10) ( Only if using Windows 10 )
* [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) ( If using the Windows Subsystem for Linux, please follow the installation instructions for the Debian/Ubuntu version located [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-apt?view=azure-cli-latest) )
* An existing Kubernetes cluster running in Azure. You can find the instruction for setting up the AKS managed service [here](https://github.com/Azure/fta-internalbusinessapps/blob/master/appmodernization/containers/articles/acs-aks-managed-deployment.md)
* Completion of the POC Walkthrough - Heptio Ark: Setting up Backup and Restore for a Kubernetes Cluster - Part I located [here](https://github.com/Azure/fta-internalbusinessapps/blob/master/appmodernization/containers/articles/heptio-ark-kubernestes-backup.md)


## Walkthrough
1. In the Linux or WSL terminal, log into Azure using the **az login** command.

   > Note: The **az login** command will provide a code for you to enter at the [microsoft.com/devicelogin](https://microsoft.com/devicelogin) address. Once the code has been entered and accepted, the terminal will be authenticated to your Azure account.
2. Clone the Heptio Ark repository 
   ```
      git clone https://github.com/heptio/ark      
   ```
3. Create an Azure resource group for the storage account Heptio Ark will use. 
   ```
      AZURE_BACKUP_RESOURCE_GROUP=K8ArkBackups
      az group create --name $AZURE_BACKUP_RESOURCE_GROUP --location EastUS
   ```
   > Note: If you are unsure of the Azure region name needed for the location parameter you can use **az account list-locations --output table** to view the Azure region names.
