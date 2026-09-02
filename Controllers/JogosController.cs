using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;

namespace DiarioDoCoelho.Controllers;

public class JogosController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    // GET: /Jogos/Detalhes/5
    public async Task<IActionResult> Detalhes(int id)
    {
        var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.Id == id);

        if (jogo is null)
        {
            return NotFound();
        }

        return View(jogo);
    }
}
