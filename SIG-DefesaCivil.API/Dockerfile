# Build Stage
using Google.Apis.Drive.v3.Data;
using static System.Net.WebRequestMethods;

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia tudo e restaura
COPY . ./
RUN dotnet restore

# Publica a aplicação
RUN dotnet publish -c Release -o out

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Exponha a porta que o Render usa (geralmente ele injeta a PORT env var, mas o .NET precisa saber)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SIG-DefesaCivil.API.dll"]# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia tudo e restaura
COPY . ./
RUN dotnet restore

# Publica a aplicação
RUN dotnet publish -c Release -o out

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Exponha a porta que o Render usa (geralmente ele injeta a PORT env var, mas o .NET precisa saber)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SIG-DefesaCivil.API.dll"]