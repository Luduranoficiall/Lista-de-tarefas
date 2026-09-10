using ListaDeTarefas.Exceptions;
using ListaDeTarefas.Models;
using ListaDeTarefas.Repositories;
using ListaDeTarefas.Services;
using Xunit;

namespace ListaDeTarefas.Tests;

public class TarefaServiceTests
{
    private static ITarefaService CriarServico()
    {
        var caminho = Path.Combine(Path.GetTempPath(), $"tarefas-{Guid.NewGuid()}.json");
        ITarefaRepository repositorio = new TarefaRepository(caminho);
        return new TarefaService(repositorio);
    }

    [Fact]
    public void Criar_ComDadosValidos_RetornaTarefaComId()
    {
        var servico = CriarServico();

        var tarefa = servico.Criar("Estudar C#", "Revisar CRUD", Prioridade.Alta);

        Assert.True(tarefa.Id > 0);
        Assert.Equal("Estudar C#", tarefa.Titulo);
        Assert.False(tarefa.Concluida);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComTituloVazio_LancaTarefaInvalidaException(string? titulo)
    {
        var servico = CriarServico();

        Assert.Throws<TarefaInvalidaException>(() => servico.Criar(titulo!, "descrição", Prioridade.Media));
    }

    [Fact]
    public void Criar_ComTituloMuitoLongo_LancaTarefaInvalidaException()
    {
        var servico = CriarServico();
        var tituloLongo = new string('a', TarefaService.TituloMaxLength + 1);

        Assert.Throws<TarefaInvalidaException>(() => servico.Criar(tituloLongo, "descrição", Prioridade.Media));
    }

    [Fact]
    public void BuscarPorId_ComIdInexistente_LancaTarefaNaoEncontradaException()
    {
        var servico = CriarServico();

        var excecao = Assert.Throws<TarefaNaoEncontradaException>(() => servico.BuscarPorId(999));
        Assert.Equal(999, excecao.Id);
    }

    [Fact]
    public void Atualizar_AlteraApenasCamposInformados()
    {
        var servico = CriarServico();
        var tarefa = servico.Criar("Título original", "Descrição original", Prioridade.Baixa);

        var atualizada = servico.Atualizar(tarefa.Id, "Novo título", null, null);

        Assert.Equal("Novo título", atualizada.Titulo);
        Assert.Equal("Descrição original", atualizada.Descricao);
        Assert.Equal(Prioridade.Baixa, atualizada.Prioridade);
    }

    [Fact]
    public void AlternarConclusao_MarcaEDesmarcaTarefa()
    {
        var servico = CriarServico();
        var tarefa = servico.Criar("Tarefa", "desc", Prioridade.Media);

        var concluida = servico.AlternarConclusao(tarefa.Id);
        Assert.True(concluida.Concluida);
        Assert.NotNull(concluida.ConcluidaEm);

        var pendenteNovamente = servico.AlternarConclusao(tarefa.Id);
        Assert.False(pendenteNovamente.Concluida);
        Assert.Null(pendenteNovamente.ConcluidaEm);
    }

    [Fact]
    public void Remover_ComIdExistente_RemoveDaLista()
    {
        var servico = CriarServico();
        var tarefa = servico.Criar("Tarefa a remover", "desc", Prioridade.Media);

        servico.Remover(tarefa.Id);

        Assert.Throws<TarefaNaoEncontradaException>(() => servico.BuscarPorId(tarefa.Id));
    }

    [Fact]
    public void Remover_ComIdInexistente_LancaTarefaNaoEncontradaException()
    {
        var servico = CriarServico();

        Assert.Throws<TarefaNaoEncontradaException>(() => servico.Remover(123));
    }

    [Fact]
    public void Buscar_FiltraPorTextoStatusEPrioridade()
    {
        var servico = CriarServico();
        servico.Criar("Comprar leite", "no mercado", Prioridade.Baixa);
        var urgente = servico.Criar("Enviar relatório", "para o chefe", Prioridade.Alta);
        servico.AlternarConclusao(urgente.Id);

        var resultado = servico.Buscar(termo: "relatório", concluida: true, prioridade: Prioridade.Alta);

        Assert.Single(resultado);
        Assert.Equal(urgente.Id, resultado[0].Id);
    }
}
