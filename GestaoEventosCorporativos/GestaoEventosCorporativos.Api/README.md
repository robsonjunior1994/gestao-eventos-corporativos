# gestao-eventos-corporativos
Esse projeto é parte de um desafio de habilidade que estou realizando de uma empresa [Link do PDF privado]().


# Passos para utilizar a aplicação

## Configurando banco de dados SQL Server com Docker

1. Baixar imagem do sql server:

docker pull mcr.microsoft.com/mssql/server:2022-latest

2. Rodar o container do sql server:

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Root@12345" -p 1433:1433 --name sqlserver2022 -d mcr.microsoft.com/mssql/server:2022-latest

3. Acessar o container do sql server se necessário:

docker exec -it sqlserver2022 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Root@12345

## Executando as Migrações Iniciais
Para aplicar as migrações do banco de dados:

# Navegue até a pasta do projeto API (ajuste o caminho se necessário)
cd ../GestaoEventosCorporativos.Api.Api

# Execute os comandos de migração (no Package Manager Console do Visual Studio ou similar)
dotnet ef migrations add InicialMigration --project ../GestaoEventosCorporativos.Api --startup-project ../GestaoEventosCorporativos.Api --output-dir ../GestaoEventosCorporativos.Api/03-Infrastructure/Migrations


dotnet ef database update --project ../GestaoEventosCorporativos.Api --startup-project ../GestaoEventosCorporativos.Api

# OBS.: Caso tenha algum problema execute o comando:
dotnet tool install --global dotnet-ef
