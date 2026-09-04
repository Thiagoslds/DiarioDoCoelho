using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;
using System.Xml.Linq; // Necessário para o XDocument

public class ExtratorNoticiasService
{
    private readonly HtmlWeb _web;

    public ExtratorNoticiasService()
    {
        _web = new HtmlWeb();
    }

    public List<NoticiaCoelho> ObterNoticiasNoAtaque()
    {
        var noticias = new List<NoticiaCoelho>();

        try
        {
            // URL do feed de esportes/América do No Ataque
            var xdoc = XDocument.Load("https://noataque.com.br/clubes/america-mg/feed/");

            // Pega os 5 primeiros itens para manter o padrão das outras fontes
            var itens = xdoc.Descendants("item").Take(4);

            foreach (var item in itens)
            {
                var titulo = item.Element("title")?.Value ?? "";
                var link = item.Element("link")?.Value ?? "";

                // Pega a tag customizada da imagem
                var imgDestaque = item.Element("img_destaque")?.Value ?? "";

                // Tenta converter a data do feed, se falhar ou vier vazia, usa a data atual
                var pubDateStr = item.Element("pubDate")?.Value;
                var data = DateTime.TryParse(pubDateStr, out DateTime dataPublicacao)
                           ? dataPublicacao
                           : DateTime.Now;

                noticias.Add(new NoticiaCoelho
                {
                    Titulo = titulo,
                    Url = link,
                    ImagemUrl = imgDestaque,
                    Fonte = "No Ataque",
                    DataExtracao = data
                });
            }
        }
        catch (Exception ex)
        {
            // Em caso de instabilidade no XML do No Ataque, o bloco catch 
            // impede que a página inicial quebre. Ele simplesmente retornará a lista vazia.
            Console.WriteLine($"Erro ao extrair No Ataque: {ex.Message}");
        }

        return noticias;
    }

    public List<NoticiaCoelho> ObterNoticiasOTempo()
    {
        var noticias = new List<NoticiaCoelho>();
        var document = _web.Load("https://www.otempo.com.br/sports/america");

        var nodes = document.DocumentNode.SelectNodes("//li[contains(@class, 'list__wrapper')]");

        if (nodes != null)
        {
            foreach (var node in nodes.Take(4))
            {
                var linkNode = node.SelectSingleNode(".//a[contains(@class, 'list__link')]");
                var tituloNode = node.SelectSingleNode(".//*[contains(@class, 'list__description')]");
                var imgNode = node.SelectSingleNode(".//img[contains(@class, 'list__img')]");

                if (linkNode != null && tituloNode != null)
                {
                    var url = linkNode.GetAttributeValue("href", "");
                    var imgSrc = imgNode?.GetAttributeValue("src", "");

                    if (url.StartsWith("/")) url = "https://www.otempo.com.br" + url;
                    if (imgSrc != null && imgSrc.StartsWith("/")) imgSrc = "https://www.otempo.com.br" + imgSrc;

                    noticias.Add(new NoticiaCoelho
                    {
                        Titulo = tituloNode.InnerText.Trim(),
                        Url = url,
                        ImagemUrl = imgSrc,
                        Fonte = "O Tempo"
                    });
                }
            }
        }
        return noticias;
    }

    public List<NoticiaCoelho> ObterNoticiasItatiaia()
    {
        var noticias = new List<NoticiaCoelho>();
        var document = _web.Load("https://www.itatiaia.com.br/esportes/futebol/futebol-nacional/futebol-mineiro/america");

        var nodes = document.DocumentNode.SelectNodes("//div[@role='listitem']");

        if (nodes != null)
        {
            foreach (var node in nodes.Take(4))
            {
                var linkNode = node.SelectSingleNode(".//a[contains(@class, 'post-link')]");
                var tituloNode = node.SelectSingleNode(".//h3");
                var imgNode = node.SelectSingleNode(".//img");

                var imagemUrl = imgNode?.GetAttributeValue("src", "");

                if (!string.IsNullOrEmpty(imagemUrl))
                {
                    var uri = new Uri(imagemUrl);
                    var query = QueryHelpers.ParseQuery(uri.Query);

                    query["w"] = "650";
                    query["h"] = "650";
                    query["quality"] = "100";

                    var novaQuery = string.Join("&", query.Select(x =>
                        $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value.ToString())}"
                    ));

                    imagemUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}?{novaQuery}";
                }

                if (linkNode != null && tituloNode != null)
                {
                    noticias.Add(new NoticiaCoelho
                    {
                        Titulo = tituloNode.InnerText.Trim(),
                        Url = linkNode.GetAttributeValue("href", ""),
                        ImagemUrl = imagemUrl,
                        Fonte = "Itatiaia"
                    });
                }
            }
        }
        return noticias;
    }

    public List<NoticiaCoelho> ObterTodasNoticias()
    {
        var todasNoticias = new List<NoticiaCoelho>();

        todasNoticias.AddRange(ObterNoticiasNoAtaque());
        todasNoticias.AddRange(ObterNoticiasOTempo());
        todasNoticias.AddRange(ObterNoticiasItatiaia());

        // Ordenando a lista unificada para que as notícias mais recentes (de qualquer portal)
        // fiquem no topo da sua View
        return todasNoticias.OrderByDescending(n => n.DataExtracao).ToList();
    }
}