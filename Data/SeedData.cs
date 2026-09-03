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
        if (await context.Posts.AnyAsync())
        {
            return;
        }

        var categorias = await context.Categorias.ToDictionaryAsync(c => c.Nome, c => c.Id);

        var posts = new List<Post>
        {
            new()
            {
                TipoPost = TipoPost.Noticia,
                Titulo = "América vence de virada e assume liderança do grupo",
                Resumo = "Coelho buscou o resultado no segundo tempo com gols de Sassá e Renato após sair atrás no placar.",
                Conteudo = "<p>O América Futebol Clube fez um grande segundo tempo neste domingo e virou o placar contra o adversário, assumindo a liderança do grupo na competição. A equipe comandada pela comissão técnica mostrou personalidade mesmo saindo atrás no marcador.</p><p>Com a vitória, o Coelho chega a 18 pontos e mantém a invencibilidade em casa nesta temporada.</p>",
                DataPublicacao = DateTime.Now.AddHours(-3),
                Slug = "america-vence-de-virada-lideranca-grupo",
                ImagemCapa = "/img/xicara.webp",
                FonteNoticiaUrl = "https://ge.globo.com/mg/futebol/times/america-mg/",
                CategoriaId = categorias.GetValueOrDefault("Últimas Notícias", 1)
            },
            new()
            {
                TipoPost = TipoPost.Noticia,
                Titulo = "Diretoria confirma três reforços para a próxima janela",
                Resumo = "Clube acerta contratação de um lateral, um volante e um atacante para reforçar o elenco na sequência da temporada.",
                Conteudo = "<p>A diretoria do América anunciou a chegada de três reforços para a próxima janela de transferências. Os nomes ainda não foram revelados oficialmente, mas a expectativa é de anúncios nos próximos dias.</p><p>O planejamento visa reforçar o elenco para as decisões do segundo semestre.</p>",
                DataPublicacao = DateTime.Now.AddDays(-1),
                Slug = "diretoria-confirma-tres-reforcos-proxima-janela",
                ImagemCapa = "/img/xicara.webp",
                FonteNoticiaUrl = "https://www.ogol.com.br/",
                CategoriaId = categorias.GetValueOrDefault("Mercado da Bola", 2)
            },
            new()
            {
                TipoPost = TipoPost.Noticia,
                Titulo = "Categoria de base revela nova joia da Toca da Raposa Alviverde",
                Resumo = "Meia de 17 anos chama atenção e já treina com o time principal sob o olhar da comissão técnica.",
                Conteudo = "<p>Mais um talento da base do América desperta interesse da comissão técnica do time principal. O jovem meia de 17 anos vem se destacando nas competições de base e já participou de treinos com o elenco profissional.</p><p>A diretoria aposta na formação de base como um dos pilares do projeto esportivo do clube.</p>",
                DataPublicacao = DateTime.Now.AddDays(-4),
                Slug = "categoria-de-base-revela-nova-joia",
                ImagemCapa = "/img/xicara.webp",
                FonteNoticiaUrl = "https://www.uol.com.br/esporte/futebol/",
                CategoriaId = categorias.GetValueOrDefault("DNA Formador", 4)
            },
            new()
            {
                TipoPost = TipoPost.Noticia,
                Titulo = "Torcida lota as Arquibancadas para acompanhar treino aberto",
                Resumo = "Milhares de torcedores compareceram ao treino aberto realizado no Independência nesta semana.",
                Conteudo = "<p>O clima de decisão já toma conta da torcida americana. Um treino aberto realizado no estádio Independência reuniu milhares de torcedores que foram prestigiar o elenco antes da rodada decisiva.</p><p>Jogadores e comissão técnica agradeceram o apoio e prometeram retribuir dentro de campo.</p>",
                DataPublicacao = DateTime.Now.AddDays(-6),
                Slug = "torcida-lota-arquibancadas-treino-aberto",
                ImagemCapa = "/img/xicara.webp",
                FonteNoticiaUrl = "https://ge.globo.com/mg/futebol/times/america-mg/",
                CategoriaId = categorias.GetValueOrDefault("Últimas Notícias", 1)
            },
            new()
            {
                TipoPost = TipoPost.Noticia,
                Titulo = "Comissão técnica define escalação para o próximo confronto direto",
                Resumo = "Treinador testa novo esquema tático durante a semana visando o confronto direto pelo acesso.",
                Conteudo = "<p>A comissão técnica do América realizou treinos táticos durante a semana buscando ajustar o esquema para o próximo confronto direto na tabela.</p><p>A expectativa é de mudanças pontuais no time titular.</p>",
                DataPublicacao = DateTime.Now.AddDays(-7),
                Slug = "comissao-tecnica-define-escalacao-confronto-direto",
                ImagemCapa = "/img/xicara.webp",
                FonteNoticiaUrl = "https://ge.globo.com/mg/futebol/times/america-mg/",
                CategoriaId = categorias.GetValueOrDefault("Últimas Notícias", 1)
            },
            new()
            {
                TipoPost = TipoPost.Noticia,
                Titulo = "Atacante do América é sondado por clube da Série A",
                Resumo = "Site especializado aponta interesse de equipe da elite do futebol brasileiro no artilheiro do Coelho.",
                Conteudo = "<p>Segundo apuração de veículo especializado em mercado da bola, um clube da Série A do Campeonato Brasileiro monitora a situação do atacante do América.</p>",
                DataPublicacao = DateTime.Now.AddDays(-8),
                Slug = "atacante-do-america-e-sondado-serie-a",
                ImagemCapa = "/img/xicara.webp",
                FonteNoticiaUrl = "https://www.ogol.com.br/",
                CategoriaId = categorias.GetValueOrDefault("Mercado da Bola", 2)
            },
            new()
            {
                TipoPost = TipoPost.Noticia,
                Titulo = "América anuncia parceria com patrocinador máster para a temporada",
                Resumo = "Nova parceria comercial deve reforçar o caixa do clube para a disputa das competições do ano.",
                Conteudo = "<p>O América Futebol Clube oficializou nesta semana uma nova parceria comercial que deve reforçar o caixa do clube ao longo da temporada.</p>",
                DataPublicacao = DateTime.Now.AddDays(-9),
                Slug = "america-anuncia-parceria-patrocinador-master",
                ImagemCapa = "/img/xicara.webp",
                FonteNoticiaUrl = "https://www.uol.com.br/esporte/futebol/",
                CategoriaId = categorias.GetValueOrDefault("Últimas Notícias", 1)
            },
            new()
            {
                TipoPost = TipoPost.Artigo,
                Titulo = "Análise: o que precisa melhorar na saída de bola do Coelho",
                Resumo = "Colunista aponta os principais pontos táticos que o América ainda precisa ajustar para seguir na briga pelo acesso.",
                Conteudo = "<p>Apesar dos bons resultados recentes, a saída de bola do América ainda apresenta fragilidades que podem custar caro contra adversários mais qualificados taticamente.</p><p>Neste texto, analisamos os principais pontos de atenção e como o time pode evoluir nas próximas rodadas.</p>",
                DataPublicacao = DateTime.Now.AddDays(-2),
                Slug = "analise-saida-de-bola-do-coelho",
                ImagemCapa = "/img/xicara.webp",
                CategoriaId = categorias.GetValueOrDefault("Análise & Opinião", 3)
            },
            new()
            {
                TipoPost = TipoPost.Artigo,
                Titulo = "Opinião: o momento é de cautela, mas com otimismo",
                Resumo = "Colunista defende que a torcida deve manter os pés no chão apesar da boa sequência de resultados.",
                Conteudo = "<p>A sequência positiva de resultados anima a torcida do Coelho, mas é preciso cautela diante do que ainda vem pela frente na temporada.</p><p>Neste artigo, discutimos os motivos para otimismo moderado.</p>",
                DataPublicacao = DateTime.Now.AddDays(-3),
                Slug = "opiniao-momento-de-cautela-com-otimismo",
                ImagemCapa = "/img/xicara.webp",
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
