using System.ComponentModel.DataAnnotations;

namespace DiarioDoCoelho.Models
{
    public class ProdutoLoja
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do produto.")]
        [StringLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a URL da imagem.")]
        [StringLength(500)]
        [Url(ErrorMessage = "Informe uma URL válida.")]
        public string ImagemUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a descrição do produto.")]
        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a loja (ex: Shopee, Amazon).")]
        [StringLength(100)]
        public string Loja { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o link de afiliado.")]
        [StringLength(500)]
        [Url(ErrorMessage = "Informe uma URL válida.")]
        public string LinkAfiliado { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;
    }
}
