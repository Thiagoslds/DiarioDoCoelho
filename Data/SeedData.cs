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
                Titulo = "América vence de virada e assume liderança do grupo",
                Resumo = "Coelho buscou o resultado no segundo tempo com gols de Sassá e Renato após sair atrás no placar.",
                Conteudo = "<p>O América Futebol Clube fez um grande segundo tempo neste domingo e virou o placar contra o adversário, assumindo a liderança do grupo na competição. A equipe comandada pela comissão técnica mostrou personalidade mesmo saindo atrás no marcador.</p><p>Com a vitória, o Coelho chega a 18 pontos e mantém a invencibilidade em casa nesta temporada.</p>",
                DataPublicacao = DateTime.Now.AddHours(-3),
                Slug = "america-vence-de-virada-lideranca-grupo",
                ImagemCapa = "/img/jogador.jpg",
                CategoriaId = categorias.GetValueOrDefault("Últimas Notícias", 1)
            },
            new()
            {
                Titulo = "Diretoria confirma três reforços para a próxima janela",
                Resumo = "Clube acerta contratação de um lateral, um volante e um atacante para reforçar o elenco na sequência da temporada.",
                Conteudo = "<p>A diretoria do América anunciou a chegada de três reforços para a próxima janela de transferências. Os nomes ainda não foram revelados oficialmente, mas a expectativa é de anúncios nos próximos dias.</p><p>O planejamento visa reforçar o elenco para as decisões do segundo semestre.</p>",
                DataPublicacao = DateTime.Now.AddDays(-1),
                Slug = "diretoria-confirma-tres-reforcos-proxima-janela",
                ImagemCapa = "/img/camisa.jpg",
                CategoriaId = categorias.GetValueOrDefault("Mercado da Bola", 2)
            },
            new()
            {
                Titulo = "Análise: o que precisa melhorar na saída de bola do Coelho",
                Resumo = "Colunista aponta os principais pontos táticos que o América ainda precisa ajustar para seguir na briga pelo acesso.",
                Conteudo = "<p>Apesar dos bons resultados recentes, a saída de bola do América ainda apresenta fragilidades que podem custar caro contra adversários mais qualificados taticamente.</p><p>Neste texto, analisamos os principais pontos de atenção e como o time pode evoluir nas próximas rodadas.</p>",
                DataPublicacao = DateTime.Now.AddDays(-2),
                Slug = "analise-saida-de-bola-do-coelho",
                ImagemCapa = "/img/Logo-America-VERDE-LIMPA.png",
                CategoriaId = categorias.GetValueOrDefault("Análise & Opinião", 3)
            },
            new()
            {
                Titulo = "Categoria de base revela nova joia da Toca da Raposa Alviverde",
                Resumo = "Meia de 17 anos chama atenção e já treina com o time principal sob o olhar da comissão técnica.",
                Conteudo = "<p>Mais um talento da base do América desperta interesse da comissão técnica do time principal. O jovem meia de 17 anos vem se destacando nas competições de base e já participou de treinos com o elenco profissional.</p><p>A diretoria aposta na formação de base como um dos pilares do projeto esportivo do clube.</p>",
                DataPublicacao = DateTime.Now.AddDays(-4),
                Slug = "categoria-de-base-revela-nova-joia",
                ImagemCapa = "/img/afc-escudos-site-branco-1.png",
                CategoriaId = categorias.GetValueOrDefault("DNA Formador", 4)
            },
            new()
            {
                Titulo = "Torcida lota as Arquibancadas para acompanhar treino aberto",
                Resumo = "Milhares de torcedores compareceram ao treino aberto realizado no Independência nesta semana.",
                Conteudo = "<p>O clima de decisão já toma conta da torcida americana. Um treino aberto realizado no estádio Independência reuniu milhares de torcedores que foram prestigiar o elenco antes da rodada decisiva.</p><p>Jogadores e comissão técnica agradeceram o apoio e prometeram retribuir dentro de campo.</p>",
                DataPublicacao = DateTime.Now.AddDays(-6),
                Slug = "torcida-lota-arquibancadas-treino-aberto",
                ImagemCapa = "/img/jogador.jpg",
                CategoriaId = categorias.GetValueOrDefault("Últimas Notícias", 1)
            }
        };

        context.Posts.AddRange(posts);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Popula um próximo jogo fictício para exibição na Home, caso não exista nenhum futuro cadastrado.
    /// </summary>
    private static async Task SeedProximoJogoFicticioAsync(ApplicationDbContext context)
    {
        var existeJogoFuturo = await context.Jogos.AnyAsync(j => j.DataHora >= DateTime.Now);
        if (existeJogoFuturo)
        {
            return;
        }

        context.Jogos.Add(new Jogo
        {
            DataHora = DateTime.Now.AddDays(4).Date.AddHours(16),
            Adversario = "Cruzeiro",
            Campeonato = "Campeonato Mineiro",
            Mandante = true
        });

        await context.SaveChangesAsync();
    }
}
