# Setting up the cross component correlation playground

## Deploying the infrastructure

Within the `deployment` directory, execute the `deploy.ps` powershell script, passing parameters as follows:

~~~ps1
.\deploy.ps1 -RG [name-for-resource-group] -ResourcesPrefix [short-prefix-for-resources]
~~~

## Deploying the code on top of the infrastructure

While the `deploy.ps1` provides you with the core Azure resources, a second script is being used to:
- compile functions
- deploy functions on top of the infrastructure
- compile and deploy a web app for receival of event grid events
- setup the event grid subscription for the above

Use it like this:

~~~ps1
.\deploy-func-code.ps1 -RG [name-for-resource-group] -ResourcesPrefix [short-prefix-for-resources]
~~~

## Configuring functions to run local

Please use the Azure Functions tooling for VS Code to download the app settings from both Function A and Function B to your local development machine as a starting point.
