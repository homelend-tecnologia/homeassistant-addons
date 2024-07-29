#!/bin/sh

echo "Listando arquivos da pasta /data"
ls -lah /data

echo "criando pastas para o RabbitMQ"
mkdir -p /data/rabbitmq/mnesia
mkdir -p /data/rabbitmq/logs

echo "Alterando permissões das pastas"
chown -R rabbitmq:rabbitmq /data/rabbitmq

echo "Listando arquivos da pasta /data/rabbitmq/mnesia"
ls -lah /data/rabbitmq/mnesia

echo "Listando arquivos da pasta /data/rabbitmq/logs"
ls -lah /data/rabbitmq/logs


# Iniciando o servidor RabbitMQ
echo "Iniciando o servidor RabbitMQ"
exec rabbitmq-server