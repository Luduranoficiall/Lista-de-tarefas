namespace ListaDeTarefas.Models;

public enum Prioridade
{
    Baixa,
    Media,
    Alta
}

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public Prioridade Prioridade { get; set; } = Prioridade.Media;
    public bool Concluida { get; set; }
    public DateTime CriadaEm { get; set; } = DateTime.Now;
    public DateTime? ConcluidaEm { get; set; }

    public override string ToString()
    {
        var status = Concluida ? "[X]" : "[ ]";
        return $"{status} #{Id} - {Titulo} ({Prioridade}) - criada em {CriadaEm:dd/MM/yyyy HH:mm}";
    }
}
