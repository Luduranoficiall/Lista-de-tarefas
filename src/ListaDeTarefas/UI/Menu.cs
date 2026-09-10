using ListaDeTarefas.Exceptions;
using ListaDeTarefas.Models;
using ListaDeTarefas.Services;

namespace ListaDeTarefas.UI;

public class Menu
{
    private readonly ITarefaService _servico;

    public Menu(ITarefaService servico)
    {
        _servico = servico;
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
            Console.WriteLine("4 - Buscar/filtrar tarefas");
            Console.WriteLine("5 - Atualizar tarefa");
            Console.WriteLine("6 - Marcar como concluída / pendente");
            Console.WriteLine("7 - Remover tarefa");
            Console.WriteLine("0 - Sair");
            Console.Write("\nEscolha uma opção: ");

            try
            {
                switch (Console.ReadLine()?.Trim())
                {
                    case "1": CriarTarefa(); break;
                    case "2": ListarTarefas(); break;
                    case "3": BuscarPorId(); break;
                    case "4": BuscarComFiltros(); break;
                    case "5": AtualizarTarefa(); break;
                    case "6": AlternarConclusao(); break;
                    case "7": RemoverTarefa(); break;
                    case "0": executando = false; break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        Pausar();
                        break;
                }
            }
            catch (TarefaNaoEncontradaException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
                Pausar();
            }
            catch (TarefaInvalidaException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
                Pausar();
            }
        }

        Console.WriteLine("Até logo!");
    }

    private void CriarTarefa()
    {
        ExibirCabecalho("Nova tarefa");

        Console.Write("Título: ");
        var titulo = Console.ReadLine() ?? string.Empty;

        Console.Write("Descrição: ");
        var descricao = Console.ReadLine() ?? string.Empty;

        var prioridade = LerPrioridade();

        var tarefa = _servico.Criar(titulo, descricao, prioridade);
        Console.WriteLine($"\nTarefa #{tarefa.Id} criada com sucesso.");
        Pausar();
    }

    private void ListarTarefas()
    {
        ExibirCabecalho("Tarefas do dia");
        ExibirLista(_servico.ListarTodas());
        Pausar();
    }

    private void BuscarPorId()
    {
        ExibirCabecalho("Buscar tarefa por ID");

        var id = LerId();
        if (id is null) return;

        var tarefa = _servico.BuscarPorId(id.Value);

        Console.WriteLine(tarefa);
        Console.WriteLine($"Descrição: {tarefa.Descricao}");
        if (tarefa.ConcluidaEm.HasValue)
            Console.WriteLine($"Concluída em: {tarefa.ConcluidaEm:dd/MM/yyyy HH:mm}");

        Pausar();
    }

    private void BuscarComFiltros()
    {
        ExibirCabecalho("Buscar/filtrar tarefas");

        Console.Write("Texto (título/descrição, Enter para ignorar): ");
        var termo = Console.ReadLine();

        Console.Write("Status (1-Pendentes, 2-Concluídas, Enter para todas): ");
        bool? concluida = Console.ReadLine()?.Trim() switch
        {
            "1" => false,
            "2" => true,
            _ => null
        };

        Console.Write("Prioridade (1-Baixa, 2-Média, 3-Alta, Enter para todas): ");
        Prioridade? prioridade = Console.ReadLine()?.Trim() switch
        {
            "1" => Prioridade.Baixa,
            "2" => Prioridade.Media,
            "3" => Prioridade.Alta,
            _ => null
        };

        var resultado = _servico.Buscar(termo, concluida, prioridade);
        Console.WriteLine();
        ExibirLista(resultado);
        Pausar();
    }

    private void AtualizarTarefa()
    {
        ExibirCabecalho("Atualizar tarefa");

        var id = LerId();
        if (id is null) return;

        var tarefaExistente = _servico.BuscarPorId(id.Value);

        Console.WriteLine("Deixe em branco para manter o valor atual.\n");

        Console.Write($"Título [{tarefaExistente.Titulo}]: ");
        var titulo = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(titulo)) titulo = null;

        Console.Write($"Descrição [{tarefaExistente.Descricao}]: ");
        var descricao = Console.ReadLine();
        if (string.IsNullOrEmpty(descricao)) descricao = null;

        Console.Write($"Prioridade [{tarefaExistente.Prioridade}] (1-Baixa, 2-Média, 3-Alta, Enter para manter): ");
        Prioridade? prioridade = Console.ReadLine()?.Trim() switch
        {
            "1" => Prioridade.Baixa,
            "2" => Prioridade.Media,
            "3" => Prioridade.Alta,
            _ => null
        };

        _servico.Atualizar(id.Value, titulo, descricao, prioridade);
        Console.WriteLine("\nTarefa atualizada com sucesso.");
        Pausar();
    }

    private void AlternarConclusao()
    {
        ExibirCabecalho("Marcar como concluída / pendente");

        var id = LerId();
        if (id is null) return;

        var tarefa = _servico.AlternarConclusao(id.Value);
        Console.WriteLine($"\nTarefa #{tarefa.Id} marcada como {(tarefa.Concluida ? "concluída" : "pendente")}.");
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

        _servico.Remover(id.Value);
        Console.WriteLine("\nTarefa removida com sucesso.");
        Pausar();
    }

    private static void ExibirLista(IReadOnlyList<Tarefa> tarefas)
    {
        if (tarefas.Count == 0)
        {
            Console.WriteLine("Nenhuma tarefa encontrada.");
            return;
        }

        foreach (var tarefa in tarefas)
            Console.WriteLine(tarefa);
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
