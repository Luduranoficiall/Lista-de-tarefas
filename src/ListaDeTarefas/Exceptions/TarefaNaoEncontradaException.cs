namespace ListaDeTarefas.Exceptions;

/// <summary>
/// Lançada quando uma operação referencia um ID de tarefa que não existe no repositório.
/// </summary>
public class TarefaNaoEncontradaException : Exception
{
    public int Id { get; }

    public TarefaNaoEncontradaException(int id)
        : base($"Nenhuma tarefa encontrada com o ID {id}.")
    {
        Id = id;
    }
}
