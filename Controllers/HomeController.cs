using Laborator09.ContextModels;
using Laborator09.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Laborator09.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ArticoleContext _context;
        public HomeController(ILogger<HomeController> logger, ArticoleContext context)
        {
            _logger = logger;
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categorie.ToListAsync();
            var latestArticles = new List<dynamic>();

            foreach (var category in categories)
            {
                var latestArticle = await _context.Articol
                    .Where(a => a.CategorieId == category.Id)
                    .OrderByDescending(a => a.DataAdaugare)
                    .FirstOrDefaultAsync();

                if (latestArticle != null)
                {
                    latestArticles.Add(new
                    {
                        CategoryName = category.Nume,
                        ArticleTitle = latestArticle.Titlu,
                        ArticleContent = latestArticle.Continut,
                        ArticleDate = latestArticle.DataAdaugare,
                        Restrictionat =latestArticle.Restrictionat,
                        Id=latestArticle.Id,
                        AutorId=latestArticle.AutorId
                    });
                }
            }

            return View(latestArticles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




    }
}
