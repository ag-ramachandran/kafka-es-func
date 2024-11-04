#!/bin/bash

set -o nounset \
    -o errexit

printf "Deleting previous (if any)..."
rm -rf secrets
mkdir secrets
mkdir -p tmp
echo " OK!"
# Generate CA key
printf "Creating CA..."
openssl req -new -x509 -keyout tmp/datahub-ca.key -out tmp/datahub-ca.crt -days 365 -subj '/CN=ca.datahub/OU=test/O=datahub/L=paris/C=fr' -passin pass:datahub -passout pass:datahub >/dev/null 2>&1

echo " OK!"

for i in 'broker' 'producer' 'consumer' 'schema-registry'
do
	printf "Creating cert and keystore of $i..."
	# Create keystores
	keytool -genkey -noprompt \
				 -alias $i \
				 -dname "CN=$i, OU=test, O=datahub, L=paris, C=fr" \
				 -keystore secrets/$i.keystore.jks \
				 -keyalg RSA \
				 -storepass datahub \
				 -keypass datahub  >/dev/null 2>&1

	# Create CSR, sign the key and import back into keystore
	keytool -keystore secrets/$i.keystore.jks -alias $i -certreq -file tmp/$i.csr -storepass datahub -keypass datahub >/dev/null 2>&1

	openssl x509 -req -CA tmp/datahub-ca.crt -CAkey tmp/datahub-ca.key -in tmp/$i.csr -out tmp/$i-ca-signed.crt -days 365 -CAcreateserial -passin pass:datahub  >/dev/null 2>&1

	keytool -keystore secrets/$i.keystore.jks -alias CARoot -import -noprompt -file tmp/datahub-ca.crt -storepass datahub -keypass datahub >/dev/null 2>&1

	keytool -keystore secrets/$i.keystore.jks -alias $i -import -file tmp/$i-ca-signed.crt -storepass datahub -keypass datahub >/dev/null 2>&1

	# Create truststore and import the CA cert.
	keytool -keystore secrets/$i.truststore.jks -alias CARoot -import -noprompt -file tmp/datahub-ca.crt -storepass datahub -keypass datahub >/dev/null 2>&1
  echo " OK!"
done

echo "datahub" > secrets/cert_creds

cd storm-events-producer
keytool -importkeystore -srckeystore ../secrets/producer.truststore.jks -destkeystore server.p12 -deststoretype PKCS12 -keypass datahub -deststorepass datahub -storepass datahub -srcstorepass datahub
openssl pkcs12 -in server.p12 -nokeys -out server.cer.pem -legacy -passin pass:datahub 
openssl pkcs12 -in server.p12 -nodes -nocerts -out server.key.pem -legacy -passin pass:datahub 
keytool -importkeystore -srckeystore ../secrets/producer.keystore.jks -destkeystore client.p12 -deststoretype PKCS12 -keypass datahub -deststorepass datahub -storepass datahub -srcstorepass datahub
openssl pkcs12 -in client.p12 -nokeys -out client.cer.pem -legacy -passin pass:datahub 
openssl pkcs12 -in client.p12 -nodes -nocerts -out client.key.pem -legacy -passin pass:datahub 

cd ..

keytool -importkeystore -srckeystore ./secrets/consumer.truststore.jks -destkeystore consumer.p12 -srcstoretype jks -deststoretype pkcs12 -keypass datahub -deststorepass datahub -storepass datahub -srcstorepass datahub
openssl pkcs12 -in consumer.p12 -out CARoot.pem -legacy -passin pass:datahub 
keytool -exportcert -alias consumer -keystore ./secrets/consumer.keystore.jks -rfc -file ./certificate.pem -storepass datahub
keytool -importkeystore -srckeystore ./secrets/consumer.keystore.jks -destkeystore keystore.p12 -deststoretype PKCS12 -storepass datahub -srcstorepass datahub
openssl pkcs12 -in keystore.p12 -nodes -nocerts -out RSAkey.pem -legacy -passin pass:datahub 

cp RSAkey.pem ./kafka-es-function
cp CARoot.pem ./kafka-es-function
cp certificate.pem ./kafka-es-function
cp consumer.p12 ./kafka-es-function


rm -rf tmp

echo "SUCCEEDED"