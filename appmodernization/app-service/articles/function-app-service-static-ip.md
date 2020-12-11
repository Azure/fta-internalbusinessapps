## Abstract

Demonstrate how to assign a dedicated static out bound IP to a Function App hosted on Standard or Premimium App Service Plan and assign one dedicated static IP. At the moment this 
can done only when a Function App is delployed/hosted on an ASE. This is on the roadmap to be made avaialble as a feature in the Premimium Plan. Rapid scale out is often of importance to customers 
when hosting their Functions and at the moment on ASE V2, scaling out can take up to 45 minutes, which become a blocker for many customers. 
Thus they want to use App Service Plan instead. 

With App Services Plan, Function Apps or Web Apps get a list of possible outbound IP addresses. If they want external systems to consume their functions, they need to share the list of outbound IPs assigned.
At the moment, there is not a way to have just one dedicated static IP. Also, it should be noted that everytime a change in App Service Plan occurs from scale up and down perpsective,
the list of outbound IPs assigned changes.

If custoerms want to expose with security in place their Function Apps to other 3rd Party Vendors to consume, the third party consumers often require a single dedicated static IP,
that can be shared with them for them to whitelist. 

This pattern created below solves for this use case. 

## Steps
