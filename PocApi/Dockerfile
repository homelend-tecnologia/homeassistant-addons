#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN echo "Listing contents of /src/PocApi" && ls -la /src/PocApi && echo "After ls command"

COPY ["PocApi/PocApi.csproj", "PocApi/"]

RUN echo "Listing contents of /src/PocApi" && ls -la /src/PocApi && echo "After ls command"

RUN dotnet restore "./PocApi/PocApi.csproj"

RUN echo "Listing contents of /src/PocApi" && ls -la /src/PocApi && echo "After ls command"

COPY . .

RUN echo "Listing contents of /src/PocApi" && ls -la /src/PocApi && echo "After ls command"

WORKDIR "/src/PocApi"
RUN dotnet build "./PocApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PocApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PocApi.dll"]