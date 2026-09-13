using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalPosts { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalProdutos { get; set; }
        public int TotalBanners { get; set; }
        public List<Post> UltimosPosts { get; set; } = new();
    }
}
