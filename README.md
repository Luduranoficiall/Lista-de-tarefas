# 📋 Lista de Tarefas (To-Do List) — C#

[![Build & Test](https://github.com/Luduranoficiall/Lista-de-tarefas/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Luduranoficiall/Lista-de-tarefas/actions/workflows/dotnet.yml)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)

Aplicação de console em **C# puro** (.NET 8, sem frameworks web) para gerenciamento de tarefas do dia a dia. Implementa um **CRUD completo** (Create, Read, Update, Delete) com uma arquitetura em camadas, regras de negócio isoladas, tratamento de erros com exceções de domínio e cobertura de testes automatizados — pensada como projeto de portfólio para demonstrar boas práticas de engenharia de software em C#.

## Funcionalidades

| # | Ação | Descrição |
|---|------|-----------|
| 1 | Criar tarefa | Título, descrição e prioridade (Baixa / Média / Alta) |
| 2 | Listar tarefas | Todas as tarefas, ordenadas por status e prioridade |
| 3 | Buscar por ID | Detalhe de uma tarefa específica |
| 4 | Buscar/filtrar | Por texto (título/descrição), status e/ou prioridade |
| 5 | Atualizar tarefa | Edição parcial — campos em branco mantêm o valor atual |
| 6 | Concluir / reabrir | Alterna o status de conclusão |
| 7 | Remover tarefa | Com confirmação |
| 0 | Sair | Encerra a aplicação |

Os dados são persistidos automaticamente em `tarefas.json` a cada alteração — sem necessidade de banco de dados.

## Arquitetura

O projeto segue uma separação de responsabilidades em camadas, inspirada em aplicações profissionais de maior porte:

```
Console (UI) → Service (regras de negócio) → Repository (persistência) → Modelo
```

```
Lista-de-tarefas/
├── src/
│   └── ListaDeTarefas/
│       ├── Models/
│       │   └── Tarefa.cs                 # Entidade Tarefa e enum Prioridade
│       ├── Exceptions/
│       │   ├── TarefaNaoEncontradaException.cs
│       │   └── TarefaInvalidaException.cs
│       ├── Repositories/
│       │   ├── ITarefaRepository.cs      # Contrato de acesso a dados
│       │   └── TarefaRepository.cs       # Persistência em JSON (System.Text.Json)
│       ├── Services/
│       │   ├── ITarefaService.cs         # Contrato de regras de negócio
│       │   └── TarefaService.cs          # Validações e orquestração
│       ├── UI/
│       │   └── Menu.cs                   # Interação via console
│       └── Program.cs                    # Composition root (injeção de dependência)
├── tests/
│   └── ListaDeTarefas.Tests/
│       ├── TarefaServiceTests.cs
│       └── TarefaRepositoryTests.cs
└── .github/workflows/dotnet.yml          # CI: build + testes a cada push/PR
```

### Por que essa separação?

- **Models** — apenas representa o dado, sem comportamento além do essencial.
- **Repositories** — abstrai *onde e como* os dados são persistidos. A UI e o Service nunca sabem que o armazenamento é um arquivo JSON; trocar para um banco de dados exigiria apenas uma nova implementação de `ITarefaRepository`.
- **Services** — concentra as regras de negócio (validação de título/descrição, buscas com filtros, transições de estado) e traduz falhas em exceções de domínio (`TarefaInvalidaException`, `TarefaNaoEncontradaException`). A UI nunca decide se um dado é válido — ela só reage ao resultado.
- **UI** — cuida exclusivamente da interação com o usuário no console (leitura de input, exibição, tratamento de exceções de domínio).
- **Injeção de dependência** — `Program.cs` monta o grafo de dependências via `Microsoft.Extensions.DependencyInjection`, o mesmo padrão usado em aplicações ASP.NET Core. Cada componente depende de abstrações (`ITarefaRepository`, `ITarefaService`), não de implementações concretas — o que também é o que torna os testes de unidade possíveis sem tocar em I/O real.

## Como executar

Pré-requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/Luduranoficiall/Lista-de-tarefas.git
cd Lista-de-tarefas
dotnet run --project src/ListaDeTarefas
```

## Testes

O projeto tem cobertura de testes automatizados (xUnit) para as regras de negócio e a camada de persistência — incluindo casos de validação, exceções de domínio e comportamento de busca/filtro.

```bash
dotnet test
```

A cada push ou pull request para `main`, o GitHub Actions builda o projeto e roda a suíte de testes automaticamente (veja o badge no topo deste README).

## Tecnologias e padrões

- **C# 12 / .NET 8**
- `System.Text.Json` para persistência local em arquivo
- **Repository Pattern** para abstrair a persistência
- **Service Layer** para isolar regras de negócio da interface
- **Exceções de domínio** (`TarefaNaoEncontradaException`, `TarefaInvalidaException`) no lugar de códigos de retorno mágicos
- **Injeção de dependência** (`Microsoft.Extensions.DependencyInjection`)
- **xUnit** para testes de unidade
- **GitHub Actions** para integração contínua (build + testes)

## Possíveis evoluções

- Persistência em banco relacional (ex.: SQLite via EF Core), bastando implementar uma nova `ITarefaRepository`
- Categorias/tags nas tarefas
- Exportação da lista para CSV/PDF
- Interface gráfica (WPF/MAUI) reaproveitando as camadas Service e Repository sem alterações

## Licença

Distribuído sob a licença [MIT](LICENSE).

---

Projeto desenvolvido como peça de portfólio, com foco em boas práticas de arquitetura em C#.
