 # POC Scenario ACS Engine: Deploy a Kubernetes IaaS Infrastructure

## Introduction
In this walkthrough, we will utilize the Microsoft Azure Container Service Engine to build a IaaS Docker cluster infrastructure using the Kubernetes orchestration. The ACS Engine is an option to customers that have architectural and/or infrastructure requirements that are not currently supported by the managed Azure AKS service. The ACS Engine gives customers a way to flexibly customize and quickly provision the IaaS infrastructure needed.

This walkthrough is a streamlined compilation of the Microsoft Azure Container Service Engine documentation hosted on GitHub [here](https://github.com/Azure/acs-engine), to help facilitate a POC.

## Prerequisites
This POC will utilize the Azure CLI to make the experience as similar as possible whether you are using a Windows or Linux system. For Windows 10 systems, the Microsoft Windows Subsystem for Linux (WSL) will need to installed. WSL has native SSH support and limits the use of needing additional tools such as PuTTY.
* [Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install-win10) (Only if using Windows 10)
* [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest)
* [ACS-Engine Binary](https://github.com/Azure/acs-engine/releases/tag/v0.14.6) for your operating system
