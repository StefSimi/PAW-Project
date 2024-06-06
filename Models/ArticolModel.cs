namespace Laborator09.Models;

public class ArticolModel
{
    public int Id { get; set; }
    public string Titlu { get; set; }

    public string Continut { get; set; }

    public DateTime DataAdaugare { get; set; }

    public bool Restrictionat { get; set; }

    public int Versiune {  get; set; }

    public int CategorieId { get; set; }

    public CategorieModel? Categorie { get; set; }

    public int AutorId { get; set; }

    public UserModel? Autor { get; set; }
}
