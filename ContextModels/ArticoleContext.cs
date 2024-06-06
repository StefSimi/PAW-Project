using Laborator09.Models;
using Microsoft.EntityFrameworkCore;

namespace Laborator09.ContextModels;

public class ArticoleContext : DbContext
{
    public ArticoleContext(DbContextOptions<ArticoleContext> options) : base(options)
    {
    }
    public DbSet<CategorieModel> Categorie { get; set; }
    public DbSet<UserModel> User { get; set; }

    public DbSet<ArticolModel> Articol { get; set; }

    public DbSet<ArticolHistoryModel> ArticolHistory { get; set; }

    

}
