using System.ComponentModel.DataAnnotations;

namespace DiarioDoCoelho.Models
{
    public enum CategoriaJogo
    {
        Profissional = 0,
        Sub20 = 1,
        Sub17 = 2
    }

    public class Jogo
    {
        public int Id { get; set; }

        [Display(Name = "Categoria")]
        public CategoriaJogo Categoria { get; set; } = CategoriaJogo.Profissional;

        [Required(ErrorMessage = "Informe a data e hora do jogo.")]
        [Display(Name = "Data e Hora")]
        [DataType(DataType.DateTime)]
        public DateTime DataHora { get; set; }

        [Required(ErrorMessage = "Informe o adversário.")]
        [StringLength(150)]
        [Display(Name = "Adversário")]
        public string Adversario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o campeonato.")]
        [StringLength(150)]
        [Display(Name = "Campeonato")]
        public string Campeonato { get; set; } = string.Empty;

        [Display(Name = "Placar América")]
        public int? PlacarAmerica { get; set; }

        [Display(Name = "Placar Adversário")]
        public int? PlacarAdversario { get; set; }

        [Display(Name = "América manda o jogo?")]
        public bool Mandante { get; set; }

        [StringLength(500)]
        [Display(Name = "Link da Tabela do Campeonato")]
        public string? LinkTabela { get; set; }
    }
}
