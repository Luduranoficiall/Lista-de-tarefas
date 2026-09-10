using System.Text.Json;
using ListaDeTarefas.Models;

namespace ListaDeTarefas.Repositories;

public class TarefaRepository : ITarefaRepository
{
    private readonly string _caminhoArquivo;
    private List<Tarefa> _tarefas;
    private int _proximoId;

    public TarefaRepository(string caminhoArquivo = "tarefas.json")
    {
        _caminhoArquivo = caminhoArquivo;
        _tarefas = CarregarDoArquivo();
        _proximoId = _tarefas.Count > 0 ? _tarefas.Max(t => t.Id) + 1 : 1;
    }

    public Tarefa Criar(string titulo, string descricao, Prioridade prioridade)
    {
        var tarefa = new Tarefa
        {
            Id = _proximoId++,
            Titulo = titulo,
            Descricao = descricao,
            Prioridade = prioridade
        };

        _tarefas.Add(tarefa);
        Salvar();
        return tarefa;
    }

    public List<Tarefa> ListarTodas()
    {
        return _tarefas.OrderBy(t => t.Concluida).ThenByDescending(t => t.Prioridade).ThenBy(t => t.Id).ToList();
    }

    public Tarefa? BuscarPorId(int id)
    {
        return _tarefas.FirstOrDefault(t => t.Id == id);
    }

    public bool Atualizar(int id, string? titulo, string? descricao, Prioridade? prioridade, bool? concluida)
    {
        var tarefa = BuscarPorId(id);
        if (tarefa is null) return false;

        if (!string.IsNullOrWhiteSpace(titulo)) tarefa.Titulo = titulo;
        if (!string.IsNullOrWhiteSpace(descricao)) tarefa.Descricao = descricao;
        if (prioridade.HasValue) tarefa.Prioridade = prioridade.Value;
        if (concluida.HasValue)
        {
            tarefa.Concluida = concluida.Value;
            tarefa.ConcluidaEm = concluida.Value ? DateTime.Now : null;
        }

        Salvar();
        return true;
    }

    public bool Remover(int id)
    {
        var tarefa = BuscarPorId(id);
        if (tarefa is null) return false;

        _tarefas.Remove(tarefa);
        Salvar();
        return true;
    }

    private List<Tarefa> CarregarDoArquivo()
    {
        if (!File.Exists(_caminhoArquivo)) return new List<Tarefa>();

        var json = File.ReadAllText(_caminhoArquivo);
        if (string.IsNullOrWhiteSpace(json)) return new List<Tarefa>();

        return JsonSerializer.Deserialize<List<Tarefa>>(json) ?? new List<Tarefa>();
    }

    private void Salvar()
    {
        var json = JsonSerializer.Serialize(_tarefas, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_caminhoArquivo, json);
    }
}
