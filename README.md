# gestao-eventos-corporativos
Esse projeto é parte de um desafio de habilidade que estou realizando de uma empresa [Link do PDF privado]().


## Como Rodar o Projeto

1. Criar container do banco de dados:

docker pull mcr.microsoft.com/mssql/server:2022-latest

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Root@12345" -p 1433:1433 --name sqlserver2022 -d mcr.microsoft.com/mssql/server:2022-latest

docker exec -it sqlserver2022 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Root@12345

