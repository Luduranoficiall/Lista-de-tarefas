namespace ListaDeTarefas.Exceptions;

/// <summary>
/// Lançada quando os dados informados para criar ou atualizar uma tarefa violam
/// as regras de negócio (ex.: título vazio ou maior que o limite permitido).
/// </summary>
public class TarefaInvalidaException : Exception
{
    public TarefaInvalidaException(string mensagem) : base(mensagem)
    {
    }
}
