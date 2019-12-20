# Cross Component Correlations using TelemetryClient

As explained in http://apmtips.com/blog/2017/10/18/two-types-of-correlation/ `TelemetryClient` can be used to generate telemetry which App Insights can use to deduct cross component correlations from.  

## How TelemetryClient works

[TelemetryClient](https://github.com/microsoft/ApplicationInsights-dotnet/blob/master/src/Microsoft.ApplicationInsights/TelemetryClient.cs) can be used to track request and dependency telemetry types using calls to `TrackDependency` and ...
Ultimate both these call, call into the more generic public (but hidden) [`public void Track(ITelemetry telemetry)`](https://github.com/microsoft/ApplicationInsights-dotnet/blob/master/src/Microsoft.ApplicationInsights/TelemetryClient.cs#L438).

In the case of a [DependencyTelemetry](https://github.com/microsoft/ApplicationInsights-dotnet/blob/master/src/Microsoft.ApplicationInsights/DataContracts/DependencyTelemetry.cs) item being tracked, the `Target` field can be set to something like `myapi.com | cid-v1:{API_SERVICE_APP_ID}` where the app id comes for example from the response headers of a dependency call. 

But how does that get handled internally to TelemetryClient?

First, the telemetry item is passed onto:

~~~cs
this.configuration.TelemetryProcessorChain.Process(telemetry);
~~~

[`ServerTelemetryChannel`](https://github.com/microsoft/ApplicationInsights-dotnet/blob/master/src/ServerTelemetryChannel/ServerTelemetryChannel.cs) is the one sending data to App Insights over HTTPS.  It takes into account several policies for throttling etc and [makes use of the first ITelemetryProcessor](https://github.com/microsoft/ApplicationInsights-dotnet/blob/master/src/ServerTelemetryChannel/ServerTelemetryChannel.cs#L282) in the chain to send the telemetrydata to.


This passes the telemetry on to the first processor in the chain.  This processor is of type `TelemetryBuffer` which is a ITelemetryProcessor which takes a `TelemetrySerializer` which in turns take a `Transmitter` which in turn takes a set of policies.


When all processors processed the item, a [`TransmissionProcessor`](https://github.com/microsoft/ApplicationInsights-dotnet/blob/master/src/Microsoft.ApplicationInsights/Extensibility/Implementation/TransmissionProcessor.cs) proxies the telemetry into a `TelemetrySink`.

