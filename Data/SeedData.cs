using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.Data;

/// <summary>
/// Responsável por aplicar migrations pendentes e garantir a existência
/// do usuário administrador único que gerencia o CMS do site, além de
/// popular dados fictícios para visualização do layout.
/// </summary>
public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var adminEmail = configuration["AdminUser:Email"] ?? "admin@diariodocoelho.com.br";
        var adminPassword = configuration["AdminUser:Password"] ?? "Coelho@2024!";

        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Falha ao criar o usuário administrador: {errors}");
            }
        }

        await SeedNoticiasFicticiasAsync(context);
        await SeedProximoJogoFicticioAsync(context);
    }

    /// <summary>
    /// Popula notícias fictícias apenas para visualização do layout.
    /// Remover/ajustar quando o conteúdo real for cadastrado pelo Admin.
    /// </summary>
    private static async Task SeedNoticiasFicticiasAsync(ApplicationDbContext context)
    {
        /*if (await context.Posts.AnyAsync())
        {
            return;
        }*/

        var categorias = await context.Categorias.ToDictionaryAsync(c => c.Nome, c => c.Id);

        var posts = new List<Post>
        {

            new()
            {
                TipoPost = TipoPost.Artigo,
                Titulo = "O Salum acabou com o América",
                Resumo = "De ano de maior faturamento ao flerte com a Série C: como a gestão centralizadora e a recusa da SAF afundaram o Coelho.",
                Conteudo = "<p>O rebaixamento em 2023 não foi apenas um tropeço; foi um atestado de incompetência. Justamente no ano em que o América registrou o maior faturamento de sua história, a gestão conseguiu entregar uma das piores campanhas que o clube já viu na elite do futebol brasileiro.\r\n\r\nEm vez de uma correção de rota, o que se viu em 2024 foi a continuidade do desastre. O restante dos recursos foi dilapidado na montagem de um elenco que entregou uma campanha absolutamente pífia na Série B. Agora, o roteiro atinge seu ponto mais crítico: a equipe faz a pior campanha de sua história na Segundona, caminhando a passos largos para um trágico e humilhante rebaixamento à Série C.\r\n\r\nO pano de fundo de toda essa derrocada atende por um erro estratégico brutal: a recusa sistemática em negociar a SAF do América com investidores quando o clube estava em alta.\r\n\r\nÉ evidente que existem outros culpados espalhados pelos departamentos do clube nos últimos anos. O grande problema é que a estrutura é viciada: todos parecem atuar como meros capachos, omissos e subservientes às decisões de um único cidadão. O que estão fazendo com a instituição é um verdadeiro absurdo.</p>",
                DataPublicacao = DateTime.Now,
                Slug = "ssalum-acabou-america",
                ImagemCapa = "/img/salum.jpg",
                CategoriaId = categorias.GetValueOrDefault("Análise & Opinião", 3)
            },
            new()
            {
                TipoPost = TipoPost.Artigo,
                Titulo = "O Preço da Incompetência: Como a Diretoria Quebrou e Rebaixou o Coelho",
                Resumo = "Da falsa promessa de responsabilidade financeira ao pânico iminente: o roteiro trágico que condena a temporada e o futuro do clube.",
                Conteudo = "<p>A atual diretoria do América vem protagonizando uma verdadeira aula de como não gerir um clube de futebol. O roteiro desse desastre começou no início do ano, pautado por um forte — e ilusório — discurso de \"austeridade financeira\". Na prática, essa política resultou na montagem de um elenco absolutamente sofrível. O reflexo em campo não poderia ser pior: o time amarga a lanterna isolada, somando míseros 6 pontos em 16 rodadas, um aproveitamento inaceitável de apenas 12,5%.\r\n\r\nAgora, no momento em que o rebaixamento já se desenha como uma realidade praticamente selada, o desespero bateu à porta da administração. Em uma completa e atabalhoada inversão de rota, os dirigentes começam a abrir os cofres para gastar o que a instituição não tem, apostando as últimas fichas em reforços de qualidade, no mínimo, duvidosa.\r\n\r\nO saldo final dessa sequência de trapalhadas já é previsível. Além de encaminhar o rebaixamento da equipe com uma campanha vexatória, a diretoria vai entregar o clube completamente quebrado e com um nível de endividamento assustador para o ano que vem. É o retrato fiel de uma gestão amadora, que compromete não apenas o presente, mas o futuro da instituição.</p>",
                DataPublicacao = DateTime.Now.AddDays(-1),
                Slug = "diretoria-quebrou-america",
                ImagemCapa = "/img/tres-patetas.png",
                CategoriaId = categorias.GetValueOrDefault("Análise & Opinião", 3)
            },
            new()
            {
                TipoPost = TipoPost.Historia,
                Titulo = "Relembre o título mineiro conquistado pelo Coelho",
                Resumo = "Uma viagem no tempo até a conquista histórica do América Futebol Clube no Campeonato Mineiro.",
                Conteudo = "<p>Relembramos um dos capítulos mais marcantes da história do América Futebol Clube: a conquista do título mineiro que ficou marcada na memória da torcida alviverde.</p>",
                DataPublicacao = DateTime.Now.AddDays(-15),
                Slug = "relembre-o-titulo-mineiro-do-coelho",
                ImagemCapa = "/img/xicara.webp",
                ProdutoAfiliadoUrl = "https://www.centauro.com.br/busca?q=camisa%20retro%20america%20mg",
                CategoriaId = categorias.GetValueOrDefault("Baú do Coelho", 5)
            },
            new()
            {
                TipoPost = TipoPost.Historia,
                Titulo = "A história do Independência, casa do Coelho",
                Resumo = "Conheça a trajetória do estádio que é a casa do América Futebol Clube há décadas.",
                Conteudo = "<p>O estádio Independência é palco de momentos históricos do América Futebol Clube. Neste texto, contamos um pouco da trajetória desse templo alviverde.</p>",
                DataPublicacao = DateTime.Now.AddDays(-20),
                Slug = "a-historia-do-independencia-casa-do-coelho",
                ImagemCapa = "/img/xicara.webp",
                CategoriaId = categorias.GetValueOrDefault("Baú do Coelho", 5)
            },
            new()
            {
                TipoPost = TipoPost.Historia,
                Titulo = "Ídolos que marcaram época vestindo o manto alviverde",
                Resumo = "Relembramos jogadores que se tornaram ídolos da torcida do América ao longo da história do clube.",
                Conteudo = "<p>Diversos jogadores marcaram época vestindo a camisa do América Futebol Clube. Neste texto, relembramos alguns dos maiores ídolos alviverdes.</p>",
                DataPublicacao = DateTime.Now.AddDays(-25),
                Slug = "idolos-que-marcaram-epoca-manto-alviverde",
                ImagemCapa = "/img/xicara.webp",
                ProdutoAfiliadoUrl = "https://www.amazon.com.br/s?k=camisa+retro+america+mineiro",
                CategoriaId = categorias.GetValueOrDefault("Baú do Coelho", 5)
            }
        };

        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Popula próximos jogos fictícios para exibição na Home, caso não exista nenhum futuro cadastrado.
    /// </summary>
    private static async Task SeedProximoJogoFicticioAsync(ApplicationDbContext context)
    {
        var existeJogoFuturo = await context.Jogos.AnyAsync(j => j.DataHora >= DateTime.Now);
        if (existeJogoFuturo)
        {
            return;
        }

        context.Jogos.AddRange(
            new Jogo
            {
                Categoria = CategoriaJogo.Profissional,
                DataHora = DateTime.Now.AddDays(4).Date.AddHours(16),
                Adversario = "Cruzeiro",
                Campeonato = "Campeonato Mineiro",
                Mandante = true,
                LinkTabela = "https://ge.globo.com/mg/futebol/campeonato-mineiro/"
            },
            new Jogo
            {
                Categoria = CategoriaJogo.Sub20,
                DataHora = DateTime.Now.AddDays(6).Date.AddHours(10),
                Adversario = "Cruzeiro",
                Campeonato = "Campeonato Mineiro Sub-20",
                Mandante = true,
                LinkTabela = "https://ge.globo.com/mg/futebol/campeonato-mineiro/"
            },
            new Jogo
            {
                Categoria = CategoriaJogo.Sub17,
                DataHora = DateTime.Now.AddDays(9).Date.AddHours(15),
                Adversario = "Cruzeiro",
                Campeonato = "Brasileiro Sub-17",
                Mandante = false,
                LinkTabela = "https://www.cbf.com.br/futebol-brasileiro/competicoes/campeonato-brasileiro-sub-17"
            }
        );

        await context.SaveChangesAsync();
    }
}
