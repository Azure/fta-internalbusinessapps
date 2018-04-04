 # POC Scenario ACS Engine: Deploy a Kubernetes IaaS Infrastructure

## Introduction
In this walkthrough, we will utilize the Microsoft Azure Container Service Engine to build a IaaS Docker cluster infrastructure using the Kubernetes orchestration. The ACS Engine is an option to customers that have architectural and/or infrastructure requirements that are not supported by the managed Azure AKS service. The ACS Engine gives customers a way to flexibly customize and quickly provision the IaaS infrastructure needed.

This walkthrough is a streamlined compilation of the Microsoft Azure Container Service Engine documentation hosted on GitHub [here](https://github.com/Azure/acs-engine), to help facilitate a POC.

## Prerequisites
This POC will utilize the Azure CLI to make the experience as similar as possible whether you are using a Windows or Linux system. For Windows 10 systems, the Microsoft Windows Subsystem for Linux (WSL) will need to installed. WSL has native SSH support and limits the use of needing additional tools such as PuTTY.
