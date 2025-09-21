Vídeo de apresentação — 🎥 [LINK](https://youtu.be/u8gembunzCA)

# Gestão de Eventos Corporativos

Este projeto faz parte de um **desafio técnico** proposto por uma empresa.  
O objetivo é implementar um sistema de **gestão de eventos corporativos** utilizando boas práticas de arquitetura e desenvolvimento em **.NET**.

[Link PDF privado](https://drive.google.com/file/d/1ns2xZR_hUF6yZ1IBx8EEy1VTNS9wrdT8/view?usp=drive_link)

🎤 **Relatório resumido – Gestão de Eventos Corporativos**

<details>

> O projeto é um sistema de **gestão de eventos corporativos**, construído em **.NET 8** com uma arquitetura em **3 camadas**: **Apresentação (controllers e DTOs)**, **Core (serviços e regras de negócio)** e **Infraestrutura (repositories e banco de dados)**.
>
> A API segue boas práticas de **RESTful**, com uso adequado de verbos HTTP, respostas padronizadas e separação clara entre entidades, DTOs de entrada e saída.
>
> As **regras de negócio** ficam isoladas em **CORE**, enquanto os **repositories** cuidam apenas da comunicação com o banco de dados.
>
> O projeto também traz **autenticação JWT** e criptografia de senhas com **PBKDF2**, garantindo segurança desde o cadastro até o login.
>
> Para a qualidade do código, foram implementados **testes unitários com xUnit e Moq**, além de integração com **Coverlet + ReportGenerator** para medição de cobertura.
>
> Como solicitado, implementei os **relatórios úteis**: agenda de participantes, fornecedores mais utilizados, tipos de participantes mais frequentes e saldo de orçamento dos eventos.
>
> Frontend criado em WPF (Windows Presentation Foundation), tecnologia da Microsoft voltada para o desenvolvimento de aplicações desktop modernas no Windows.
>
> Documentação técnica da API gerada automaticamente pelo Swagger
> 
> Em resumo: é um sistema completo, modular e seguro, pronto para ser expandido para cenários reais de gestão de eventos.

</details>

🎤 **Relatório COMPLETO – Gestão de Eventos Corporativos**

<details>


Bom, eu vou explicar como organizei esse projeto de Gestão de Eventos Corporativos.

### 1. Arquitetura em 3 camadas

Eu utilizei uma arquitetura em três camadas: **Apresentação, Core e Infraestrutura**.

* Na **camada de apresentação** ficam os controllers, os DTOs(requests/responses). Ela é responsável por receber as requisições HTTP, validar e devolver as respostas padronizadas.
* No **Core** estão as entidades, os serviços e as regras de negócio, ou seja, a parte central da aplicação.
* Já a **infraestrutura** concentra os repositórios, que fazem a comunicação com o banco de dados.
  Essa separação traz clareza, facilita manutenção e torna os testes bem mais simples.

### 2. RESTful API

Eu também segui o padrão **RESTful**. Cada recurso é exposto em endpoints (`/eventos`, `/participantes`, `/fornecedores`, `/relatorios`) e respeita os verbos HTTP (`GET`, `POST`, `PUT`, `DELETE`).
As respostas usam **status codes adequados**, como 200, 201, 400, 404 e 500.
Eu diria que a API está no nível 2 do modelo REST: já temos recursos, verbos e status, mas ainda não implementei o HATEOAS (que seria o nível 3).

### 3. DTOs

Eu usei **DTOs** para separar os contextos:

* **Request DTOs** para entrada de dados,
* **Response DTOs** para saída.
  Isso evita expor diretamente as entidades de domínio e dá mais segurança e flexibilidade.

### 4. Padrão Service

A lógica de negócio está concentrada nos **services**, o que deixa os controllers enxutos, apenas coordenando as requisições. Isso também ajuda muito nos testes unitários, já que cada regra está encapsulada em um serviço.

### 5. Padrão Repository

Cada entidade tem seu **repositório**, que cuida só da persistência. Essa separação permite que, no futuro, se for necessário trocar o banco, a regra de negócio não precise mudar.

### 6. Result Pattern

Eu também implementei o padrão **Result<T>**.
Com ele, todos os métodos retornam de forma padronizada: se deu certo ou não, com mensagem de erro e código de erro quando necessário. Isso evita ficar jogando exceções desnecessárias e facilita muito a vida no controller.

### 7. Tratamento de erros

Dentro dos serviços, eu sempre envolvi operações críticas em **try/catch**. Quando acontece algo inesperado, retorno erros como `DATABASE_ERROR` ou `INTERNAL_ERROR`, sem expor detalhes internos da aplicação.

### 8. Segurança

Na parte de segurança, eu implementei autenticação com **JWT**, que gera o token no login e valida em endpoints privados.
As senhas são armazenadas de forma segura, usando **PBKDF2 com hash + salt**.

### 9. Testes unitários

Eu utilizei **xUnit + Moq**.
Cubro cenários de sucesso, falha de regra de negócio e exceções. Isso garante qualidade, evita regressões e comprova que as regras de negócio estão bem implementadas.

### 10. Relatórios

Além do CRUD, eu implementei os **relatórios** solicitados que trazem valor ao negócio:

* Agenda dos participantes,
* Fornecedores mais utilizados,
* Tipos de participantes mais frequentes,
* E o saldo de orçamento dos eventos.
  Isso mostra que o sistema não é só cadastro, mas também pode gerar insights.

### 11. Boas práticas

Por fim, eu apliquei conceitos de **Clean Code e SOLID**, padronizei as respostas com DTOs e deixei a arquitetura extensível e modular. Isso facilita incluir novos relatórios, novas regras e até novas entidades no futuro sem quebrar a aplicação.

👉 Então, em resumo: eu separei responsabilidades, padronizei comunicações e garanti segurança e testes. Essa foi a minha linha de raciocínio ao estruturar o projeto.

</details>

<details>

## 🗣️ Minha explicação sobre os relacionamentos do sistema

No meu sistema de **Gestão de Eventos Corporativos**, eu modelei as entidades e configurei os relacionamentos no Entity Framework para garantir que tudo ficasse bem organizado, normalizado e refletisse as regras de negócio.

### 🔹 1. Evento e TipoEvento (1\:N)

Cada evento precisa ter um **tipo definido**. Então, por exemplo, posso ter o tipo “Workshop” e vários eventos associados a ele.
Esse é um relacionamento **um-para-muitos**:

* Um **TipoEvento** pode estar em vários eventos.
* Mas cada **Evento** só pode ter um tipo.

---

### 🔹 2. Evento e Participante (N\:N com ParticipanteEvento)

Os eventos precisam controlar os **participantes**.
Um participante pode estar em vários eventos, e um evento pode ter vários participantes. Para modelar isso, eu criei a tabela de junção **ParticipanteEvento**.
Além de ligar as duas entidades, ela ainda guarda informações extras, como a **Data de Inscrição**.

---

### 🔹 3. Evento e Fornecedor (N\:N com EventoFornecedor)

Os eventos também podem ter vários **fornecedores contratados** (buffet, segurança, etc.).
E esses fornecedores podem ser usados em diferentes eventos.
Esse relacionamento é **muitos-para-muitos**, representado pela entidade **EventoFornecedor**, que além das chaves de ligação, armazena o **Valor Contratado**. Isso é um exemplo de relacionamento N\:N **com payload** (porque tem informação extra na relação).

---

### 🔹 4. Evento e regras de negócio

Centralizei algumas **constantes** na classe `EventoRegras`, como a **lotação mínima** e o **orçamento mínimo**, para garantir que essas validações não fiquem soltas no código.

---

### 🔹 5. Participante e Fornecedor

Participantes e fornecedores não se relacionam diretamente entre si, mas ambos têm sua relação indireta com os eventos.

---

### 🔹 6. User

Também tenho uma entidade `User`, que serve para o **controle de autenticação e acesso ao sistema**. Ela não está diretamente ligada às demais, mas é importante para a parte de segurança.

---

### 🔹 Configurações do Entity Framework

Com o **Fluent API**, defini:

* **Nomes das tabelas** (`Eventos`, `Participantes`, etc.).
* **Chaves primárias e compostas** (como em `ParticipanteEvento` e `EventoFornecedor`).
* **Tipos de dados SQL** (`decimal(18,2)` para valores monetários).
* **Regras de tamanho** (ex.: `Nome` com até 200 caracteres).
* **Relacionamentos claros** com `HasOne`, `HasMany`, `WithOne` e `WithMany`.

Isso garante que o banco fique bem estruturado e sem inconsistências.

---

### 🔹 Visão do Banco de Dados

No final, minha modelagem gerou:

* **Tabelas principais**: `Eventos`, `Participantes`, `Fornecedores`, `TiposEventos`, `Users`.
* **Tabelas de junção**: `ParticipantesEventos` e `EventosFornecedores`.

Ou seja, a estrutura está toda em **Terceira Forma Normal (3FN)**, evitando redundância e mantendo integridade.

---

### 🔹 Como eu uso na prática

Quando quero consultar um evento completo, por exemplo, consigo carregar **participantes e fornecedores juntos** usando `Include` e `ThenInclude`. Isso facilita na hora de gerar relatórios e controlar orçamentos.

</details>

---

## 🤔 Como rodar o projeto

<details>
  <summary> Detalhes </summary>

  ## 🛠️ Pré-requisitos

Certifique-se de ter instalado:

1. Carga de trabalho **ASP.NET e desenvolvimento Web** (Visual Studio Installer)  
2. Carga de trabalho **Desenvolvimento para desktop com .NET** (Visual Studio Installer)    
3. **SDK .NET 8**  
4. **Docker Desktop**  



## 🗄️ Configuração do Banco de Dados (SQL Server via Docker)

<details>
<summary><strong>Passo a passo</strong></summary>

**1. Baixar a imagem do SQL Server:**

   _docker pull mcr.microsoft.com/mssql/server:2022-latest_


**2. Rodar o container:**

   _docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Root@12345" -p 1433:1433 --name sqlserver2022 -d mcr.microsoft.com/mssql/server:2022-latest_


**3. Acessar o container (se necessário):**

   _docker exec -it sqlserver2022 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Root@12345_


### 📦 Executando Migrações Iniciais

**Navegue até a pasta da API (ajuste o caminho se necessário):**

...

> ⚠️ Instale a ferramenta se necessário:

_dotnet tool install --global dotnet-ef_

**Crie a primeira migração:**

_dotnet ef migrations add InicialMigration --project ../GestaoEventosCorporativos.Api --startup-project ../GestaoEventosCorporativos.Api --output-dir ../GestaoEventosCorporativos.Api/03-Infrastructure/Migrations_

**Aplicar migrations:**

_dotnet ef database update --project ../GestaoEventosCorporativos.Api --startup-project ../GestaoEventosCorporativos.Api_


</details>



## ▶️ Rodando a Aplicação (API + WPF via Visual Studio)

<details>
<summary><strong>Passo a passo</strong></summary>
  
Após configurar o banco e aplicar as migrações iniciais, você pode rodar a aplicação completa (API + WPF) direto no Visual Studio:

1. Abra a **Solution** no Visual Studio.
2. Clique com o botão direito na **Solution (`GestaoEventosCorporativos.sln`)** → vá em **Propriedades**.
3. No menu lateral, selecione **Startup Project**.
4. Marque a opção **Multiple startup projects**.
5. Configure:

   * **GestaoEventosCorporativos.Api** → **Start**
   * **GestaoEventosCorporativos.Wpf** → **Start**
6. Salve as configurações.
7. Pressione **F5** ou clique em **Start** para rodar.

🔹 Assim, o Visual Studio vai iniciar **simultaneamente a API (Web API)** e o **cliente desktop WPF**, permitindo testar toda a solução integrada.

Obs.: [LINK](https://web.postman.co/workspace/My-Workspace~c2368300-0f6e-4a80-8979-850b7b16f939/collection/7362818-090dde86-d7b4-4fd6-8751-7b6ba12e4182?action=share&source=copy-link&creator=7362818) para testar a API via POSTMAN

</details>
  
</details>

## 📌 Estrutura do Projeto

<details>


* **01-Presentation (Controllers, DTOs, Responses)**
* **02-Core (Entidades, Serviços, Regras de Negócio)**
* **03-Infrastructure (Repositories, Migrations, Banco de Dados)**
* **Tests (xUnit, Moq, cobertura com Coverlet)**
  
</details>

## 🚀 Tecnologias Utilizadas

<details>

- **.NET 8**
- **ASP.NET Core Web API**
- **WPF (desktop client)**
- **Entity Framework Core**
- **SQL Server (via Docker)**
- **xUnit + Moq (testes unitários)**
- **JWT (autenticação)**
- **Coverlet + ReportGenerator (cobertura de testes)**


</details>

## ✅ Rodando Testes com Cobertura de Código

<details>


👉 40% de todo o código do sistema está coberto por testes unitários. Já na camada Core, onde se concentra a lógica de negócio, alcançamos **87%** de cobertura, garantindo robustez e confiabilidade justamente na parte mais crítica da aplicação.

<img width="1720" height="1314" alt="image" src="https://github.com/user-attachments/assets/771c77da-2697-4bb0-9085-affbe9040e80" />


</details>

<details>
<summary><strong>Configuração e execução</strong></summary>

### 1) Configuração inicial

No projeto de **testes**, instale o coletor do Coverlet:

```
dotnet add GestaoEventosCorporativos.Tests.csproj package coverlet.collector
```

Instale o ReportGenerator:

```
dotnet tool install --global dotnet-reportgenerator-globaltool
```

Crie um arquivo **`coverlet.runsettings`** na raiz do repositório:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <Format>cobertura</Format>
          <ExcludeByAttribute>CompilerGeneratedAttribute,GeneratedCodeAttribute</ExcludeByAttribute>
          <Exclude>
            [xunit.*]*
            [*.Tests]*
          </Exclude>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```



### 2) Execução do dia a dia

Rodar testes com cobertura:

```
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

Ou de forma simples:

```
dotnet test --collect:"XPlat Code Coverage"
```

Isso gera arquivos `coverage.cobertura.xml` dentro de `TestResults/**/`.

---

### 3) Gerar relatório em HTML

```
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Abrir relatório no Windows:

```
start coveragereport\index.html
```

</details>


# 📊 Relatório de Funcionalidades — Sistema de Gestão de Eventos Corporativos (WPF + API)

<details>

## 🖥️ Frontend (WPF)

Interface desenvolvida em **WPF (.NET)**, conectada à API via **HttpClient**, com autenticação **JWT** e suporte a **paginação** em todas as listagens.

---

## 🔐 Autenticação

* Login com **usuário e senha**, recebendo **JWT** para acesso autenticado.
* Sessão mantida via `AppSession.Token`.

---

## 📂 Módulos do Sistema

### 🎭 **Tipos de Evento**

* Cadastrar tipo de evento.
* Editar descrição do tipo de evento.
* Excluir tipo de evento.
* Listagem com **paginação**.

---

### 🏢 **Fornecedores**

* Cadastrar fornecedor (Nome do serviço, CNPJ, Valor Base).
* Editar fornecedor direto no formulário.
* Excluir fornecedor.
* Listagem com **paginação**.
* Associar fornecedor a eventos.
* Remover fornecedor de eventos.**_(FUNÇÃO EXTRA)_**
* Atualização automática do **saldo/orçamento** do evento.

---

### 👥 **Participantes**

* Cadastrar participante (Nome Completo, CPF, Telefone, Tipo: VIP, Interno, Externo).
* Editar participante direto no formulário.
* Excluir participante.
* Listagem com **paginação**.
* Associar participante a eventos.
* Remover participante de eventos.**_(FUNÇÃO EXTRA)_**
* Atualização automática da **lotação** do evento.

---

### 📅 **Eventos**

* Cadastrar evento (Nome, Período, Local, Endereço, Tipo, Orçamento, Lotação Máxima).
* Editar evento direto no formulário.
* Excluir evento.
* Listagem com **paginação**.
* Gerenciar participantes de um evento:

  * Adicionar participantes disponíveis.
  * Remover participantes vinculados.**_(FUNÇÃO EXTRA)_**
  * Exibir lista de participantes vinculados.
* Gerenciar fornecedores de um evento:

  * Adicionar fornecedores disponíveis.
  * Remover fornecedores vinculados. **_(FUNÇÃO EXTRA)_**
  * Exibir lista de fornecedores vinculados.
* Atualização automática de **lotação** e **saldo/orçamento**.

---

## 📊 Relatórios

Disponíveis diretamente no **Dashboard (HomeView)**:

* **Saldo e Orçamento dos Eventos**
  Lista cada evento com orçamento máximo, valor gasto em fornecedores e saldo disponível.

* **Tipos de Participantes Mais Frequentes**
  Exibe quantidade de participantes por tipo (VIP, Interno, Externo).

* **Fornecedores Mais Utilizados**
  Lista os fornecedores com número de eventos atendidos e valor total contratado.

* **Agenda de Participante por CPF**
  Permite consultar, a partir do CPF, em quais eventos o participante está cadastrado (com datas e locais).

---

## 🛠️ Outras Funcionalidades Técnicas

* **Paginação implementada** em todas as listagens (Eventos, Participantes, Fornecedores, Tipos de Evento).
* **WPF Navigation**: transição entre telas com botão *Voltar*.
* **Validações básicas** (números, CNPJ, CPF, valores).
* **Feedback visual** ao usuário via `MessageBox`.

---

</details>
