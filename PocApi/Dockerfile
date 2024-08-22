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

#ENTRYPOINT ["dotnet", "PocApi.dll"]

# Cria o script de inicialização diretamente na imagem
RUN echo '#!/bin/sh\n\n# Altera as permissões do arquivo de configuração (se existir)\nif [ -f /data/options.json ]; then\n    chmod 644 /data/options.json\nfi\n\n# Executa o comando original do seu aplicativo\nexec dotnet PocApi.dll' > /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]