using System.ComponentModel.DataAnnotations;

namespace DiarioDoCoelho.Models
{
    public class BannerAfiliado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o título do banner.")]
        [StringLength(150)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a URL da imagem.")]
        [StringLength(500)]
        [Display(Name = "Imagem (URL)")]
        public string ImagemUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o link de afiliado.")]
        [StringLength(500)]
        [Display(Name = "Link de Afiliado")]
        public string LinkAfiliado { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Posição")]
        public string Posicao { get; set; } = "Topo"; // Topo, Lateral, MeioTexto

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}
