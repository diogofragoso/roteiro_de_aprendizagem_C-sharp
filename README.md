# C# na Prática com Visual Studio 2022: Trilha Unificada do Zero ao Sistema de Supermercado (MarketFlow)

Guia progressivo, linear e autoexplicativo para estudantes que estão **começando do absoluto zero** na linguagem **C#** utilizando o **Visual Studio 2022**. Em **uma única trilha organizada de 24 aulas (6 módulos — 84 horas)**, você parte do seu primeiro programa no console, domina Orientação a Objetos, aprende arquitetura em camadas e constrói por conta própria o **MarketFlow** — um aplicativo desktop Windows Forms com áreas separadas para **Administrador** e **Caixa**, conectado ao **Neon PostgreSQL** ou **MySQL/MariaDB pelo XAMPP**.

---

## Como Este Guia Funciona (Filosofia de Aprendizagem)

1. **Uma única trilha linear, sem repetições nem arquivos dispersos:** comece na **Aula 01** e siga em ordem até a **Aula 24**. Cada encontro introduz conceitos novos na dose certa, sempre apoiados no encontro anterior.
2. **Explicação detalhada do "O quê", "Como" e "Por quê":** nenhum comando, palavra-chave ou símbolo da linguagem C# aparece sem explicação. Cada capítulo disseca a sintaxe com microexemplos independentes e mostra **por que** fazemos daquela forma (e qual problema ocorreria se fizéssemos diferente).
3. **O guia orienta; você constrói:** não existem respostas prontas do projeto para "copiar e colar". Cada aula oferece **passo a passo no Visual Studio 2022** e **orientações estruturadas de construção** para que você escreva cada classe, método, consulta SQL e formulário com autonomia.

> [!TIP]
> **Onde fica o código pronto?** A pasta `src/` deste repositório é de uso exclusivo do professor para conferência e execução das verificações automatizadas. Durante seus estudos, construa tudo pelas orientações dos capítulos — errar, ler a **Lista de Erros** do Visual Studio 2022 e depurar faz parte do aprendizado.

---

## Mapa da Trilha Unificada (24 Aulas em 6 Módulos — 84 Horas)

```mermaid
flowchart LR
    M1["Módulo 1 (01–03)<br/>Do Zero à Lógica no VS 2022"] --> M2["Módulo 2 (04–07)<br/>Orientação a Objetos"]
    M2 --> M3["Módulo 3 (08–10)<br/>Arquitetura e WinForms"]
    M3 --> M4["Módulo 4 (11–14)<br/>Banco, Async e Login"]
    M4 --> M5["Módulo 5 (15–21)<br/>Produtos, Estoque e Caixa"]
    M5 --> M6["Módulo 6 (22–24)<br/>Relatório, Usuários e Entrega"]
```

### Módulo 1 — Do Zero à Lógica no Visual Studio 2022 (Console)

| Aula | Tema e Guia de Construção | O que você vai construir ao final de 3h30 |
| --- | --- | --- |
| 01 | [Primeiro programa, variáveis e tipos](01-Primeiro-Programa-e-Tipos.md) | Projeto Console no VS 2022, os 7 tipos do domínio, `const`, atribuição e moeda `:C` |
| 02 | [Entrada segura, decisões, laços e depuração](02-Entrada-Decisoes-Lacos-e-Depuracao.md) | Caixa de console blindado com `TryParse`, `if`/`switch`, `for`/`while`/`foreach` e `F9`/`F10` |
| 03 | [Métodos, parâmetros, retorno e escopo](03-Metodos-Parametros-e-Escopo.md) | Funções puras de cálculo/validação, `void`, escopo, cópia de valor e depuração `F11` |

### Módulo 2 — Programação Orientada a Objetos na Prática (Console)

| Aula | Tema e Guia de Construção | O que você vai construir ao final de 3h30 |
| --- | --- | --- |
| 04 | [Classes, objetos, propriedades e referências](04-Classes-Objetos-e-Referencias.md) | Classes em arquivos separados, instâncias independentes e mapa de referências na RAM |
| 05 | [Construtores, campos, encapsulamento e exceções](05-Construtores-Encapsulamento-e-Excecoes.md) | Classes blindadas com construtor, `private set`, `throw`/`try-catch` e desafio `SessaoDeEstudo` |
| 06 | [Composição, coleções (`List<T>`) e subtotal](06-Composicao-Colecoes-e-Subtotal.md) | `ItemDeCompra` + `Produto`, lista dinâmica, subtotal calculado (`=>`) e totalização |
| 07 | [Namespaces, interfaces, herança e polimorfismo](07-Namespaces-Interfaces-Heranca-e-Polimorfismo.md) | Contratos `interface`, injeção pelo construtor, `virtual`/`override`/`: base` e polimorfismo |

### Módulo 3 — Arquitetura em Camadas e Interface Windows Forms

| Aula | Tema e Guia de Construção | O que você vai construir ao final de 3h30 |
| --- | --- | --- |
| 08 | [Arquitetura em camadas e contratos](08-Arquitetura-e-Padroes.md) | Separação em 5 responsabilidades e troca real de repositório em memória por contrato |
| 09 | [Ambiente e solução Windows Forms no VS 2022](09-Configuracao-Ambiente.md) | Solução `Supermercado` e `SupermercadoApp` organizados em 5 pastas compilando |
| 10 | [Interface de login pelo Designer](10-Criando-Interface-Login.md) | Tela `FormLogin` navegável pelo teclado (`Tab`/`Enter`), senha oculta e validação visual |

