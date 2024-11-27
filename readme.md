# KML Filter API

Este é um projeto de **Web API** desenvolvida em **ASP.NET Core** para filtrar e exportar dados a partir de um arquivo KML. A API oferece três principais funcionalidades:

- **Exportar arquivo KML** com base em filtros aplicados.
- **Listar elementos filtrados** no formato JSON.
- **Obter os valores disponíveis para filtragem**.

## Funcionalidades

A API permite filtrar dados de um arquivo KML usando os seguintes critérios:

- **CLIENTE** (pré-seleção de valores)
- **SITUAÇÃO** (pré-seleção de valores)
- **BAIRRO** (pré-seleção de valores)
- **REFERÊNCIA** (texto parcial, mínimo 3 caracteres)
- **RUA/CRUZAMENTO** (texto parcial, mínimo 3 caracteres)

A API gera um novo arquivo KML com base nos filtros ou retorna os dados filtrados em formato JSON.

## Estrutura do Projeto

- **Controllers**: Contém os controladores da API.
- **Services**: Implementa a lógica de negócios para manipulação do arquivo KML.
- **Models**: Define os modelos de dados usados nas requisições e respostas da API.

## Pré-requisitos

Certifique-se de ter os seguintes softwares instalados em seu ambiente:

- [SDK do .NET 6 ou superior](https://dotnet.microsoft.com/download/dotnet) para rodar o projeto.
- [Visual Studio](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/) para editar o código.

## Instalação

1. Clone o repositório para sua máquina local:
    ```bash
    git clone https://github.com/seu-usuario/kml-filter-api.git
    ```

2. Navegue até a pasta do projeto:
    ```bash
    cd kml-filter-api
    ```

3. Restaure as dependências do projeto:
    ```bash
    dotnet restore
    ```

4. Execute a aplicação:
    ```bash
    dotnet run
    ```

5. A API estará rodando em `http://localhost:5000` ou outro URL:PORTA configurado. 
   Utilizando o Swagger, sua URL será: `http://localhost:5259/swagger/index.html`, com a porta podendo mudar dependendo da configuração

## Endpoints

### 1. Exportar Novo Arquivo KML

- **Método**: `POST`
- **URL**: `/api/placemarks/export`
- **Parâmetros**:
    - `cliente` (string): Filtro por cliente (opcional).
    - `situacao` (string): Filtro por situação (opcional).
    - `bairro` (string): Filtro por bairro (opcional).
    - `referencia` (string): Texto parcial para filtro por referência (mínimo 3 caracteres).
    - `ruaCruzamento` (string): Texto parcial para filtro por rua/cruzamento (mínimo 3 caracteres).

- **Resposta**: Retorna o arquivo KML filtrado como um download.

#### Exemplo de cURL:
```bash
curl -X 'POST' 
  'http://localhost:5259/api/Placemark/export' 
  -H 'accept: */*' 
  -H 'Content-Type: application/json' 
  -d '{
      "cliente": "GRADE",
      "bairro": "CENTRO"
  }' --output exportedFile.kml
```

### 2. Listar Placemarks Filtrados

- **Método**: `GET`
- **URL**: `/api/placemarks`
- **Parâmetros**:
  - `cliente` (string): Filtro por cliente (opcional).
  - `situacao` (string): Filtro por situação (opcional).
  - `bairro` (string): Filtro por bairro (opcional).
  - `referencia` (string): Texto parcial para filtro por referência (mínimo 3 caracteres).
  - `ruaCruzamento` (string): Texto parcial para filtro por rua/cruzamento (mínimo 3 caracteres).

- **Resposta**:
  - Retorna os elementos filtrados no formato JSON.

#### Exemplo de cURL:
```bash
  curl -X 'GET'
  'http://localhost:5259/api/Placemark?cliente=GRADE&bairro=CENTRO'
  -H 'accept: */*'
```

### 3. Obter Valores para Filtragem

- **Método**: `GET`
- **URL**: `/api/placemarks/filters`
- **Resposta**:
  - Retorna os valores únicos para os filtros disponíveis (CLIENTE, SITUAÇÃO, BAIRRO).

#### Exemplo de cURL:
```bash
  curl -X 'GET' 
  'http://localhost:5259/api/Placemark/filters' 
  -H 'accept: */*'
```
