using ListaDeTarefas.Models;
using ListaDeTarefas.Repositories;
using Xunit;

namespace ListaDeTarefas.Tests;

public class TarefaRepositoryTests : IDisposable
{
    private readonly string _caminho = Path.Combine(Path.GetTempPath(), $"tarefas-{Guid.NewGuid()}.json");

    [Fact]
    public void Criar_PersisteERetornaComIdIncremental()
    {
        var repositorio = new TarefaRepository(_caminho);

        var primeira = repositorio.Criar("Tarefa 1", "desc", Prioridade.Media);
        var segunda = repositorio.Criar("Tarefa 2", "desc", Prioridade.Media);

        Assert.Equal(1, primeira.Id);
        Assert.Equal(2, segunda.Id);
        Assert.True(File.Exists(_caminho));
    }

    [Fact]
    public void NovaInstancia_CarregaDadosDoArquivoExistente_ESeguemIncrementandoId()
    {
        var repositorio1 = new TarefaRepository(_caminho);
        repositorio1.Criar("Tarefa 1", "desc", Prioridade.Media);

        var repositorio2 = new TarefaRepository(_caminho);
        var novaTarefa = repositorio2.Criar("Tarefa 2", "desc", Prioridade.Media);

        Assert.Equal(2, repositorio2.ListarTodas().Count);
        Assert.Equal(2, novaTarefa.Id);
    }

    [Fact]
    public void BuscarPorId_ComIdInexistente_RetornaNull()
    {
        var repositorio = new TarefaRepository(_caminho);

        Assert.Null(repositorio.BuscarPorId(42));
    }

    [Fact]
    public void Atualizar_ComIdInexistente_RetornaFalse()
    {
        var repositorio = new TarefaRepository(_caminho);

        Assert.False(repositorio.Atualizar(42, "novo", null, null, null));
    }

    [Fact]
    public void Remover_ComIdExistente_RetornaTrueERemoveDaLista()
    {
        var repositorio = new TarefaRepository(_caminho);
        var tarefa = repositorio.Criar("Tarefa", "desc", Prioridade.Media);

        var sucesso = repositorio.Remover(tarefa.Id);

        Assert.True(sucesso);
        Assert.Null(repositorio.BuscarPorId(tarefa.Id));
    }

    public void Dispose()
    {
        if (File.Exists(_caminho)) File.Delete(_caminho);
    }
}
