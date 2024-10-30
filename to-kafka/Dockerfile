FROM eclipse-temurin:17-jre-jammy
ARG JAR_FILE=target/to-kafka-*.jar
ENV CERT_HOME=/opt/cert
RUN mkdir -p $CERT_HOME
COPY producer.keystore.jks $CERT_HOME/producer.keystore.jks
COPY producer.truststore.jks $CERT_HOME/producer.truststore.jks
COPY $JAR_FILE /app/runner.jar
EXPOSE 8080
ENTRYPOINT java -jar /app/runner.jar