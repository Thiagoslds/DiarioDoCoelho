using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.ViewModels;

/// <summary>
/// Modelo para a listagem pública de notícias (/Post), combinando as notícias
/// internas cadastradas via Admin com as notícias externas obtidas em tempo real
/// pelo serviço de extração (substituindo o antigo conteúdo mockado).
/// </summary>
public class PostIndexViewModel
{
    public List<Post> Posts { get; set; } = new();
    public List<NoticiaCoelho> NoticiasExternas { get; set; } = new();
}
