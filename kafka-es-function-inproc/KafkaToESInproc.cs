using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
using Avro.Generic;

namespace KafkaToEventStreamInProcFunctions
{
    public class KafkaToESInproc
    {
        // KafkaTrigger sample 
        // Consume the message from "topic" on the LocalBroker.
        // Add `BrokerList` and `KafkaPassword` to the local.settings.json
        // For EventHubs
        // "BrokerList": "{EVENT_HUBS_NAMESPACE}.servicebus.windows.net:9093"
        // "KafkaPassword":"{EVENT_HUBS_CONNECTION_STRING}
        [FunctionName("KafkaToESInproc")]
        public void Run(
            [KafkaTrigger("%CONNECT_BOOTSTRAP_SERVERS%","%CONNECT_TOPIC%",
                    Protocol = BrokerProtocol.Ssl,
                    SslKeyLocation = "%CONNECT_SSL_KEYSTORE_LOCATION%",
                    SslKeyPassword = "%CONNECT_SSL_KEYSTORE_PASSWORD%",
                    SslCaLocation = "%CONNECT_SSL_CA_LOCATION%",
                    SslCertificateLocation = "%CONNECT_SSL_TRUSTSTORE_LOCATION%",
                    SchemaRegistryUrl = "%CONNECT_SCHEMA_REGISTRY_URL%",                    
                    ConsumerGroup = "%CONNECT_GROUP_ID%"
                    )] KafkaEventData<GenericRecord>[] events, ILogger log)
        {
            foreach (KafkaEventData<GenericRecord> eventData in events)
            {
                log.LogInformation($"C# Kafka trigger function processed a message: {eventData.Value}");
            }
        }
    }
}
