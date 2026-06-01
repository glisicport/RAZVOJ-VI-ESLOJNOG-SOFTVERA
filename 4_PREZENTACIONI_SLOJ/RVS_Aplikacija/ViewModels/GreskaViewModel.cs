namespace RVS_Aplikacija.ViewModels
{
    public class GreskaViewModel
    {
        public string? IdPoziva { get; set; }
        public bool PokaziIdPoziva => !string.IsNullOrEmpty(IdPoziva);
    }
}
    