# gestao-eventos-corporativos
Esse projeto é parte de um desafio de habilidade que estou realizando de uma empresa [Link do PDF privado]().


# Passos para utilizar a aplicação

**Pré-requisitos:**
0 - Carga de trabalho de "ASP.NET e desenvolvimento Web"
1 - Carga de trabalho de "Desenvolvimetno para desktop com .NET (WPF)
2 - SKD .NET 8
3 - Docker Desktop

<details>
<summary> Configurando banco de dados SQL Server com Docker </summary>
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

</details>


# Rodando cobertura de teste
<details>
<summary> Clique aqui </summary>


# 1) Uma vez (setup)

1. No(s) projeto(s) de **teste**, instale o coletor do Coverlet:

```bash
dotnet add GestaoEventosCorporativos.Tests.csproj package coverlet.collector
```

2. Instale o **ReportGenerator** (ferramenta global pra gerar HTML):

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
```

3. (Opcional, mas recomendado) Crie um arquivo de configuração para cobertura: **`coverlet.runsettings`** na raiz do repositório:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <!-- Saída em Cobertura (compatível com vários serviços) -->
          <Format>cobertura</Format>

          <!-- Excluir atributos gerados -->
          <ExcludeByAttribute>CompilerGeneratedAttribute,GeneratedCodeAttribute</ExcludeByAttribute>

          <!-- Excluir assemblies/padrões (ajuste conforme seu naming) -->
          <Exclude>
            [xunit.*]*
            [*.Tests]*      <!-- não cobrir os próprios testes -->
          </Exclude>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

# 2) Comando do dia a dia (rodar cobertura)

Na raiz da solução/projeto:

```bash
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Isso vai gerar arquivos `coverage.cobertura.xml` dentro de `TestResults/**/`.

Agora gere o relatório HTML:

```bash
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Abra o relatório no Windows:

```bash
start coveragereport\index.html
```
</details>


