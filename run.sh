#!/bin/bash

sh generate-certs.sh

docker compose up --build -d

rm -fr secrets 
rm -fr *.pem
rm -fr *.p12

rm -fr storm-events-producer/*.pem
rm -fr storm-events-producer/*.p12
rm -fr storm-events-producer/*.jks

