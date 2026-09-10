# Lista de Tarefas (To-Do List) — C#

Aplicação de console em **C# puro** (.NET 8, sem frameworks) para gerenciamento de tarefas do dia a dia, com CRUD completo:

- **Create** — criar tarefas com título, descrição e prioridade
- **Read** — listar todas as tarefas e buscar uma tarefa específica por ID
- **Update** — editar título, descrição, prioridade e status (concluída/pendente)
- **Delete** — remover tarefas com confirmação

## Arquitetura

O projeto é organizado em camadas para separar responsabilidades:

```
ListaDeTarefas/
├── Models/
│   └── Tarefa.cs           # Entidade Tarefa e enum Prioridade
├── Repositories/
│   ├── ITarefaRepository.cs # Contrato de acesso a dados
│   └── TarefaRepository.cs  # Implementação com persistência em JSON
├── UI/
│   └── Menu.cs               # Interação via console
└── Program.cs                 # Ponto de entrada
```

- **Models**: representa os dados da aplicação.
- **Repositories**: abstrai o acesso e a persistência dos dados (usa `System.Text.Json` para salvar em `tarefas.json`), seguindo o padrão Repository com uma interface (`ITarefaRepository`) para permitir troca de implementação (ex: banco de dados) sem alterar a camada de UI.
- **UI**: cuida apenas da interação com o usuário no console.

## Como executar

Pré-requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run
```

Os dados são persistidos automaticamente em `tarefas.json`, na pasta do projeto, a cada alteração.

## Funcionalidades

| Opção | Ação |
|-------|------|
| 1 | Criar tarefa |
| 2 | Listar tarefas |
| 3 | Buscar tarefa por ID |
| 4 | Atualizar tarefa |
| 5 | Marcar como concluída / pendente |
| 6 | Remover tarefa |
| 0 | Sair |

## Tecnologias

- C# 12 / .NET 8
- `System.Text.Json` para persistência local
