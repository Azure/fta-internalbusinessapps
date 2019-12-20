
using System.Text.Json.Serialization;
using System;

namespace eg_webhook_api{


  public class CloudEvent<T> where T : class
    {

        // Spec https://github.com/cloudevents/spec/blob/v1.0/spec.md

        [JsonPropertyName("specversion")]
        public string SpecVersion { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("datacontenttype")]
        public string DataContentType { get; set; }


        // trace extension properties https://github.com/cloudevents/spec/blob/v1.0/extensions/distributed-tracing.md

        [JsonPropertyName("traceparent")]
        public string TraceParent { get; set; }

        [JsonPropertyName("tracestate")]
        public string TraceState { get; set; }

    }
}