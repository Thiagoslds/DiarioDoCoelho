namespace DiarioDoCoelho.ViewModels;

/// <summary>
/// Modelo unificado para a página intermediária (Giro), usado tanto para
/// posts internos marcados como fonte externa quanto para notícias
/// obtidas diretamente pelo serviço de extração (sem persistência em banco).
/// </summary>
public class GiroLerViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public string? ImagemCapa { get; set; }
    public string FonteNoticiaUrl { get; set; } = string.Empty;
    public string? CategoriaNome { get; set; }
    public List<ProdutoAfiliadoViewModel> ProdutosLoja { get; set; } = new();
}
