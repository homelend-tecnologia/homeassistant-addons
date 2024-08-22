FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# mudando para o usuário root, para conseguir os acessor necessários no HA
#USER app
USER root
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

ENTRYPOINT ["dotnet", "PocApi.dll"]