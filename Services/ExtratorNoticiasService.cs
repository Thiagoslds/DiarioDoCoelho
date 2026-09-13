using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text.RegularExpressions;
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

            // Pega os 4 primeiros itens para manter o padrão das outras fontes
            var itens = xdoc.Descendants("item").Take(4);

            foreach (var item in itens)
            {
                var titulo = item.Element("title")?.Value ?? "";
                var link = item.Element("link")?.Value ?? "";

                // A tag description no RSS contém exatamente o sub-texto/linha fina
                var descricao = item.Element("description")?.Value ?? "";

                // Pega a tag customizada da imagem
                var imgDestaque = item.Element("img_destaque")?.Value ?? "";

                // Tenta converter a data do feed, se falhar ou vier vazia, usa a data atual
                var pubDateStr = item.Element("pubDate")?.Value;
                var data = DateTime.TryParse(pubDateStr, out DateTime dataPublicacao)
                           ? dataPublicacao.ToLocalTime() // Converte de UTC (+0000) para horário local
                           : DateTime.Now;

                noticias.Add(new NoticiaCoelho
                {
                    Titulo = System.Net.WebUtility.HtmlDecode(titulo.Trim()),
                    SubTexto = System.Net.WebUtility.HtmlDecode(descricao.Trim()), // Extrai e limpa o sub-texto
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

    // Atualização do método principal para O Tempo
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

                    // O robô entra na página da notícia e busca os detalhes (Subtexto e Data/Hora)
                    var detalhes = ExtrairDetalhesOTempo(url);
                    DateTime dataPublicacao = detalhes.DataPublicacao ?? DateTime.Now;

                    // Fallback: se a página não tiver a data explícita, mantém a extração pela URL
                    if (detalhes.DataPublicacao == null)
                    {
                        var matchUrl = Regex.Match(url, @"\/(\d{4})\/(\d{1,2})\/(\d{1,2})\/");
                        if (matchUrl.Success)
                        {
                            int ano = int.Parse(matchUrl.Groups[1].Value);
                            int mes = int.Parse(matchUrl.Groups[2].Value);
                            int dia = int.Parse(matchUrl.Groups[3].Value);
                            dataPublicacao = new DateTime(ano, mes, dia);
                        }
                    }

                    noticias.Add(new NoticiaCoelho
                    {
                        Titulo = System.Net.WebUtility.HtmlDecode(tituloNode.InnerText.Trim()),
                        SubTexto = detalhes.SubTexto,
                        Url = url,
                        ImagemUrl = imgSrc,
                        Fonte = "O Tempo",
                        DataExtracao = dataPublicacao
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

        var nodesToProcess = new List<HtmlNode>();

        var nodesBloco2 = document.DocumentNode.SelectNodes("//div[@id='blockList_1_2']//figure");
        if (nodesBloco2 != null)
        {
            nodesToProcess.AddRange(nodesBloco2.Take(3));
        }

        var nodeBloco3 = document.DocumentNode.SelectSingleNode("(//div[@id='blockList_1_3']//div[@role='listitem'])[1]");
        if (nodeBloco3 != null)
        {
            nodesToProcess.Add(nodeBloco3);
        }

        foreach (var node in nodesToProcess)
        {
            var linkNode = node.SelectSingleNode(".//a[contains(@class, 'post-link')]");
            var tituloNode = node.SelectSingleNode(".//h3");
            var imgNode = node.SelectSingleNode(".//img");

            if (linkNode != null && tituloNode != null)
            {
                var url = linkNode.GetAttributeValue("href", "");

                // Garante que a URL está completa antes de mandar para o extrator
                if (url.StartsWith("/")) url = "https://www.itatiaia.com.br" + url;

                // AQUI ESTÁ A MÁGICA: O robô entra na página da notícia e pega os detalhes
                var detalhes = ExtrairDetalhesItatiaia(url);

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

                noticias.Add(new NoticiaCoelho
                {
                    Titulo = WebUtility.HtmlDecode(tituloNode.InnerText.Trim()),
                    SubTexto = detalhes.SubTexto, // Recebe o subtexto da página interna
                    Url = url,
                    ImagemUrl = imagemUrl,
                    Fonte = "Itatiaia",
                    // Se achou a data na página interna, usa ela. Se não, usa a data atual.
                    DataExtracao = detalhes.DataPublicacao ?? DateTime.Now
                });
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

    private (string SubTexto, DateTime? DataPublicacao) ExtrairDetalhesItatiaia(string urlDaNoticia)
    {
        string subTexto = "";
        DateTime? dataPublicacao = null;

        try
        {
            var document = _web.Load(urlDaNoticia);

            // 1. Pega o Sub-texto
            var subTextoNode = document.DocumentNode.SelectSingleNode("//h2[@data-single-excerpt='true']");
            if (subTextoNode != null)
            {
                subTexto = WebUtility.HtmlDecode(subTextoNode.InnerText.Trim());
            }

            // 2. Pega a Data e Horário Exatos (Correção de Fuso)
            var timeNode = document.DocumentNode.SelectSingleNode("//time[@datetime]");
            if (timeNode != null)
            {
                var dataTexto = timeNode.GetAttributeValue("datetime", "");

                // Corta qualquer fuso horário (ex: "Z" ou "-03:00"), pegando apenas os primeiros 19 caracteres "yyyy-MM-ddTHH:mm:ss"
                if (dataTexto.Length >= 19)
                {
                    dataTexto = dataTexto.Substring(0, 19);
                }

                if (DateTime.TryParse(dataTexto, out DateTime dataParseada))
                {
                    // Usa a hora literal, sem aplicar ToLocalTime() para evitar que o C# subtraia 3 horas
                    dataPublicacao = dataParseada;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao extrair detalhes de {urlDaNoticia}: {ex.Message}");
        }

        return (subTexto, dataPublicacao);
    }

    // Novo método auxiliar PRIVADO para O Tempo
    private (string SubTexto, DateTime? DataPublicacao) ExtrairDetalhesOTempo(string urlDaNoticia)
    {
        string subTexto = "";
        DateTime? dataPublicacao = null;

        try
        {
            var document = _web.Load(urlDaNoticia);

            // 1. Pega o Sub-texto
            // No portal O Tempo, o subtítulo fica na tag <h2> com a classe 'cmp__title-bigode'
            var subTextoNode = document.DocumentNode.SelectSingleNode("//h2[contains(@class, 'cmp__title-bigode')]");
            if (subTextoNode != null)
            {
                subTexto = System.Net.WebUtility.HtmlDecode(subTextoNode.InnerText.Trim());
            }

            // 2. Pega a Data e Horário
            // A data fica em uma div com a classe 'cmp__title-publication' no formato "10 de setembro de 2026 | 08:00"
            var timeNode = document.DocumentNode.SelectSingleNode("//div[contains(@class, 'cmp__title-publication')]");
            if (timeNode != null)
            {
                string dataTexto = timeNode.InnerText.Trim();

                // Usando Regex para fatiar a string exatamente onde precisamos (Dia, Mês, Ano, Hora, Minuto)
                var match = Regex.Match(dataTexto, @"(\d{1,2})\s+de\s+([a-zA-ZçÇ]+)\s+de\s+(\d{4})\s+\|\s+(\d{2}):(\d{2})", RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    int dia = int.Parse(match.Groups[1].Value);
                    string mesStr = match.Groups[2].Value.ToLower();
                    int ano = int.Parse(match.Groups[3].Value);
                    int hora = int.Parse(match.Groups[4].Value);
                    int minuto = int.Parse(match.Groups[5].Value);

                    // Converte o mês por extenso em número
                    int mes = mesStr switch
                    {
                        "janeiro" => 1,
                        "fevereiro" => 2,
                        "março" => 3,
                        "marco" => 3,
                        "abril" => 4,
                        "maio" => 5,
                        "junho" => 6,
                        "julho" => 7,
                        "agosto" => 8,
                        "setembro" => 9,
                        "outubro" => 10,
                        "novembro" => 11,
                        "dezembro" => 12,
                        _ => 1
                    };

                    dataPublicacao = new DateTime(ano, mes, dia, hora, minuto, 0);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao extrair detalhes de {urlDaNoticia}: {ex.Message}");
        }

        return (subTexto, dataPublicacao);
    }
}