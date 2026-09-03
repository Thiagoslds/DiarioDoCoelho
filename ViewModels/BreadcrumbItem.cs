namespace DiarioDoCoelho.ViewModels;

/// <summary>
/// Representa um item da trilha de navegação (breadcrumb) exibida no topo das páginas públicas.
/// </summary>
public class BreadcrumbItem
{
    public BreadcrumbItem(string texto, string? url = null)
    {
        Texto = texto;
        Url = url;
    }

    public string Texto { get; set; }

    public string? Url { get; set; }
}
