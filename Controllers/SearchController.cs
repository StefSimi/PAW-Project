using Laborator09.ContextModels;
using Laborator09.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Laborator09.Controllers;

public class SearchController : Controller
{
    private readonly ArticoleContext _context;

    public SearchController(ArticoleContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult SearchPortal()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SearchPortal(string? titluArticol, DateTime? dataInf, DateTime? dataSup)
    {
        IQueryable<ArticolModel> articole = _context.Articol;
        
        if (titluArticol != null) {
            articole = articole.Where(articole => articole.Titlu.ToLower().Contains(titluArticol.ToLower()));
        }
        if (dataInf != null) {
            articole = articole.Where(articole => articole.DataAdaugare >= dataInf);
        }
        if (dataSup != null) {
            articole = articole.Where(articole => articole.DataAdaugare <= dataSup);
        }

        ViewBag.titluArticol = titluArticol;
        ViewBag.DataInf = dataInf;
        ViewBag.DataSup = dataSup;
        return View(articole.ToList());
    }
}
