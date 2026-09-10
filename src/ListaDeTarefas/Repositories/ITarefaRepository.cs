using ListaDeTarefas.Models;

namespace ListaDeTarefas.Repositories;

/// <summary>
/// Acesso a dados de tarefas. Não valida regras de negócio — isso é responsabilidade
/// da camada de serviço; a implementação apenas persiste e recupera o estado.
/// </summary>
public interface ITarefaRepository
{
    Tarefa Criar(string titulo, string descricao, Prioridade prioridade);
    List<Tarefa> ListarTodas();
    Tarefa? BuscarPorId(int id);
    bool Atualizar(int id, string? titulo, string? descricao, Prioridade? prioridade, bool? concluida);
    bool Remover(int id);
}
