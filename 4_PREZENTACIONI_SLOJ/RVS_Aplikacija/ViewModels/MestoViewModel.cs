using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class MestoViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Naziv mesta je obavezan.")]
        public string Naziv { get; set; }

        [Required(ErrorMessage = "PTT je obavezan.")]
        public int Ptt { get; set; }
    }
}
    