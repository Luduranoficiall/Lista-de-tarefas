# Lista de Tarefas (To-Do List) em C#

[![Build & Test](https://github.com/Luduranoficiall/Lista-de-tarefas/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Luduranoficiall/Lista-de-tarefas/actions/workflows/dotnet.yml)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)

Aplicação de console em C# puro (.NET 8, sem framework web), CRUD completo de tarefas com
arquitetura em camadas, regras de negócio isoladas em exceções de domínio, e testes
automatizados cobrindo a lógica real, não só o feliz caminho.

## Índice

- [O que faz](#o-que-faz)
- [Como rodar](#como-rodar)
- [Arquitetura](#arquitetura)
- [Testes](#testes)
- [Estrutura de arquivos](#estrutura-de-arquivos)
- [Possíveis evoluções](#possíveis-evoluções)

## O que faz

| # | Ação | Descrição |
|---|------|-----------|
| 1 | Criar tarefa | Título, descrição e prioridade (Baixa, Média, Alta) |
| 2 | Listar tarefas | Todas, ordenadas por status e prioridade |
| 3 | Buscar por ID | Detalhe de uma tarefa específica |
| 4 | Buscar/filtrar | Por texto (título/descrição), status e/ou prioridade |
| 5 | Atualizar tarefa | Edição parcial: campo em branco mantém o valor atual |
| 6 | Concluir/reabrir | Alterna o status de conclusão |
| 7 | Remover tarefa | Com confirmação |
| 0 | Sair | Encerra a aplicação |

Os dados persistem em `tarefas.json`, gravado a cada alteração. Sem banco de dados, sem
dependência externa.

## Como rodar

Precisa do [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/Luduranoficiall/Lista-de-tarefas.git
cd Lista-de-tarefas
dotnet run --project src/ListaDeTarefas
```

## Arquitetura

```
Console (UI) → Service (regras de negócio) → Repository (persistência) → Modelo
```

- **Models**: só representa o dado, sem comportamento além do essencial.
- **Repositories**: abstrai onde e como os dados são persistidos. UI e Service nunca sabem que
  o armazenamento é um arquivo JSON; trocar por banco de dados exige só uma nova implementação
  de `ITarefaRepository`.
- **Services**: concentra a regra de negócio (validação de título/descrição, busca com
  filtro, transição de estado) e traduz falha em exceção de domínio
  (`TarefaInvalidaException`, `TarefaNaoEncontradaException`). A UI nunca decide se um dado é
  válido, só reage ao resultado.
- **UI**: cuida só da interação no console (ler input, exibir, tratar exceção de domínio).
- **Injeção de dependência**: `Program.cs` monta o grafo via
  `Microsoft.Extensions.DependencyInjection`, mesmo padrão de uma aplicação ASP.NET Core. Cada
  componente depende de abstração (`ITarefaRepository`, `ITarefaService`), nunca de
  implementação concreta, o que é também o que torna o teste de unidade possível sem tocar em
  I/O real.

## Testes

```bash
dotnet test
```

16 testes (xUnit), cobrindo `TarefaService` (validação de título/descrição, busca com filtro
combinado, exceção ao buscar/atualizar/remover ID inexistente) e `TarefaRepository`
(persistência real em arquivo temporário, geração de próximo ID, atualização parcial). GitHub
Actions builda e roda a suíte a cada push ou pull request pra `main` (badge no topo).

## Estrutura de arquivos

```
src/ListaDeTarefas/
├── Models/Tarefa.cs                  entidade Tarefa e enum Prioridade
├── Exceptions/                       TarefaNaoEncontradaException, TarefaInvalidaException
├── Repositories/
│   ├── ITarefaRepository.cs          contrato de acesso a dado
│   └── TarefaRepository.cs           persistência em JSON (System.Text.Json)
├── Services/
│   ├── ITarefaService.cs             contrato de regra de negócio
│   └── TarefaService.cs              validação e orquestração
├── UI/Menu.cs                        interação via console
└── Program.cs                        composition root (injeção de dependência)
tests/ListaDeTarefas.Tests/
├── TarefaServiceTests.cs
└── TarefaRepositoryTests.cs
.github/workflows/dotnet.yml          CI: build + teste a cada push/PR
```

## Possíveis evoluções

- Persistência em banco relacional (SQLite via EF Core), bastando implementar uma nova
  `ITarefaRepository`.
- Categoria/tag nas tarefas.
- Exportação da lista pra CSV/PDF.
- Interface gráfica (WPF/MAUI) reaproveitando Service e Repository sem alteração.

## Licença

Distribuído sob a licença [MIT](LICENSE).
