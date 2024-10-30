package com.microsoft.fabric.tokafka;

import com.microsoft.entity.wallet.WalletTransaction;
import io.confluent.kafka.serializers.AbstractKafkaSchemaSerDeConfig;
import io.confluent.kafka.serializers.KafkaAvroSerializer;
import net.datafaker.Faker;
import org.apache.kafka.clients.producer.KafkaProducer;
import org.apache.kafka.clients.producer.ProducerConfig;
import org.apache.kafka.clients.producer.ProducerRecord;
import org.apache.kafka.clients.producer.RecordMetadata;
import org.apache.kafka.common.errors.SerializationException;
import org.apache.kafka.common.serialization.StringSerializer;
import org.jetbrains.annotations.NotNull;

import java.io.IOException;
import java.time.LocalDateTime;
import java.util.Properties;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.Future;
import java.util.logging.Level;
import java.util.logging.Logger;

public class ToKafkaApplication {
    private final KafkaProducer<String, WalletTransaction> producer;
    private static final Logger logger = Logger.getLogger(ToKafkaApplication.class.getName());
    private static final Faker FAKE_DATA = new Faker();
    private static final String TOPIC = System.getenv("CONNECT_TOPIC");
    public ToKafkaApplication() {
        Properties props = getBrokerProperties();
        producer = new KafkaProducer<>(props);
        Timer messageTimer = new Timer();
        messageTimer.schedule(new TimerTask() {
            @Override
            public void run() {
                sendMessage();
            }
        }, 30 * 1000);
    }

    public static void main(final String[] args) throws IOException {
        ToKafkaApplication toKafkaApplication = new ToKafkaApplication();
        logger.log(Level.INFO,"Initialized class ["+ getBrokerProperties() +"]");
    }

    @NotNull
    private static Properties getBrokerProperties() {
        String bootStrapServer = System.getenv("PRODUCER_BOOTSTRAP_SERVERS");
        String sslTrustStoreLocation = System.getenv("PRODUCER_SSL_TRUSTSTORE_LOCATION");
        String sslTrustStorePassword = System.getenv("PRODUCER_SSL_TRUSTSTORE_PASSWORD");
        String sslKeystoreLocation = System.getenv("PRODUCER_SSL_KEYSTORE_LOCATION");
        String sslKeystorePassword = System.getenv("PRODUCER_SSL_KEYSTORE_PASSWORD");
        String sslKeyPassword = System.getenv("PRODUCER_KEY_PASSWORD");

        String srUrl = System.getenv("CONNECT_SR_URL");


        Properties props = new Properties();
        props.put("bootstrap.servers", bootStrapServer);
        props.put("producer.bootstrap.servers", bootStrapServer);
        props.put("producer.security.protocol", "SSL");
        props.put("producer.ssl.truststore.location", sslTrustStoreLocation);
        props.put("producer.ssl.truststore.password", sslTrustStorePassword);
        props.put("producer.ssl.keystore.location", sslKeystoreLocation);
        props.put("producer.ssl.keystore.password", sslKeystorePassword);
        props.put("producer.ssl.key.password", sslKeyPassword);

        props.put("security.protocol", "SSL");
        props.put("ssl.truststore.location", sslTrustStoreLocation);
        props.put("ssl.truststore.password", sslTrustStorePassword);
        props.put("ssl.keystore.location", sslKeystoreLocation);
        props.put("ssl.keystore.password", sslKeystorePassword);
        props.put("ssl.key.password", sslKeyPassword);


        props.put(ProducerConfig.CLIENT_ID_CONFIG, "ToKafkaApplication");
        props.put(AbstractKafkaSchemaSerDeConfig.SCHEMA_REGISTRY_URL_CONFIG, srUrl);
        props.put(ProducerConfig.ACKS_CONFIG, "all");
        props.put(ProducerConfig.RETRIES_CONFIG, 0);
        props.put(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG, StringSerializer.class);
        props.put(ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, KafkaAvroSerializer.class);
        return props;
    }

    private void sendMessage() {
        try {
            for(int i = 0; i < 10; i++) {
                String id = String.valueOf(FAKE_DATA.number().randomNumber(2, true));
                String cc = String.valueOf(FAKE_DATA.finance().creditCard());
                String idPerson = FAKE_DATA.idNumber().peselNumber();
                final WalletTransaction walletTransaction = WalletTransaction.newBuilder().
                        setAmount(FAKE_DATA.number().randomDouble(2, 1, 99))
                .setId(id)
                        .setType("charge")
                        .setCurrency("USD").setFee(3.07d)
                        .setPayoutStatus("scheduled")
                        .setPayoutId(id + FAKE_DATA.number().randomNumber(2, true))
                        .setSourceType("CreditCard")
                        .setProcessedAt(LocalDateTime.now().toString())
                        .setWalletId("W-" + idPerson)
                        .setSourceId(cc)
                        .setCustomerId(idPerson).build();
                final ProducerRecord<String, WalletTransaction> record = new ProducerRecord<>(TOPIC,
                        walletTransaction.getId().toString(), walletTransaction);
                Future<RecordMetadata> recordMetadataFuture = producer.send(record);
                RecordMetadata recordMetadata = recordMetadataFuture.get();
                logger.info("response -- " + recordMetadata.topic() + " : " +
                        recordMetadata.partition() + " : " + recordMetadata.offset());
                Thread.sleep(100L);
            }
            producer.flush();
        } catch (final SerializationException | InterruptedException | ExecutionException e) {
            logger.log(Level.SEVERE, e.getMessage(), e);
        }
    }
}
