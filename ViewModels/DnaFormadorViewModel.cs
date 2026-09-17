namespace DiarioDoCoelho.ViewModels
{
    public class DnaFormadorViewModel
    {
        public PartidaViewModel? PartidaAnterior { get; set; }
        public PartidaViewModel? ProximaPartida { get; set; }
        public List<ClassificacaoViewModel> ClassificacaoMineiroSub20 { get; set; } = new();
        public List<ClassificacaoViewModel> ClassificacaoMineiroSub17 { get; set; } = new();
        public List<ClassificacaoViewModel> ClassificacaoBrasileiroSub17 { get; set; } = new();
    }
}