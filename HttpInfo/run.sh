#!/bin/bash

# Caminho do arquivo de opções gerado pelo Home Assistant
OPTIONS_FILE="/data/options.json"

# Verifica se o arquivo de opções existe
if [ -f "$OPTIONS_FILE" ]; then
  echo "Conteúdo do arquivo options.json:"
  cat "$OPTIONS_FILE"
else
  echo "Arquivo options.json não encontrado."
fi

# Lê a configuração passada pelo Home Assistant
VAR_TESTE=$(jq --raw-output '.require_ssl // empty' "$OPTIONS_FILE")

# Exporta a variável de ambiente
export VAR_TESTE

# Verifica se a variável foi corretamente configurada
echo "Iniciando meu app com VAR_TESTE=$VAR_TESTE"

# Inicia o servidor Node.js
npm start
