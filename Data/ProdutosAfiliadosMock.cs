using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Data;

/// <summary>
/// Vitrine estática de produtos afiliados (Shopee, Amazon, Centauro) reutilizada
/// na Loja do Coelho e no carrossel da Home. Futuramente pode ser migrada para
/// uma tabela no banco de dados administrada pelo Admin.
/// </summary>
public static class ProdutosAfiliadosMock
{
    public static readonly List<ProdutoAfiliadoViewModel> Produtos = new()
    {
        new ProdutoAfiliadoViewModel
        {
            Nome = "BANDEIRA AMÉRICA 2 PANOS 0,90 X 1,30M",
            ImagemUrl = "/img/bandeira-america.jpg",
            Descricao = "Bandeira Oficial do América, produzidas de acordo com as normas técnicas, seguindo padrões e medidas oficiais utilizando tecido 100% Poliéster.",
            Loja = "Amazon",
            LinkAfiliado = "https://link.amazon/B0dxG1gua"
        },
        new ProdutoAfiliadoViewModel
        {
            Nome = "Camisa América Treino Purple 2025",
            ImagemUrl = "/img/camisa-treino-1.webp",
            Descricao = "Camisa Oficial América Futebol Clube - MG - Temporada 2025 - TREINO - ROXA",
            Loja = "Mercado Livre",
            LinkAfiliado = "https://meli.la/1taqCQo"
        },
        new ProdutoAfiliadoViewModel
        {
            Nome = "Camisa América Retrô 1971 Verde e Preta",
            ImagemUrl = "/img/camisa-retro1.jpg",
            Descricao = "A camisa retrô do América, foi inspirada no modelo usado pelos atletas do Coelho em 1971.",
            Loja = "Amazon",
            LinkAfiliado = "https://link.amazon/B0caOZVVp"
        },
        new ProdutoAfiliadoViewModel
        {
            Nome = "Camisa América Volt 2025 Aquece Masculino",
            ImagemUrl = "/img/camisa-treino-2.webp",
            Descricao = "Produzida com fibras de garrafas PET recicladas e malha sem tingimento.",
            Loja = "Mercado Livre",
            LinkAfiliado = "https://meli.la/2NZ2G9i"
        },
        new ProdutoAfiliadoViewModel
        {
            Nome = "Camisa Goleiro América I 2024/25",
            ImagemUrl = "/img/camisa-goleiro-1.webp",
            Descricao = "Camisa Oficial América Futebol Clube - MG - Temporada 2024/25 - GOLEIRO JOGO I - LARANJA.",
            Loja = "Mercado Livre",
            LinkAfiliado = "https://meli.la/2UU8T52"
        },
        new ProdutoAfiliadoViewModel
        {
            Nome = "Camisa América Vintage Verde",
            ImagemUrl = "/img/camisa-retro2.webp",
            Descricao = "Camisa América Vintage Verde 100% Algodão",
            Loja = "Mercado Livre",
            LinkAfiliado = "https://meli.la/2geH9ju"
        }
    };
}
