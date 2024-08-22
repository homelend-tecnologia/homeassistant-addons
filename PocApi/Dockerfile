FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

#COPY ["PocApi/PocApi.csproj", "PocApi/"]
#RUN dotnet restore "./PocApi/PocApi.csproj"

COPY ["PocApi.csproj", "./"]
#RUN dotnet nuget locals all --clear
RUN dotnet restore "./PocApi.csproj" 
#--verbosity detailed

COPY . .

#WORKDIR "/src/PocApi"
RUN dotnet build "./PocApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PocApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

USER root

# Cria o script de inicialização diretamente na imagem com as permissões corretas
RUN echo '#!/bin/sh\n\n# Copia o arquivo de configuração para um local acessível pelo usuário '\''app'\''\nif [ -f /data/options.json ]; then\n    cp /data/options.json /app/options.json  # Copia para o diretório /app\n    chmod 644 /app/options.json  # Define permissões de leitura para todos\nfi\n\n# Executa o comando original do seu aplicativo\nexec dotnet PocApi.dll' > /app/entrypoint.sh && chmod +x /app/entrypoint.sh

USER app

ENTRYPOINT ["/app/entrypoint.sh"]

#ENTRYPOINT ["dotnet", "PocApi.dll"]