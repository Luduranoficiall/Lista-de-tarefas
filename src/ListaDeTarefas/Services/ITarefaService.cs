using ListaDeTarefas.Exceptions;
using ListaDeTarefas.Models;

namespace ListaDeTarefas.Services;

/// <summary>
/// Regras de negócio para tarefas: valida os dados de entrada e traduz falhas
/// em exceções de domínio antes de delegar a persistência ao repositório.
/// </summary>
public interface ITarefaService
{
    /// <exception cref="TarefaInvalidaException">Título vazio ou dados fora dos limites permitidos.</exception>
    Tarefa Criar(string titulo, string descricao, Prioridade prioridade);

    IReadOnlyList<Tarefa> ListarTodas();

    /// <summary>Filtra tarefas por texto (título/descrição), status de conclusão e/ou prioridade.</summary>
    IReadOnlyList<Tarefa> Buscar(string? termo = null, bool? concluida = null, Prioridade? prioridade = null);

    /// <exception cref="TarefaNaoEncontradaException">Nenhuma tarefa com o ID informado.</exception>
    Tarefa BuscarPorId(int id);

    /// <exception cref="TarefaNaoEncontradaException">Nenhuma tarefa com o ID informado.</exception>
    /// <exception cref="TarefaInvalidaException">Dados fora dos limites permitidos.</exception>
    Tarefa Atualizar(int id, string? titulo, string? descricao, Prioridade? prioridade);

    /// <exception cref="TarefaNaoEncontradaException">Nenhuma tarefa com o ID informado.</exception>
    Tarefa AlternarConclusao(int id);

    /// <exception cref="TarefaNaoEncontradaException">Nenhuma tarefa com o ID informado.</exception>
    void Remover(int id);
}
