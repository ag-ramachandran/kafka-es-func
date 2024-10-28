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
        [FixedDelayRetry(5, "00:00:10")]
        [EventHubOutput("dest", Connection = "EventHubConnection")]
        public void Run(
        [KafkaTrigger("BrokerList",
                    "topic",
                    Username = "ConfluentCloudUserName",
                    Password = "ConfluentCloudPassword",
                    Protocol = BrokerProtocol.Ssl,
                    SslKeyLocation = "",
                    SslKeyPassword = "",
                    SslCaLocation = "",
                    SslCertificateLocation = "",
                    AuthenticationMode = BrokerAuthenticationMode.,
                    ConsumerGroup = "$Default")] string eventData, FunctionContext context)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
