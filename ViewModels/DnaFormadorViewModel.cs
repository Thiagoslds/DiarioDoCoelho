namespace DiarioDoCoelho.ViewModels
{
    public class DnaFormadorViewModel
    {
        public PartidaViewModel? PartidaAnterior { get; set; }
        public PartidaViewModel? ProximaPartida { get; set; }
        public List<ClassificacaoViewModel> Classificacao { get; set; } = new();
    }
}