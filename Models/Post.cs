using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiarioDoCoelho.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o título da notícia.")]
        [StringLength(200)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe um resumo.")]
        [StringLength(500)]
        [Display(Name = "Resumo")]
        public string Resumo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o conteúdo da notícia.")]
        [Display(Name = "Conteúdo")]
        [DataType(DataType.Html)]
        public string Conteudo { get; set; } = string.Empty;

        [Display(Name = "Data de Publicação")]
        [DataType(DataType.DateTime)]
        public DateTime DataPublicacao { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Informe o slug da URL.")]
        [StringLength(220)]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Use apenas letras minúsculas, números e hífens.")]
        [Display(Name = "Slug (URL amigável)")]
        public string Slug { get; set; } = string.Empty;

        [Display(Name = "Imagem de Capa")]
        [StringLength(500)]
        public string? ImagemCapa { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [ForeignKey(nameof(CategoriaId))]
        public Categoria? Categoria { get; set; }

        [Display(Name = "URL da Fonte da Notícia")]
        [StringLength(500)]
        public string? FonteNoticiaUrl { get; set; }

        [Display(Name = "Vídeo Incorporado (embed)")]
        public string? VideoEmbed { get; set; }

        [Display(Name = "Link do Produto Afiliado (ex: Manto/Camisa)")]
        [StringLength(500)]
        public string? ProdutoAfiliadoUrl { get; set; }
    }
}
