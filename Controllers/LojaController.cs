using Microsoft.AspNetCore.Mvc;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Controllers;

public class LojaController : Controller
{
    // GET: /Loja
    public IActionResult Index()
    {
        return View(ProdutosAfiliadosMock.Produtos);
    }
}
