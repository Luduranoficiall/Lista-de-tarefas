using ListaDeTarefas.Exceptions;
using ListaDeTarefas.Models;
using ListaDeTarefas.Repositories;

namespace ListaDeTarefas.Services;

public class TarefaService : ITarefaService
{
    public const int TituloMaxLength = 100;
    public const int DescricaoMaxLength = 500;

    private readonly ITarefaRepository _repositorio;

    public TarefaService(ITarefaRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public Tarefa Criar(string titulo, string descricao, Prioridade prioridade)
    {
        ValidarTitulo(titulo);
        ValidarDescricao(descricao);

        return _repositorio.Criar(titulo.Trim(), descricao.Trim(), prioridade);
    }

    public IReadOnlyList<Tarefa> ListarTodas() => _repositorio.ListarTodas();

    public IReadOnlyList<Tarefa> Buscar(string? termo = null, bool? concluida = null, Prioridade? prioridade = null)
    {
        IEnumerable<Tarefa> tarefas = _repositorio.ListarTodas();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            tarefas = tarefas.Where(t =>
                t.Titulo.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                t.Descricao.Contains(termo, StringComparison.OrdinalIgnoreCase));
        }

        if (concluida.HasValue)
            tarefas = tarefas.Where(t => t.Concluida == concluida.Value);

        if (prioridade.HasValue)
            tarefas = tarefas.Where(t => t.Prioridade == prioridade.Value);

        return tarefas.ToList();
    }

    public Tarefa BuscarPorId(int id)
    {
        return _repositorio.BuscarPorId(id) ?? throw new TarefaNaoEncontradaException(id);
    }

    public Tarefa Atualizar(int id, string? titulo, string? descricao, Prioridade? prioridade)
    {
        if (titulo is not null) ValidarTitulo(titulo);
        if (descricao is not null) ValidarDescricao(descricao);

        var sucesso = _repositorio.Atualizar(id, titulo?.Trim(), descricao?.Trim(), prioridade, null);
        if (!sucesso) throw new TarefaNaoEncontradaException(id);

        return BuscarPorId(id);
    }

    public Tarefa AlternarConclusao(int id)
    {
        var tarefa = BuscarPorId(id);
        _repositorio.Atualizar(id, null, null, null, !tarefa.Concluida);
        return BuscarPorId(id);
    }

    public void Remover(int id)
    {
        if (!_repositorio.Remover(id))
            throw new TarefaNaoEncontradaException(id);
    }

    private static void ValidarTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new TarefaInvalidaException("O título da tarefa é obrigatório.");

        if (titulo.Trim().Length > TituloMaxLength)
            throw new TarefaInvalidaException($"O título não pode ter mais que {TituloMaxLength} caracteres.");
    }

    private static void ValidarDescricao(string descricao)
    {
        if (descricao.Length > DescricaoMaxLength)
            throw new TarefaInvalidaException($"A descrição não pode ter mais que {DescricaoMaxLength} caracteres.");
    }
}
