# 🛒 Guia Completo: Desenvolvimento em C# com Windows Forms e Neon PostgreSQL

Bem-vindo ao guia definitivo para aprender **C#** construindo um projeto real! 

Neste curso, você não aprenderá apenas a teoria. Nós vamos construir, do zero, um **Sistema de Gerenciamento para um Supermercado**, utilizando as melhores práticas de desenvolvimento, padrões de projeto e uma arquitetura limpa. 

O projeto utilizará **Windows Forms (WFA)** para a interface gráfica e o banco de dados em nuvem **Neon PostgreSQL** para armazenamento dos dados.

## 🎯 Objetivo do Guia
Capacitar estudantes e desenvolvedores iniciantes na linguagem C#, ensinando os fundamentos da linguagem, Programação Orientada a Objetos (POO), acesso a banco de dados e boas práticas de arquitetura de software (como o padrão Repository). Não presumimos nenhum conhecimento prévio em configuração de banco de dados!

---

## 📚 Índice de Conteúdos

Siga a ordem dos módulos abaixo para um aprendizado progressivo:

### Parte 1: Fundamentos da Linguagem
1. [**Introdução ao C# e Lógica de Programação**](./01-Introducao-CSharp.md)
   - Variáveis, tipos de dados, estruturas de decisão e repetição.
2. [**Programação Orientada a Objetos (POO)**](./02-Orientacao-a-Objetos.md)
   - Classes, objetos, herança, polimorfismo, encapsulamento e interfaces.

### Parte 2: Arquitetura e Ambiente
3. [**Arquitetura, Padrões e Boas Práticas**](./03-Arquitetura-e-Padroes.md)
   - Por que organizar o código? Padrão Repository e Injeção de Dependências.
4. [**Configuração do Ambiente (Visual Studio 2022)**](./04-Configuracao-Ambiente.md)
   - Instalação e configuração do Visual Studio 2022 e criação do projeto Windows Forms.

### Parte 3: Mão na Massa - O Sistema do Supermercado
5. [**Criando a Interface Visual da Tela de Login**](./05-Criando-Interface-Login.md)
   - Desenhando o Form no Visual Studio 2022 utilizando o Toolbox (Caixa de Ferramentas).
6. [**Criando o Banco de Dados Neon e Tabelas**](./06-Criando-Banco-Dados-Neon.md)
   - Passo a passo detalhado para criar sua conta no Neon PostgreSQL e criar as tabelas `TipoUsuario` e `Usuario`.
7. [**Conectando o Sistema ao Banco (Instalação de Plugins e Autenticação)**](./07-Conectando-Banco-Dados.md)
   - Instalação do plugin/pacote `Npgsql` via NuGet, criação do Repositório e integração com o botão de Login.

---

## 🛠️ Tecnologias Utilizadas
- **Linguagem:** C# (.NET)
- **Interface Gráfica:** Windows Forms App (WFA)
- **IDE:** Visual Studio 2022
- **Banco de Dados:** Neon PostgreSQL (Cloud Serverless)
- **Pacotes NuGet:** `Npgsql` (Driver de acesso ao Postgres)

> *Desenvolvido para ser uma jornada prática, visual e alinhada com as demandas do mercado de trabalho.* Comece pelo [Módulo 1](./01-Introducao-CSharp.md) e bons estudos!
