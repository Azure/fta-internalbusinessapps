# POC Walkthrough - ACS: Deploy a Managed AKS Preview Service (Portal and CLI)

## Introduction
In this walkthrough, we will deploy the managed AKS Preview service showing both the portal and CLI experience of setting up the managed Azure Container Services (AKS) Kubernetes cluster. The CLI experience will utilize the same prerequisite information needed for the deployment. 

AKS offers customers a managed experience for running Docker clusters using the Kubernetes orchestration. All of the necessary infrastructure needed, will be provisioned and configured automatically. 

This walkthrough is a streamlined compilation of the Microsoft Azure Container Service (AKS) documentation found on the Microsoft docs site [here](https://docs.microsoft.com/en-us/azure/aks/intro-kubernetes), and has more step-by-step details to help facilitate and lead a POC.

## Prerequisites
This POC will utilize the Azure CLI to make the experience as similar as possible whether you are using a Windows or Linux system. For Windows 10 systems, the Microsoft Windows Subsystem for Linux (WSL) will need to be installed. WSL will provide native tools, such as SSH support, and will limit the use of requiring additional tools to interact with the Docker/Kubernetes cluster.
* [Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install-win10) ( Only if using Windows 10 )
* [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) ( If using the Windows Subsystem for Linux, please follow the installation instructions for the Debian/Ubuntu version located [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-apt?view=azure-cli-latest) )
* Install the kubectl binary via curl. Follow the Linux tab on the instructions [here](https://kubernetes.io/docs/tasks/tools/install-kubectl/#install-kubectl-binary-via-curl). 
* The ability to register an application with your Azure AD tenant. Please follow the instructions [here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal#required-permissions) to see if you have the necessary permissions to create Service Principles in your Azure AD.

