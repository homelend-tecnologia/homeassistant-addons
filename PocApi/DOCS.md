# Homelend Tecnologia
## Home Assistant Add-on: PocApi

### Instalação

Siga as instruções abaixo para instalar o add-on PocApi no Home Assistant.

1. Abra o Home Assistant.
2. No menu lateral, clique em Configurações.
3. Clique em Add-ons.
4. Clique no ícone no canto inferior direito para entrar na loja de add-ons.
5. No canto superior direito, clique no ícone de 3 pontos e selecione Repositórios.
6. Cole o link do repositório da Homelend: `https://github.com/Homelend-Tecnologia/homeassistant-addons`
7. Clique em Adicionar.
8. Volte para a loja de add-ons e instale o PocApi.
9. Configure o add-on e inicie-o.

### Configuração

### Health Check
Verifique se o add-on está em execução e se o serviço está disponível.

```http://<base_url>:<port>/healthz```

#### Retorno http:
**200** - Execução normal


