using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Laborator09.Models;
using Laborator09.Logic;
using Laborator09.ContextModels;

public class AdminController : Controller
{
    private readonly ArticoleContext _context;

    public AdminController(ArticoleContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> AdminPanel()
    {
        var users = await _context.User.ToListAsync();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeUserRole(int userId, UserType newRole)
    {
        var user = await _context.User.FindAsync(userId);
        if (user != null)
        {
            user.Role = newRole;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(AdminPanel));
    }
}
