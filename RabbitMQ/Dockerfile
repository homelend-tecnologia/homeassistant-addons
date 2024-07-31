# Imagem oficial do RabbitMQ com a interface de gerenciamento
#FROM --platform=$BUILDPLATFORM rabbitmq:3.13-management
FROM rabbitmq:3.13-management

# Define a variável de ambiente para o RabbitMQ
ENV RABBITMQ_DEFAULT_USER=admin
ENV RABBITMQ_DEFAULT_PASS=admin

# Define variáveis de ambiente para o RabbitMQ apontar para o diretório de dados persistente
ENV RABBITMQ_MNESIA_BASE=/data/rabbitmq/mnesia
ENV RABBITMQ_LOG_BASE=/data/rabbitmq/log

# Cria o diretório de dados persistente
#RUN mkdir -p /data/rabbitmq/mnesia /data/rabbitmq/log
#
# Cria os diretórios necessários e ajusta as permissões
#RUN mkdir -p /data/rabbitmq/mnesia /data/rabbitmq/log && \
    #chown -R rabbitmq:rabbitmq /data/rabbitmq
#
#
# Copia arquivos de configuração personalizados
#COPY rabbitmq.conf /etc/rabbitmq/rabbitmq.conf
COPY configs/enabled_plugins /etc/rabbitmq/enabled_plugins

COPY start.sh /start.sh
RUN chmod +x /start.sh

# Não é necessário definir VOLUME para /var/lib/rabbitmq, pois estamos movendo a persistência para /data
# Isso é gerenciado pelo Home Assistant
#
# Configure o ponto de montagem para persistência de dados
#VOLUME ["/var/lib/rabbitmq"]

# Exponha as portas padrão do RabbitMQ
# 5672 é a porta de comunicação padrão
# 1883 é a porta de comunicação MQTT
# 8883 é a porta de comunicação MQTT com TLS
# 15672 é a porta da interface de gerenciamento
# 15674 é a porta de comunicação STOMP com TLS
# 15675 é a porta de comunicação HTTP/WebSockets
# 15692 é a porta de comunicação HTTP/WebSockets com TLS
# 25672 é a porta de comunicação segura (cluster)
# 61613 é a porta de comunicação STOMP

EXPOSE 5672 15672 1883

# Comando para executar o servidor RabbitMQ
# Você pode adicionar comandos para configurar usuários e permissões aqui se necessário
#CMD ["rabbitmq-server"]
CMD ["/start.sh"]
