using System;

namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    public class PoreskaPrijavaModel
    {
        public Guid Id { get; set; }

        public Guid ObveznikLiceId { get; set; }
        public Guid? PodnosilacLiceId { get; set; }

        public bool JeIzmenjena { get; set; }

        public short VrstaPravaId { get; set; }
        public Guid JlsId { get; set; }

        public DateTime? DanNastanka { get; set; }
        public DateTime? DanPrestanka { get; set; }

        public bool ImaPoreskiKredit { get; set; }
        public string KreditOsnov { get; set; }
        public string OrganizacionaJedinica { get; set; }

        public bool ImaPoreskoOslobodjenje { get; set; }
        public string OslobodjenjeOsnov { get; set; }

        public DateTime? DatumPodnosenja { get; set; }
        public string MestoPodnosenja { get; set; }

        public string PopisDokaza { get; set; }
        public string Napomena3 { get; set; }
        public string Napomena10 { get; set; }
        public string? kredit_osnov { get; set; }
        public string? oslobodjenje_osnov { get; set; }
        public short? RazlogPrestankaId { get; set; }

        public DateTime KreiranaU { get; set; }
        public DateTime AzuriranaU { get; set; }

        public List<Nepokretnost> Nepokretnosti { get; set; } = new();
    }
}