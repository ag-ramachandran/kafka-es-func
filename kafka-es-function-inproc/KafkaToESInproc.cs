using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Azure.WebJobs.Extensions.EventHubs;
using Microsoft.Extensions.Logging;
using Avro.Generic;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        public async Task Run(
            [KafkaTrigger("%CONNECT_BOOTSTRAP_SERVERS%","%CONNECT_TOPIC%",
                    Protocol = BrokerProtocol.Ssl,
                    SslKeyLocation = "%CONNECT_SSL_KEYSTORE_LOCATION%",
                    SslKeyPassword = "%CONNECT_SSL_KEYSTORE_PASSWORD%",
                    SslCaLocation = "%CONNECT_SSL_CA_LOCATION%",
                    SslCertificateLocation = "%CONNECT_SSL_TRUSTSTORE_LOCATION%",
                    SchemaRegistryUrl = "%CONNECT_SCHEMA_REGISTRY_URL%",                    
                    ConsumerGroup = "%CONNECT_GROUP_ID%"
                    )] KafkaEventData<GenericRecord>[] events, 
                    [EventHub("%EVENTSTREAM_NAME%", Connection = "ES_CONNECTION")]IAsyncCollector<EventData> outputEvents,
                    ILogger log)
        {
            foreach (KafkaEventData<GenericRecord> eventData in events)
            {
                var valueRecord = eventData.Value;
                log.LogInformation($"Key: {eventData.Key}, Value: {valueRecord.ToString()}");
                await outputEvents.AddAsync(new EventData(GenericToJson(valueRecord)));
            }
        }
        // From : https://github.com/Azure/azure-functions-kafka-extension/blob/dev/samples/dotnet/KafkaFunctionSample/AvroGenericTriggers.cs
        private static byte[] GenericToJson(GenericRecord record)
        {
            var props = new Dictionary<string, object>();
            foreach (var field in record.Schema.Fields)
            {
                if (record.TryGetValue(field.Name, out var value))
                    props[field.Name] = value;
            }
            return System.Text.Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(props));
        }        
    }
}
