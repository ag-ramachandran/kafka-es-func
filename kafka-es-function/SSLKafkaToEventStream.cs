using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace KafkaToEventStreamIsolatedFunctions
{
    public class SSLKafkaToEventStream
    {
        private readonly ILogger _logger;

        public SSLKafkaToEventStream(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SSLKafkaToEventStream>();
        }

        [Function("SSLKafkaToEventStream")]
        // [FixedDelayRetry(5, "00:00:10")]
        // [EventHubOutput("dest", Connection = "EventHubConnection")]
        public void Run(
        [KafkaTrigger("%CONNECT_BOOTSTRAP_SERVERS%","%CONNECT_TOPIC%",
                    Protocol = BrokerProtocol.Ssl,
                    SslKeyLocation = "%CONNECT_SSL_KEYSTORE_LOCATION%",
                    SslKeyPassword = "%CONNECT_SSL_KEYSTORE_PASSWORD%",
                    SslCaLocation = "%CONNECT_SSL_CA_LOCATION%",
                    SslCertificateLocation = "%CONNECT_SSL_TRUSTSTORE_LOCATION%",
                    IsBatched = false,
                    SchemaRegistryUrl = "%CONNECT_SCHEMA_REGISTRY_URL%",                    
                    ConsumerGroup = "%CONNECT_GROUP_ID%"
                    )] string eventData, FunctionContext context)
        {
            _logger.LogInformation($"Message: {eventData}");
        }
    }
}
