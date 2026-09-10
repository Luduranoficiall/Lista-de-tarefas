using ListaDeTarefas.Models;

namespace ListaDeTarefas.Repositories;

public interface ITarefaRepository
{
    Tarefa Criar(string titulo, string descricao, Prioridade prioridade);
    List<Tarefa> ListarTodas();
    Tarefa? BuscarPorId(int id);
    bool Atualizar(int id, string? titulo, string? descricao, Prioridade? prioridade, bool? concluida);
    bool Remover(int id);
}
