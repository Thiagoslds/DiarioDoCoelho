using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Admin/Home
        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalPosts = await _context.Posts.CountAsync(),
                TotalCategorias = await _context.Categorias.CountAsync(),
                TotalJogos = await _context.Jogos.CountAsync(),
                TotalBanners = await _context.BannersAfiliados.CountAsync(),
                UltimosPosts = await _context.Posts
                    .Include(p => p.Categoria)
                    .OrderByDescending(p => p.DataPublicacao)
                    .Take(5)
                    .ToListAsync(),
                ProximoJogo = await _context.Jogos
                    .Where(j => j.DataHora >= DateTime.Now)
                    .OrderBy(j => j.DataHora)
                    .FirstOrDefaultAsync()
            };

            return View(viewModel);
        }
    }
}
