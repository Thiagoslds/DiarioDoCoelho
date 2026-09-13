using System.ComponentModel.DataAnnotations;

namespace DiarioDoCoelho.Models
{
    public class DadoExtraido
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Chave { get; set; } = string.Empty;
        
        [Required]
        public string ConteudoJson { get; set; } = string.Empty;
        
        public DateTime DataAtualizacao { get; set; } = DateTime.Now;
    }
}
