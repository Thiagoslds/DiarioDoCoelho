namespace DiarioDoCoelho.ViewModels
{
    public class TabelaIndexViewModel
    {
        public List<ClassificacaoViewModel> Classificacao { get; set; } = new();
        public List<PartidaViewModel> ProximosJogos { get; set; } = new();
        public List<PartidaViewModel> JogosAnteriores { get; set; } = new();
    }
}