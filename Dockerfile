# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia tudo
COPY . ./

# Restaura as dependências (pode manter na solução ou apontar pro projeto, aqui vamos no projeto pra ser seguro)
RUN dotnet restore "SIG-DefesaCivil.API/SIG-DefesaCivil.API.csproj"

# Publica a aplicação APENAS do projeto da API
# AQUI ESTAVA O ERRO: Adicionamos o caminho do .csproj
RUN dotnet publish "SIG-DefesaCivil.API/SIG-DefesaCivil.API.csproj" -c Release -o out

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SIG-DefesaCivil.API.dll"]