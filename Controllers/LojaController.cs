using Microsoft.AspNetCore.Mvc;
using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Controllers;

public class LojaController : Controller
{
    // Vitrine estática de produtos afiliados (Shopee, Amazon, Centauro).
    // Futuramente pode ser migrada para uma tabela no banco de dados.
    private static readonly List<ProdutoAfiliadoViewModel> Produtos = new()
    {
        new ProdutoAfiliadoViewModel
        {
            Nome = "Manto I América 2024",
            ImagemUrl = "/images/loja/manto-1.jpg",
            Descricao = "Camisa oficial titular do América, tecido leve e respirável.",
            Loja = "Centauro",
            LinkAfiliado = "https://www.centauro.com.br/busca?q=camisa%20america%20mg"
        },
        new ProdutoAfiliadoViewModel
        {
            Nome = "Manto II América 2024",
            ImagemUrl = "/images/loja/manto-2.jpg",
            Descricao = "Segundo uniforme do Coelho, ideal para o dia a dia.",
            Loja = "Amazon",
            LinkAfiliado = "https://www.amazon.com.br/s?k=camisa+america+mineiro"
        },
        new ProdutoAfiliadoViewModel
        {
            Nome = "Boné do América",
            ImagemUrl = "/images/loja/bone.jpg",
            Descricao = "Boné oficial bordado com o escudo do Coelho.",
            Loja = "Shopee",
            LinkAfiliado = "https://shopee.com.br/search?keyword=bon%C3%A9%20america%20mineiro"
        },
        new ProdutoAfiliadoViewModel
        {
            Nome = "Caneca Coelho Campeão",
            ImagemUrl = "/images/loja/caneca.jpg",
            Descricao = "Caneca personalizada para o torcedor raiz do Coelho.",
            Loja = "Shopee",
            LinkAfiliado = "https://shopee.com.br/search?keyword=caneca%20america%20mineiro"
        }
    };

    // GET: /Loja
    public IActionResult Index()
    {
        return View(Produtos);
    }
}
