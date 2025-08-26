namespace H4.Day1.Identity.Models;

public class Cpr
{
    public int Id { get; set; }

    public string User { get; set; } = null!;

    public string CprNr { get; set; } = null!;

    public virtual Todolist? Todolist { get; set; }
}
