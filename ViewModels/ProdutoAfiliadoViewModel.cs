namespace DiarioDoCoelho.ViewModels
{
    public class ProdutoAfiliadoViewModel
    {
        public string Nome { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Loja { get; set; } = string.Empty; // Shopee, Amazon, Centauro
        public string LinkAfiliado { get; set; } = string.Empty;
    }
}
