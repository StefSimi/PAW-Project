namespace Laborator09.Models;

public class ArticolHistoryModel
{
    public int Id { get; set; }
    public string Titlu { get; set; }

    public string Continut { get; set; }

    public DateTime DataAdaugare { get; set; }

    public bool Restrictionat { get; set; }

    public int Versiune { get; set; }

    public int ArticolId {  get; set; }

    public ArticolModel? Articol { get; set; }
}