### Módulo 4 — Banco de Dados, Assincronia, Segurança e Perfis

| Aula | Tema e Guia de Construção | O que você vai construir ao final de 3h30 |
| --- | --- | --- |
| 11 | [Banco de dados (Neon ou XAMPP) e usuários](11-Criando-Banco-Dados-Neon.md) | Tabela `usuario` com restrições de integridade testadas e sem senha em texto puro |
| 12 | [Conexão assíncrona e pool de conexões](12-Conectando-Banco-Dados.md) | `Banco.cs` com variável de ambiente e consulta `SELECT 1` sem congelar a tela |
| 13 | [Autenticação segura com hash e salt](13-Autenticacao-Segura.md) | Primeiro administrador e login verificando hash com `PasswordHasher` e SQL parametrizado |
| 14 | [Sessão, perfis e autorização](14-Sessao-e-Perfis.md) | Menus separados (`Administrador` e `Caixa`), autorização no serviço e troca limpa de usuário |

### Módulo 5 — Produtos, Estoque e Frente de Caixa (PDV)

| Aula | Tema e Guia de Construção | O que você vai construir ao final de 3h30 |
| --- | --- | --- |
| 15 | [Modelo e regras de produtos](15-Modelo-de-Produtos.md) | Tabelas `produto`/`movimento_estoque` e `ProdutoService` testado com Fake em memória |
| 16 | [Consulta de produtos na grade (`DataGridView`)](16-Consulta-de-Produtos.md) | Busca parametrizada (`ILIKE`/`LIMIT 200`) e carregamento assíncrono no evento `Shown` |
| 17 | [Cadastro, edição e inativação lógica](17-Cadastro-de-Produtos.md) | CRUD de produtos, seleção por `DataBoundItem`, tratamento de duplicidade (`23505`) e inativação |
| 18 | [Reposição de estoque com transação](18-Reposicao-de-Estoque.md) | Incremento atômico no SQL e histórico de auditoria gravados na mesma transação |
| 19 | [Carrinho do operador de caixa](19-Carrinho-do-Caixa.md) | Agrupamento de itens em memória (`AsReadOnly`, `with`, `checked`) e total previsto |
| 20 | [Persistência de venda com transação e concorrência](20-Venda-com-Transacao.md) | Venda atômica, snapshot histórico, idempotência (`Guid`), `FOR UPDATE` e prova de rollback |
| 21 | [Finalização, troco e tentativa pendente](21-Finalizacao-e-Troco.md) | Máquina de estados do caixa, cálculo de troco, `FormClosing` e reconciliação de falhas |

### Módulo 6 — Relatórios, Usuários, Verificação e Publicação

| Aula | Tema e Guia de Construção | O que você vai construir ao final de 3h30 |
| --- | --- | --- |
| 22 | [Relatório de vendas por período](22-Relatorio-de-Vendas.md) | Consulta `JOIN` por intervalo semiaberto UTC, horário local e totalização |
| 23 | [Cadastro de usuários pelo administrador](23-Cadastro-de-Usuarios.md) | Formulário de contas com `ComboBox`/`Enum` reutilizando `AutenticacaoService` |
| 24 | [Verificação automatizada e publicação no VS 2022](24-Testes-e-Publicacao.md) | Projeto de verificações com Fakes, roteiro manual em dupla e executável autocontido |

---

## Documentos de Apoio e Escolha do Banco de Dados

- **Banco de Dados (a partir da Aula 11):** a trilha principal utiliza **Neon / PostgreSQL** (`Npgsql`). Para banco local sem nuvem, siga a [**Trilha Alternativa MySQL/MariaDB pelo XAMPP**](INTEGRACAO-MYSQL-XAMPP.md).
- **Planejamento e Avaliação:** consulte o [**Plano de Aulas**](PLANO-DE-AULAS.md), o [**Caderno de Atividades por Conceito (`A01`–`A24`)**](ATIVIDADES-POR-CONCEITO.md) e o [**Caderno de Evidências**](CADERNO-DE-EVIDENCIAS.md).
- **Para Professores e Referência:** consulte o [**Apoio ao Professor**](ensino/README.md), o [**Código de Referência Docente**](src/README.md), a [**Análise Pedagógica**](ANALISE-PEDAGOGICA.md), as [**Referências Técnicas**](REFERENCIAS.md), o [**Guia de Transição**](TRANSICAO.md) e os registros de validação ([VALIDACAO-MELHORIAS.md](VALIDACAO-MELHORIAS.md) e [VALIDACAO.md](VALIDACAO.md)).

---

## Ambiente Recomendado (Visual Studio 2022)

- **IDE:** **Visual Studio 2022 Community** (gratuito) com a carga de trabalho **Desenvolvimento para desktop com .NET**.
- **SDK / .NET:** utilize a versão moderna do .NET adotada pelo laboratório (**.NET 8 LTS** nativo no Visual Studio 2022; os arquivos de referência do repositório seguem `global.json`).
- **Decisões de escopo:** um projeto único organizado em 5 pastas, injeção manual pelo construtor, SQL parametrizado, produtos em unidades inteiras e dinheiro simulado (*sem valor fiscal*). A fronteira de segurança de uma API comercial é explicada explicitamente desde a [Aula 08](08-Arquitetura-e-Padroes.md).
