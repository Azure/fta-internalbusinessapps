# POC Walkthrough - Heptio Ark: Setting up Backup and Restore for a Kubernetes Cluster

## Introduction
In this walkthrough we will utilize Heptio Ark to configure backups of Kubernetes objects, such as Deployments and Pods, for the ability to restore them in case of a Kubernetes cluster failure. Heptio Ark assists with the BCDR strategy of customers moving critical workloads to a container infrastructure using the Kubernetes orchestration.

This walkthrough leverages and references Heptio's own Ark documentation for Azure hosted on GitHub [here](https://heptio.github.io/ark/v0.8.1/azure-config), to help facilitate a POC. The walkthrough will also build on this with other Azure services.

## Prerequisites
This POC will utilize the Azure CLI to make the experience as similar as possible whether you are using a Windows or Linux system. For Windows 10 systems, the Microsoft Windows Subsystem for Linux (WSL) will need to be installed. 
* [Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install-win10) ( Only if using Windows 10 )
* [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) ( If using the Windows Subsystem for Linux, please follow the installation instructions for the Debian/Ubuntu version located [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-apt?view=azure-cli-latest) )
* An existing Kubernetes cluster running in Azure. You can find the instruction for setting up the AKS managed service [here](https://github.com/Azure/fta-internalbusinessapps/blob/master/appmodernization/containers/articles/acs-aks-managed-deployment.md)
* Ensure that you have the resource group name that the Kubernetes cluster is in.

## Walkthrough
1. In the Linux or WSL terminal, log into Azure using the **az login** command.

   > Note: The **az login** command will provide a code for you to enter at the [microsoft.com/devicelogin](https://microsoft.com/devicelogin) address. Once the code has been entered and accepted, the terminal will be authenticated to your Azure account.
2. Create an Azure resource group for the storage account Heptio Ark will use. 
   ```
      AZURE_BACKUP_RESOURCE_GROUP=K8ArkBackups
      az group create -name $AZURE_BACKUP_RESOURCE_GROUP --location EastUS
   ```
   > Note: If you are unsure of the Azure region name needed for the location parameter you can use **az account list-locations --output table** to view the Azure region names.
3. Create the Azure storage account Heptio Ark will use. 
   ```
      AZURE_STORAGE_ACCOUNT_ID="ark`cat /proc/sys/kernel/random/uuid | cut -d '-' -f5`"
      az storage account create \
    --name $AZURE_STORAGE_ACCOUNT_ID \
    --resource-group $AZURE_BACKUP_RESOURCE_GROUP \
    --sku Standard_GRS \
    --encryption-services blob \
    --https-only true \
    --kind BlobStorage \
    --access-tier Hot
    
   ```
4. Create the blob container for the storage account. 
     > Note: The default name is "ark". If you choose a different name for the blob container, you will need to update the "backupStorageProvider" value in the Ark config file.
   ```
      az storage container create -n ark --public-access off --account-name $AZURE_STORAGE_ACCOUNT_ID
      
   ```
5. Get the storage key for the storage account.
   ```
      AZURE_STORAGE_KEY=`az storage account keys list \
    --account-name $AZURE_STORAGE_ACCOUNT_ID \
    --resource-group $AZURE_BACKUP_RESOURCE_GROUP \
    --query [0].value \
    -o tsv`
    
   ```

5. Run **acs-deploy** with the following arguments:
   ```
   $ ./acs-engine deploy --subscription-id "<your subscription GUID>" \
     --resource-group "<your resource group name>" --location "<your resource group region>" \
     --dns-prefix "<your k8 cluster name>" --auto-suffix \
     --api-model ./kubernetes.json
   ```
      > Note: You may be asked to authenticate your terminal session again back to Azure when running the deployment.

    The acs-engine deploy will automatically create several configuration files and set up several Azure services for hosting the Kubernetes cluster. Key things created are:
    * Azure Service Principle and App ID
    * SSH key to connect to the Kubernetes cluster
    
      > Note: Please make note of the Service Principle and App ID that will be output in the terminial. We will use these IDs for setting up authentication for the Azure Container Registry (ACR) in a later POC walk-through. You can also find the IDs by using **az ad sp list --display-name \"<your k8 cluster name\>"**
    
    The deployment is fairly quick and you should get a succeeded message when completed. 
    ![Screenshot](images/acs-engine-deploy-k8-iaas/acs-engine-deploy-terminal.png)
    
    You can verify that all the resources have been created by running the resource list command on the resource group.
    
    **az resource list --resource-group \"<resource group name\>" --output table**
    
    ![Screenshot](images/acs-engine-deploy-k8-iaas/acs-engine-list-resources-in-rg.png)
    
6. Connect to the Kubernetes cluster:
   
   1. Once the deployment is complete, navigate to the **_output** directory of your kubernetes deployment where you will see several files and a kubeconfig directory.
      ![Screenshot](images/acs-engine-deploy-k8-iaas/acs-engine-deploy-output.png)
   2. View your Kubernetes configuration. The following command will output your Kubernetes endpoints that you will use to connect to the Kubernetes cluster.
      ```
      $ KUBECONFIG=$HOME/acs-engine/acs-engine-v0.14.6-linux-amd64/_output/pgibson-acs-engine-poc1-5ac5abba/kubeconfig/kubeconfig.eastus2.json kubectl cluster-info
      ```
      
      ![Screenshot](images/acs-engine-deploy-k8-iaas/acs-engine-kubectl-config-display.png)
      
    3. Capture the Kubernetes master FQDN endpoint to SSH. The ACS Engine has created the SSH key that will be used to connect. The SSH key is located in the directory name of your Kubernetes cluster in the **_output** directory and has the name **azureuser_rsa**. Run the following command to connect to the Kubernetes master.
        ```
        ssh -i ./azureuser_rsa azureuser@pgibson-acs-engine-poc1-5ac5abba.eastus2.cloudapp.azure.com 
        ```
      
       ![Screenshot](images/acs-engine-deploy-k8-iaas/acs-engine-ssh-k8-connect.png)

7. Now that you are connected to your Kubernetes cluster, you can begin to issue kubectl commands to look over the cluster such as **kubectl get nodes**, as well as start testing the deployment of containerized applications.  



   


