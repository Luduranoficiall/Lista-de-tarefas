using ListaDeTarefas.Repositories;
using ListaDeTarefas.UI;

ITarefaRepository repositorio = new TarefaRepository();
var menu = new Menu(repositorio);
menu.Executar();
