using Laborator09.ContextModels;
using Laborator09.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Laborator09.Controllers;

public class ArticolController : Controller
{
    private readonly ArticoleContext _context;

    //private readonly UserManager <IdentityUser> _userManager;

    public List<ArticolModel>? Articole { get; set; }
    public ArticolModel? ArticolCurent { get; set; }


    public ArticolController(ArticoleContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        Articole = _context.Articol.Include(articol => articol.Categorie).ToList();
        if (Articole == null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(Articole);
        //return View("Index", Stiri);
    }


    [HttpGet]
    public IActionResult Articol(int articolId)
    {
        ArticolCurent = _context.Articol
            .Where(articol => articol.Id == articolId).Include(articol => articol.Categorie).FirstOrDefault();
        if (ArticolCurent == null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(ArticolCurent);
    }




    [HttpGet]
    public IActionResult AdaugaArticol()
    {
        List<SelectListItem> categorii = _context.Categorie
            .Select(categorie => new SelectListItem { Text = categorie.Nume, Value = categorie.Id.ToString() })
            .ToList();
        List<SelectListItem> autori = _context.User
            .Select(autor => new SelectListItem { Text = autor.Username, Value = autor.Id.ToString() })
            .ToList();
        ViewBag.Categorii = categorii;
        ViewBag.Autori = autori;
        return View();
    }

    [HttpPost]
    public IActionResult AdaugaArticol(ArticolModel articolNou)
    {
        if (!ModelState.IsValid)
        {
            List<SelectListItem> categorii = _context.Categorie
            .Select(categorie => new SelectListItem { Text = categorie.Nume, Value = categorie.Id.ToString() })
            .ToList();
            ViewBag.Categorii = categorii;
            return View(articolNou);
        }


        string userIdClaim = User?.Claims?.FirstOrDefault(claim => claim.Type == "Id")?.Value ?? "1";



        articolNou.Categorie = _context.Categorie.Where(categorie => categorie.Id == articolNou.CategorieId).FirstOrDefault();
        articolNou.Autor = _context.User.Where(autor => autor.Id == articolNou.Id).FirstOrDefault();
        articolNou.Versiune = 1;
        articolNou.Restrictionat = false;
        articolNou.AutorId = int.Parse(userIdClaim!);



        Console.WriteLine(userIdClaim);
        _context.Add(articolNou);

        _context.SaveChanges();
        ArticolHistoryModel ArticolBackup = new ArticolHistoryModel
        {
            Titlu = articolNou.Titlu,
            Continut = articolNou.Continut,
            DataAdaugare = articolNou.DataAdaugare,
            Restrictionat = articolNou.Restrictionat,
            Versiune = articolNou.Versiune,
            ArticolId = articolNou.Id

        };
        _context.Add(ArticolBackup);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }


    //-----------
    [HttpGet]
    public IActionResult ModificaArticol(int articolId)
    {
        List<SelectListItem> categorii = _context.Categorie
            .Select(categorie => new SelectListItem { Text = categorie.Nume, Value = categorie.Id.ToString() })
            .ToList();
        ViewBag.Categorii = categorii;

        List<SelectListItem> autori = _context.User
            .Select(autor => new SelectListItem { Text = autor.Username, Value = autor.Id.ToString() })
            .ToList();

        ArticolModel? articol = _context.Articol
            .Where(articol => articol.Id == articolId).Include(articol => articol.Categorie).FirstOrDefault();

        if (articol == null)
        {
            return RedirectToAction("Error", "Home");
        }
        return View(articol);
    }

    [HttpPost]
    public IActionResult ModificaArticol(ArticolModel articol)
    {
        if (!ModelState.IsValid)
        {
            List<SelectListItem> categorii = _context.Categorie
            .Select(categorie => new SelectListItem { Text = categorie.Nume, Value = categorie.Id.ToString() })
            .ToList();
            ViewBag.Categorii = categorii;
            return View(articol);
        }
        articol.Categorie = _context.Categorie.Where(categorie => categorie.Id == articol.CategorieId).FirstOrDefault();



        string userIdClaim = User?.Claims?.FirstOrDefault(claim => claim.Type == "Id")?.Value ?? "1";
        articol.AutorId = int.Parse(userIdClaim!);
        //articol.Versiune++;


        var v = _context.ArticolHistory.Where(art => art.ArticolId == articol.Id).OrderBy(x => x.Versiune).LastOrDefault();
        articol.Versiune = v.Versiune + 1;



        ArticolHistoryModel ArticolNou = new ArticolHistoryModel
        {
            Titlu = articol.Titlu,
            Continut = articol.Continut,
            DataAdaugare = articol.DataAdaugare,
            Restrictionat = articol.Restrictionat,
            Versiune = articol.Versiune,
            ArticolId = articol.Id

        };
        _context.Add(ArticolNou);

        _context.Update(articol);
        _context.SaveChanges();
        return View("Articol", articol);
    }

    [HttpPost]
    public IActionResult Restrict(int articolId)
    {
        // Retrieve the Articol from the database based on the articolId
        var articol = _context.Articol.FirstOrDefault(a => a.Id == articolId);

        if (articol == null)
        {
            return NotFound(); // Handle the case where the articol is not found
        }

        // Toggle the value of Restrictionat
        articol.Restrictionat = !articol.Restrictionat;

        // Update the Articol in the database
        _context.Update(articol);
        _context.SaveChanges();

        // Redirect to an appropriate action, such as the Index action
        return View("Articol", articol);
    }

    [HttpPost]
    public IActionResult Rollback(int articolId)
    {
        var articol = _context.Articol.FirstOrDefault(a => a.Id == articolId);
        if (articol == null)
        {
            return NotFound(); // Handle the case where the articol is not found
        }
        var v = _context.ArticolHistory.Where(art => art.ArticolId == articol.Id && art.Versiune == articol.Versiune-1).FirstOrDefault();
        Console.WriteLine(articol.Versiune);

        if (v == null)
        {
            Console.WriteLine("FF");
            return NotFound();
        }
        articol.Titlu = v.Titlu;
        articol.Continut = v.Continut;
        articol.DataAdaugare = v.DataAdaugare;
        articol.Versiune=v.Versiune;

        var versionToDelete= _context.ArticolHistory.Where(art => art.ArticolId == v.ArticolId && art.Versiune > v.Versiune).FirstOrDefault();
        while (versionToDelete != null)
        {
            _context.Remove(versionToDelete);
            _context.SaveChanges();
            versionToDelete = _context.ArticolHistory.Where(art => art.ArticolId == v.ArticolId && art.Versiune > v.Versiune).FirstOrDefault();
        }
        ArticolHistoryModel ArticolNou = new ArticolHistoryModel
        {
            Titlu = articol.Titlu,
            Continut = articol.Continut,
            DataAdaugare = articol.DataAdaugare,
            Restrictionat = articol.Restrictionat,
            Versiune = articol.Versiune,
            ArticolId = articol.Id

        };
        _context.Add(ArticolNou);

        _context.Update(articol);
        _context.SaveChanges();
        return View("Articol", articol);
    }



    public IActionResult ByCategory(int categoryId, string sortOrder)
    {
        var articles = _context.Articol
            .Where(a => a.CategorieId == categoryId)
            .AsQueryable();

        ViewBag.TitleSortParam = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
        ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
        ViewBag.CategoryId = categoryId;

        switch (sortOrder)
        {
            case "title_desc":
                articles = articles.OrderByDescending(a => a.Titlu);
                break;
            case "Date":
                articles = articles.OrderBy(a => a.DataAdaugare);
                break;
            case "date_desc":
                articles = articles.OrderByDescending(a => a.DataAdaugare);
                break;
            default:
                articles = articles.OrderBy(a => a.Titlu);
                break;
        }

        var category = _context.Categorie
            .FirstOrDefault(c => c.Id == categoryId);

        ViewBag.Category = category?.Nume;
        return View("ArticlesByCategory", articles.ToList());
    }


    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = Url.Content("~/uploads/" + file.FileName);
        return Json(new { location = fileUrl });
    }


}
