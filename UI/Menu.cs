using ListaDeTarefas.Models;
using ListaDeTarefas.Repositories;

namespace ListaDeTarefas.UI;

public class Menu
{
    private readonly ITarefaRepository _repositorio;

    public Menu(ITarefaRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public void Executar()
    {
        var executando = true;

        while (executando)
        {
            ExibirCabecalho();
            Console.WriteLine("1 - Criar tarefa");
            Console.WriteLine("2 - Listar tarefas");
            Console.WriteLine("3 - Buscar tarefa por ID");
            Console.WriteLine("4 - Atualizar tarefa");
            Console.WriteLine("5 - Marcar como concluída / pendente");
            Console.WriteLine("6 - Remover tarefa");
            Console.WriteLine("0 - Sair");
            Console.Write("\nEscolha uma opção: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": CriarTarefa(); break;
                case "2": ListarTarefas(); break;
                case "3": BuscarTarefa(); break;
                case "4": AtualizarTarefa(); break;
                case "5": AlternarConclusao(); break;
                case "6": RemoverTarefa(); break;
                case "0": executando = false; break;
                default:
                    Console.WriteLine("Opção inválida.");
                    Pausar();
                    break;
            }
        }

        Console.WriteLine("Até logo!");
    }

    private void CriarTarefa()
    {
        ExibirCabecalho("Nova tarefa");

        Console.Write("Título: ");
        var titulo = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(titulo))
        {
            Console.WriteLine("O título é obrigatório.");
            Pausar();
            return;
        }

        Console.Write("Descrição: ");
        var descricao = Console.ReadLine() ?? string.Empty;

        var prioridade = LerPrioridade();

        var tarefa = _repositorio.Criar(titulo, descricao, prioridade);
        Console.WriteLine($"\nTarefa #{tarefa.Id} criada com sucesso.");
        Pausar();
    }

    private void ListarTarefas()
    {
        ExibirCabecalho("Tarefas do dia");

        var tarefas = _repositorio.ListarTodas();

        if (tarefas.Count == 0)
        {
            Console.WriteLine("Nenhuma tarefa cadastrada.");
        }
        else
        {
            foreach (var tarefa in tarefas)
                Console.WriteLine(tarefa);
        }

        Pausar();
    }

    private void BuscarTarefa()
    {
        ExibirCabecalho("Buscar tarefa por ID");

        var id = LerId();
        if (id is null) return;

        var tarefa = _repositorio.BuscarPorId(id.Value);

        if (tarefa is null)
        {
            Console.WriteLine($"Nenhuma tarefa encontrada com o ID {id}.");
        }
        else
        {
            Console.WriteLine(tarefa);
            Console.WriteLine($"Descrição: {tarefa.Descricao}");
            if (tarefa.ConcluidaEm.HasValue)
                Console.WriteLine($"Concluída em: {tarefa.ConcluidaEm:dd/MM/yyyy HH:mm}");
        }

        Pausar();
    }

    private void AtualizarTarefa()
    {
        ExibirCabecalho("Atualizar tarefa");

        var id = LerId();
        if (id is null) return;

        var tarefaExistente = _repositorio.BuscarPorId(id.Value);
        if (tarefaExistente is null)
        {
            Console.WriteLine($"Nenhuma tarefa encontrada com o ID {id}.");
            Pausar();
            return;
        }

        Console.WriteLine("Deixe em branco para manter o valor atual.\n");

        Console.Write($"Título [{tarefaExistente.Titulo}]: ");
        var titulo = Console.ReadLine();

        Console.Write($"Descrição [{tarefaExistente.Descricao}]: ");
        var descricao = Console.ReadLine();

        Console.Write($"Prioridade [{tarefaExistente.Prioridade}] (1-Baixa, 2-Média, 3-Alta, Enter para manter): ");
        var prioridadeEntrada = Console.ReadLine();
        Prioridade? prioridade = prioridadeEntrada switch
        {
            "1" => Prioridade.Baixa,
            "2" => Prioridade.Media,
            "3" => Prioridade.Alta,
            _ => null
        };

        var sucesso = _repositorio.Atualizar(id.Value, titulo, descricao, prioridade, null);
        Console.WriteLine(sucesso ? "\nTarefa atualizada com sucesso." : "\nNão foi possível atualizar a tarefa.");
        Pausar();
    }

    private void AlternarConclusao()
    {
        ExibirCabecalho("Marcar como concluída / pendente");

        var id = LerId();
        if (id is null) return;

        var tarefa = _repositorio.BuscarPorId(id.Value);
        if (tarefa is null)
        {
            Console.WriteLine($"Nenhuma tarefa encontrada com o ID {id}.");
            Pausar();
            return;
        }

        _repositorio.Atualizar(id.Value, null, null, null, !tarefa.Concluida);
        Console.WriteLine($"\nTarefa #{id} marcada como {(!tarefa.Concluida ? "pendente" : "concluída")}.");
        Pausar();
    }

    private void RemoverTarefa()
    {
        ExibirCabecalho("Remover tarefa");

        var id = LerId();
        if (id is null) return;

        Console.Write($"Tem certeza que deseja remover a tarefa #{id}? (s/n): ");
        var confirmacao = Console.ReadLine()?.Trim().ToLower();

        if (confirmacao != "s")
        {
            Console.WriteLine("Operação cancelada.");
            Pausar();
            return;
        }

        var sucesso = _repositorio.Remover(id.Value);
        Console.WriteLine(sucesso ? "\nTarefa removida com sucesso." : $"\nNenhuma tarefa encontrada com o ID {id}.");
        Pausar();
    }

    private static Prioridade LerPrioridade()
    {
        Console.Write("Prioridade (1-Baixa, 2-Média, 3-Alta) [padrão: Média]: ");
        return Console.ReadLine()?.Trim() switch
        {
            "1" => Prioridade.Baixa,
            "3" => Prioridade.Alta,
            _ => Prioridade.Media
        };
    }

    private int? LerId()
    {
        Console.Write("ID da tarefa: ");
        var entrada = Console.ReadLine();

        if (int.TryParse(entrada, out var id)) return id;

        Console.WriteLine("ID inválido.");
        Pausar();
        return null;
    }

    private static void Pausar()
    {
        Console.WriteLine("\nPressione ENTER para continuar...");
        Console.ReadLine();
    }

    private static void ExibirCabecalho(string? subtitulo = null)
    {
        Console.Clear();
        Console.WriteLine("=========================================");
        Console.WriteLine("           LISTA DE TAREFAS");
        Console.WriteLine("=========================================");
        if (subtitulo is not null)
        {
            Console.WriteLine($"-- {subtitulo} --\n");
        }
        else
        {
            Console.WriteLine();
        }
    }
}
