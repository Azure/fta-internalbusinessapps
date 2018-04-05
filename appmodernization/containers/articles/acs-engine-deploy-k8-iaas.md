# POC Walkthrough - ACS Engine: Deploy a Kubernetes IaaS Infrastructure

## Introduction
In this walkthrough, we will utilize the Microsoft Azure Container Service Engine to build a IaaS Docker cluster infrastructure using the Kubernetes orchestration. The ACS Engine is an option to customers that have architectural and/or infrastructure requirements that are not currently supported by the managed Azure AKS service. The ACS Engine gives customers a way to flexibly customize and quickly provision the IaaS infrastructure needed.

This walkthrough is a streamlined compilation of the Microsoft Azure Container Service Engine documentation hosted on GitHub [here](https://github.com/Azure/acs-engine), to help facilitate a POC.

## Prerequisites
This POC will utilize the Azure CLI to make the experience as similar as possible whether you are using a Windows or Linux system. For Windows 10 systems, the Microsoft Windows Subsystem for Linux (WSL) will need to installed. WSL will provide native tools, such as SSH support, and will limit the use of requiring additional tools to interact with the Docker/Kubernetes cluster.
* [Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install-win10) ( Only if using Windows 10 )
* [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) ( If using the Windows Subsystem for Linux, please follow the installation instructions for the Debian/Ubuntu version located [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-apt?view=azure-cli-latest) )
* [ACS-Engine Binary](https://github.com/Azure/acs-engine/releases/tag/v0.14.6) for your operating system
  * If using the Windows Subsystem for Linux, please do the following in the WSL terminal:
    1. mkdir acs-engine
    2. cd acs-engine
    3. wget https://github.com/Azure/acs-engine/releases/download/v0.14.6/acs-engine-v0.14.6-linux-amd64.zip
    4. uzip acs-engine-v0.14.6-linux-amd64.zip

## Walkthrough
1. In the Linux or WSL terminal, log into Azure using the **az login** command.

   > Note: The **az login** command will provide a code for you to enter at the [microsoft.com/devicelogin](https://microsoft.com/devicelogin) address. Once the code has been entered and accepted, the terminal will be authenticated to your Azure account.
2. Find and set the Azure subcription you will be deploying the Kubernetes cluster to:
   - List out all subcriptions you have access to - **az account list --output table**
   - Once you have identified the subcription name that you will utilize for the Kubernetes cluster, set the terminal session to the Azure account subcription - **az account set --subscription \"<subscription name\>"**
   
     > Note: Also make note of the subcription ID of the subcription name used, as you will need that information when deploying using the acs-engine.
3. Create an Azure resource group to deploy the Kubernetes cluster to - Ex. **az group create --name \"<resource group name\>" --location \"<Azure region\>"**

   > Note: If you are unsure of the Azure region name needed for the location parameter you can use **az account list-locations --output table** to view the Azure region names.
4. Download the Kubernetes api model template json file - wget https://raw.githubusercontent.com/Azure/acs-engine/master/examples/kubernetes.json

   > Note: By default the configuration file will create 1 master node and 3 worker nodes. The master nodes will be deployed in their own availability set, as well as the worker nodes will be deployed in their own availability set. If you would like to change the number of nodes being deployed, make edits to the count property in the configuration file.
5. Run **acs-deploy** with the following arguments:
   ```
   $ ./acs-engine deploy --subscription-id "<your subscription GUID>" \
     --resource-group "<your resource group name>" --location "<your resource group region>" \
     --dns-prefix "<your k8 cluster name>" --auto-suffix \
     --api-model ./kubernetes.json
   ```
      > Note: You may be asked to authenticate your terminal session again back to Azure when running the deployment.

    The deployment is fairly quick and you should get a succeeded message when completed. 
    ![Screenshot](images/acs-engine-deploy-k8-iaas/acs-engine-deploy-terminal.png)
    
    You can verify that all the resources have been created by running the resource list command on the resource group.
    
    **az resource list --resource-group \"<resource group name\>" --output table**
    
    ![Screenshot](images/acs-engine-deploy-k8-iaas/acs-engine-list-resources-in-rg.png)
    
6. Connect to the Kubernetes cluster:





   

