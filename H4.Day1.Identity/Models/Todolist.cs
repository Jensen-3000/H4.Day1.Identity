namespace H4.Day1.Identity.Models;

public class Todolist
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Item { get; set; } = null!;

    public virtual Cpr User { get; set; } = null!;
}
