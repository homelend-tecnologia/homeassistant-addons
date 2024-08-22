#!/bin/sh

# Copia o arquivo de configuração para um local acessível pelo usuário 'app'
if [ -f /data/options.json ]; then
    cp /data/options.json /app/options.json  # Copia para o diretório /app
    chmod 644 /app/options.json  # Define permissões de leitura para todos
fi

# Executa o comando original do seu aplicativo
exec dotnet PocApi.dll
