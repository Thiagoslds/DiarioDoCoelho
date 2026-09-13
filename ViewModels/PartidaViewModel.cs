namespace DiarioDoCoelho.ViewModels
{
    public class PartidaViewModel
    {
        public string Campeonato { get; set; } = string.Empty;
        public string Mandante { get; set; } = string.Empty;
        public string EscudoMandante { get; set; } = string.Empty;
        public string EscudoFallbackMandante { get; set; } = string.Empty;
        public string? PlacarMandante { get; set; }
        public string Visitante { get; set; } = string.Empty;
        public string EscudoVisitante { get; set; } = string.Empty;
        public string EscudoFallbackVisitante { get; set; } = string.Empty;
        public string? PlacarVisitante { get; set; }
        public string DataHora { get; set; } = string.Empty;
    }
}