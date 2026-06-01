using BibliotekaKlasa.KlasePodataka.Modeli;

namespace RVS_Aplikacija.ViewModels
{
    public class PoreskaPrijavaViewModel
    {
        // ====== GLAVNI PODACI PRIJAVE ======
        public Guid Id { get; set; }

        public Guid ObveznikLiceId { get; set; }
        public Guid? PodnosilacLiceId { get; set; }

        public int VrstaPravaId { get; set; }
        public Guid JlsId { get; set; }

        public string? OrganizacionaJedinica { get; set; }

        public DateTime? DanNastanka { get; set; }
        public DateTime? DanPrestanka { get; set; }
        public DateTime? DatumPodnosenja { get; set; }

        public bool ImaPoreskiKredit { get; set; }
        public string? KreditOsnov { get; set; }

        public bool ImaPoreskoOslobodjenje { get; set; }
        public string? OslobodjenjeOsnov { get; set; }

        public string? MestoPodnosenja { get; set; }
        public string? PopisDokaza { get; set; }
        public string? Napomena3 { get; set; }
        public string? Napomena10 { get; set; }

        public short? RazlogPrestankaId { get; set; }

        // ====== DETAIL LISTA ======
        public List<Nepokretnost> Nepokretnosti { get; set; } = new();

        // ====== DROPDOWN PODACI ======
        public List<VrstaPravaModel> VrstePrava { get; set; } = new();
        public List<KatastarskaOpstina> opstine { get; set; } = new();
    }
}