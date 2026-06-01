using BibliotekaKlasa.KlasePodataka.Modeli;
using System.Xml.Serialization;

namespace PoslovnaLogika;

public static class Ogranicenja
{
    [XmlRoot("Oslobodjenje")]
    public class OslobodjenjeConfig
    {
        public int MinimalanBrojNepokretnosti { get; set; }
        public decimal PragUkupneKorisnePovrsine { get; set; }
        public decimal ProcenatProporcionalnogUmanjenja { get; set; }
        public bool ObracunPoUdelima { get; set; }
    }



    public static decimal IzracunajOslobodjenje(PoreskaPrijavaModel prijava)
    {

        if (prijava == null)
        {
            Console.WriteLine("Prijava je NULL");
            throw new ArgumentNullException(nameof(prijava));
        }



        if (prijava.ImaPoreskoOslobodjenje == true)
        {
            Console.WriteLine("Prijava vec ima poresko oslobodjenje -> vracam 0");
            return 0;
        }

        string putanja = Path.Combine(
            "Ogranicenja",
            "proporcionalno_oslobodjenje_po_udelima.xml");

        var serializer = new XmlSerializer(typeof(OslobodjenjeConfig));

        OslobodjenjeConfig config;

        using (var stream = new FileStream(putanja, FileMode.Open, FileAccess.Read))
        {
            config = (OslobodjenjeConfig)serializer.Deserialize(stream)!;
        }
   
        int brojNepokretnosti = prijava.Nepokretnosti?.Count ?? 0;

        decimal ukupnaKorisnaPovrsina =
            prijava.Nepokretnosti?.Sum(x => x.KorisnaPovrsina) ?? 0m;

        bool ispunjavaUslov =
            brojNepokretnosti >= config.MinimalanBrojNepokretnosti &&
            ukupnaKorisnaPovrsina > config.PragUkupneKorisnePovrsine;

        Console.WriteLine($"Ispunjava uslov: {ispunjavaUslov}");

        decimal rezultat = ispunjavaUslov
            ? config.ProcenatProporcionalnogUmanjenja
            : 0m;

        return rezultat;
    }
}