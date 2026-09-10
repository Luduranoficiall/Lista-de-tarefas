using ListaDeTarefas.Repositories;
using ListaDeTarefas.Services;
using ListaDeTarefas.UI;
using Microsoft.Extensions.DependencyInjection;

var servicos = new ServiceCollection();
servicos.AddSingleton<ITarefaRepository>(new TarefaRepository());
servicos.AddSingleton<ITarefaService, TarefaService>();
servicos.AddSingleton<Menu>();

using var provedor = servicos.BuildServiceProvider();
provedor.GetRequiredService<Menu>().Executar();
