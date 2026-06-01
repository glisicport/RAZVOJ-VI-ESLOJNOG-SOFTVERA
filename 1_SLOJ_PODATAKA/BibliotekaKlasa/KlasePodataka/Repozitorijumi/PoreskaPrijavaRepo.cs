using BibliotekaKlasa.KlasePodataka.ApiPozivi;
using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    public class PoreskaPrijavaRepo
    {
        private const string UpitObrisiNepokretnost = @"
        DELETE FROM nepokretnost
        WHERE prijava_id = @PrijavaId;";

        private const string UpitObrisiPoreskuPrijavu = @"
        DELETE FROM poreska_prijava
        WHERE id = @PrijavaId;";

        private const string UpitVratiPrijavuPoKorisnikId = @"
        SELECT *
        FROM vw_poreska_prijava_detalji
        WHERE obveznik_lice_id = (
            SELECT LiceID FROM Korisnik WHERE id = @id
        )
        OR podnosilac_lice_id = (
            SELECT LiceID FROM Korisnik WHERE id = @id
        );";

        private const string UpitVratiPrijavuPoId = @"
        SELECT *
        FROM prijava_po_id
        WHERE id = @id;";

        private const string UpitIzmeniPoreskuPrijavu = @"
        UPDATE poreska_prijava
        SET 
            vrsta_prava_id = @vrsta,
            jls_id = @jls,
            dan_nastanka_poreske_obaveze = @dan_nastanka,
            dan_prestanka_poreske_obaveze = @dan_prestanka,
            razlog_prestanka_id = @razlog,
            ima_poreski_kredit = @kredit,
            kredit_osnov = @kredit_osnov,
            ima_poresko_oslobodjenje = @oslobodjenje,
            oslobodjenje_osnov = @oslobodjenje_osnov,
            napomena = @napomena,
            popis_prilozenih_dokaza = @dokazi,
            OrganizacionaJedinica = @org,
            azurirana_u = GETDATE(),
            je_izmenjena = 1,
            poresko_umanjenje = @poresko_umanjenje
        WHERE id = @id;";

        private const string UpitIzmeniPoreskeNepokretnosti = @"
        INSERT INTO Nepokretnost
        (id, prijava_id, vrsta_id, katastarska_opstina_id, UlicaIbroj, br_kat_parcele,
         godina_izgradnje, korisna_povrsina, ukupna_povrsina, udeo_obveznika,
         ima_poreski_kredit, ima_poresko_oslobodjenje, napomena, rbr)
        VALUES
        (@id, @prijava, @vrsta, @kops, @ulica, @parcela,
         @god, @korisna, @ukupna, @udeo,
         @kredit, @oslobodjenje, @napomena, @rbr);";

        private const string SpPoreskaPrijavaInsert = "dbo.sp_poreska_prijava_insert";
        private const string SpNepokretnostInsert = "dbo.sp_nepokretnost_insert";

        private readonly string _konekcionaVeza;

        public PoreskaPrijavaRepo(string konekcionaVeza)
        {
            _konekcionaVeza = konekcionaVeza;
        }

        public bool Obrisi(Guid prijavaId)
        {
            Console.WriteLine(prijavaId);
            using var konekcija = new SqlConnection(_konekcionaVeza);
            konekcija.Open();

            using var transakcija = konekcija.BeginTransaction();

            try
            {
                using (var komanda = new SqlCommand(UpitObrisiNepokretnost, konekcija, transakcija))
                {
                    komanda.Parameters.AddWithValue("@PrijavaId", prijavaId);
                    komanda.ExecuteNonQuery();
                }

                int brojObrisanihPrijava;

                using (var komanda = new SqlCommand(UpitObrisiPoreskuPrijavu, konekcija, transakcija))
                {
                    komanda.Parameters.AddWithValue("@PrijavaId", prijavaId);
                    brojObrisanihPrijava = komanda.ExecuteNonQuery();
                }

                transakcija.Commit();
                return brojObrisanihPrijava > 0;
            }
            catch
            {
                transakcija.Rollback();
                return false;
            }
        }

        public List<PoreskaPrijavaOdziv> VratiPrijavuPoKorisnikID(Guid korisnikId)
        {
            using var konekcija = new SqlConnection(_konekcionaVeza);
            konekcija.Open();

            var prijavePoId = new Dictionary<Guid, PoreskaPrijavaOdziv>();

            using var komanda = konekcija.CreateCommand();
            komanda.CommandText = UpitVratiPrijavuPoKorisnikId;
            komanda.Parameters.Add(new SqlParameter("@id", korisnikId));

            using var reader = komanda.ExecuteReader();
            while (reader.Read())
            {
                var prijavaId = reader.GetGuid(reader.GetOrdinal("poreska_prijava_id"));

                if (!prijavePoId.TryGetValue(prijavaId, out var prijava))
                {
                    prijava = new PoreskaPrijavaOdziv
                    {
                        PoreskaPrijavaId = prijavaId,

                        ObveznikLiceId = reader.GetGuid(reader.GetOrdinal("obveznik_lice_id")),
                        ObveznikLiceNaziv = reader["obveznik_lice_naziv"] as string,
                        JMBG = reader["jmbg"] as string,
                        PIB = reader["pib"] as string,
                        Telefon = reader["telefon"] as string,
                        Email = reader["email"] as string,
                        PodnosilacLiceId = reader["podnosilac_lice_id"] as Guid?,
                        PodnosilacLiceNaziv = reader["podnosilac_lice_naziv"] as string,

                        JeIzmenjena = reader.GetBoolean(reader.GetOrdinal("je_izmenjena")),

                        VrstaPravaId = reader.GetInt16(reader.GetOrdinal("vrsta_prava_id")),
                        VrstaPravaNaziv = reader["vrsta_prava_naziv"] as string,

                        JlsId = reader.GetGuid(reader.GetOrdinal("jls_id")),
                        JlsNaziv = reader["jls_naziv"] as string,
                        ObveznikStan = reader["obveznik_stan"] as string,
                        ObveznikUlicaBroj = reader["obveznik_ulica_broj"] as string,
                        ObveznikMesto = reader["obveznik_mesto"] as string,
                        ObveznikPtt = reader["obveznik_ptt"] as int?,

                        DanNastankaPoreskeObaveze = reader["dan_nastanka_poreske_obaveze"] as DateTime?,
                        DanPrestankaPoreskeObaveze = reader["dan_prestanka_poreske_obaveze"] as DateTime?,
                        RazlogPrestankaId = reader["razlog_prestanka_id"] as short?,

                        PpImaPoreskiKredit = reader.GetBoolean(reader.GetOrdinal("pp_ima_poreski_kredit")),
                        PpKreditOsnov = reader["pp_kredit_osnov"] as string,

                        PpImaPoreskoOslobodjenje = reader.GetBoolean(reader.GetOrdinal("pp_ima_poresko_oslobodjenje")),
                        PpOslobodjenjeOsnov = reader["pp_oslobodjenje_osnov"] as string,

                        DatumPodnosenja = reader["datum_podnosenja"] as DateTime?,

                        PpNapomena = reader["pp_napomena"] as string,

                        KreiranaU = reader.GetDateTime(reader.GetOrdinal("kreirana_u")),
                        AzuriranaU = reader["azurirana_u"] as DateTime?,
                        PopisPrilozenihDokaza = reader["popis_prilozenih_dokaza"] as string,
                        OrganizacionaJedinica = reader["OrganizacionaJedinica"] as string,

                        Nepokretnosti = new List<NepokretnostOdziv>()
                    };

                    prijavePoId.Add(prijavaId, prijava);
                }

                if (reader["nepokretnost_id"] != DBNull.Value)
                {
                    prijava.Nepokretnosti.Add(new NepokretnostOdziv
                    {
                        NepokretnostId = reader["nepokretnost_id"] as Guid?,
                        PrijavaId = reader["nepokretnost_prijava_id"] as Guid?,

                        VrstaId = reader["nepokretnost_vrsta_id"] as short?,
                        Rbr = reader["rbr"] as short?,

                        KatastarskaOpstinaId = reader["katastarska_opstina_id"] as Guid?,
                        KatastarskaOpstinaNaziv = reader["katastarska_opstina_naziv"] as string,

                        UlicaIbroj = reader["UlicaIbroj"] as string,
                        BrKatParcele = reader["br_kat_parcele"] as string,
                        SifraVrste = reader["sifra"] as string,

                        GodinaIzgradnje = reader["godina_izgradnje"] as short?,

                        KorisnaPovrsina = reader["korisna_povrsina"] as decimal?,
                        UkupnaPovrsina = reader["ukupna_povrsina"] as decimal?,
                        UdeoObveznika = reader["udeo_obveznika"] as decimal?,

                        ImaPoreskiKredit = reader["n_ima_poreski_kredit"] as bool?,
                        ImaPoreskoOslobodjenje = reader["n_ima_poresko_oslobodjenje"] as bool?,

                        OslobodjenjeOsnov = reader["n_oslobodjenje_osnov"] as string,
                        Napomena = reader["n_napomena"] as string
                    });
                }
            }

            return prijavePoId.Values.ToList();
        }

        public List<PoreskaPrijava> VratiPrijavuPoID(Guid id)
        {
            using var konekcija = new SqlConnection(_konekcionaVeza);
            konekcija.Open();

            var prijavePoId = new Dictionary<Guid, PoreskaPrijava>();

            using var komanda = konekcija.CreateCommand();
            komanda.CommandText = UpitVratiPrijavuPoId;
            komanda.Parameters.Add(new SqlParameter("@id", id));

            using var reader = komanda.ExecuteReader();
            while (reader.Read())
            {
                var prijavaId = reader.GetGuid(reader.GetOrdinal("id"));

                if (!prijavePoId.TryGetValue(prijavaId, out var prijava))
                {
                    prijava = new PoreskaPrijava
                    {
                        Id = prijavaId,

                        ObveznikTip = reader["obveznik_lice_naziv"] as string,
                        PodnosilacNaziv = reader["podnosilac_lice_naziv"] as string,

                        VrstaPravaId = reader["vrsta_prava_id"].ToString(),
                        JlsId = reader.GetGuid(reader.GetOrdinal("jls_id")),

                        ImaPoreskiKredit = reader.GetBoolean(reader.GetOrdinal("ima_poreski_kredit")),
                        ImaPoreskoOslobodjenje = reader.GetBoolean(reader.GetOrdinal("ima_poresko_oslobodjenje")),

                        kredit_osnov = reader["kredit_osnov"] as string,
                        oslobodjenje_osnov = reader["oslobodjenje_osnov"] as string,
                        OrganizacionaJedinica = reader["OrganizacionaJedinica"] as string,

                        DanNastankaPoreskeObaveze = reader["dan_nastanka_poreske_obaveze"] as DateTime? ?? default,
                        DanPrestankaPoreskeObaveze = reader["dan_prestanka_poreske_obaveze"] as DateTime? ?? default,
                        PopisPrilozenihDokazas = reader["popis_prilozenih_dokaza"] as string,

                        DatumPodnosenja = reader["datum_podnosenja"] as DateTime? ?? default,

                        Napomena = reader["napomena"] as string,

                        Objekti = new List<Nepokretnost>()
                    };

                    prijavePoId.Add(prijavaId, prijava);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("nepokretnost_id")))
                {
                    var nepokretnost = new Nepokretnost
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("nepokretnost_id")),
                        PrijavaId = reader.GetGuid(reader.GetOrdinal("nepokretnost_prijava_id")),

                        VrstaId = reader.GetInt16(reader.GetOrdinal("nepokretnost_vrsta_id")),
                        Rbr = reader.GetInt16(reader.GetOrdinal("rbr")),

                        KatastarskaOpstinaId = reader.GetGuid(reader.GetOrdinal("katastarska_opstina_id")),
                        AdresaId = Guid.Empty,

                        UlicaIbroj = reader["UlicaIbroj"] as string,
                        BrKatParcele = reader["br_kat_parcele"] as string,

                        GodinaIzgradnje = reader["godina_izgradnje"] as short?,

                        KorisnaPovrsina = reader.GetDecimal(reader.GetOrdinal("korisna_povrsina")),
                        UkupnaPovrsina = reader.GetDecimal(reader.GetOrdinal("ukupna_povrsina")),
                        UdeoObveznika = reader.GetDecimal(reader.GetOrdinal("udeo_obveznika")),

                        ImaPoreskiKredit = reader.GetBoolean(reader.GetOrdinal("ima_poreski_kredit_objekat")),
                        ImaPoreskoOslobodjenje = reader.GetBoolean(reader.GetOrdinal("ima_poresko_oslobodjenje_objekat")),

                        OslobodjenjeOsnov = reader["oslobodjenje_osnov_objekat"] as string,
                        Napomena = reader["nepokretnost_napomena"] as string
                    };

                    prijava.Objekti.Add(nepokretnost);
                }
            }

            return prijavePoId.Values.ToList();
        }

        public void DodajPoreskuPrijavu(
            PoreskaPrijavaModel prijava,
            decimal poresko_umanjenje)
        {
            using var konekcija = new SqlConnection(_konekcionaVeza);
            konekcija.Open();

            using var transakcija = konekcija.BeginTransaction();

            try
            {
                using (var komanda = new SqlCommand(SpPoreskaPrijavaInsert, konekcija, transakcija))
                {
                    komanda.CommandType = CommandType.StoredProcedure;

                    komanda.Parameters.AddWithValue("@id", prijava.Id);
                    komanda.Parameters.AddWithValue("@obveznik_korisnik_id", prijava.ObveznikLiceId);
                    komanda.Parameters.AddWithValue("@OrganizacionaJedinica", (object?)prijava.OrganizacionaJedinica ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@podnosilac_korisnik_id", (object?)prijava.PodnosilacLiceId ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@je_izmenjena", prijava.JeIzmenjena);
                    komanda.Parameters.AddWithValue("@vrsta_prava_id", prijava.VrstaPravaId);
                    komanda.Parameters.AddWithValue("@jls_id", prijava.JlsId);
                    komanda.Parameters.AddWithValue("@dan_nastanka_poreske_obaveze", (object?)prijava.DanNastanka ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@dan_prestanka_poreske_obaveze", (object?)prijava.DanPrestanka ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@razlog_prestanka_id", (object?)prijava.RazlogPrestankaId ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@ima_poreski_kredit", prijava.ImaPoreskiKredit);
                    komanda.Parameters.AddWithValue("@kredit_osnov", (object?)prijava.kredit_osnov ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@ima_poresko_oslobodjenje", prijava.ImaPoreskoOslobodjenje);
                    komanda.Parameters.AddWithValue("@oslobodjenje_osnov", (object?)prijava.oslobodjenje_osnov ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@datum_podnosenja", (object?)prijava.DatumPodnosenja ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@napomena", (object?)prijava.Napomena3 ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@kreirana_u", prijava.KreiranaU);
                    komanda.Parameters.AddWithValue("@azurirana_u", prijava.AzuriranaU);
                    komanda.Parameters.AddWithValue("@popis_prilozenih_dokaza", (object?)prijava.PopisDokaza ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@poresko_umanjenje", poresko_umanjenje);

                    komanda.ExecuteNonQuery();
                }

                foreach (var nepokretnost in prijava.Nepokretnosti)
                {
                    using var komanda = new SqlCommand(SpNepokretnostInsert, konekcija, transakcija);
                    komanda.CommandType = CommandType.StoredProcedure;

                    komanda.Parameters.AddWithValue("@id", nepokretnost.Id);
                    komanda.Parameters.AddWithValue("@UlicaIbroj", nepokretnost.UlicaIbroj);
                    komanda.Parameters.AddWithValue("@prijava_id", prijava.Id);
                    komanda.Parameters.AddWithValue("@vrsta_id", nepokretnost.VrstaId);
                    komanda.Parameters.AddWithValue("@rbr", nepokretnost.Rbr);
                    komanda.Parameters.AddWithValue("@katastarska_opstina_id", (object?)nepokretnost.KatastarskaOpstinaId ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@br_kat_parcele", (object?)nepokretnost.BrKatParcele ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@godina_izgradnje", (object?)nepokretnost.GodinaIzgradnje ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@korisna_povrsina", nepokretnost.KorisnaPovrsina);
                    komanda.Parameters.AddWithValue("@ukupna_povrsina", nepokretnost.UkupnaPovrsina);
                    komanda.Parameters.AddWithValue("@udeo_obveznika", nepokretnost.UdeoObveznika);
                    komanda.Parameters.AddWithValue("@ima_poreski_kredit", nepokretnost.ImaPoreskiKredit);
                    komanda.Parameters.AddWithValue("@ima_poresko_oslobodjenje", nepokretnost.ImaPoreskoOslobodjenje);
                    komanda.Parameters.AddWithValue("@oslobodjenje_osnov", (object?)nepokretnost.OslobodjenjeOsnov ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@napomena", (object?)nepokretnost.Napomena ?? DBNull.Value);

                    komanda.ExecuteNonQuery();
                }

                transakcija.Commit();
            }
            catch
            {
                transakcija.Rollback();
                throw;
            }
        }

        public void IzmeniPoreskaPrijava(PoreskaPrijavaModel model, decimal poresko_umanjenje)
        {
            using var konekcija = new SqlConnection(_konekcionaVeza);
            konekcija.Open();

            using var transakcija = konekcija.BeginTransaction();

            try
            {
                using (var komanda = konekcija.CreateCommand())
                {
                    komanda.Transaction = transakcija;
                    komanda.CommandText = UpitIzmeniPoreskuPrijavu;

                    komanda.Parameters.AddWithValue("@id", model.Id);
                    komanda.Parameters.AddWithValue("@poresko_umanjenje", poresko_umanjenje);
                    komanda.Parameters.AddWithValue("@vrsta", model.VrstaPravaId);
                    komanda.Parameters.AddWithValue("@jls", model.JlsId);
                    komanda.Parameters.AddWithValue("@kredit_osnov", (object)model.kredit_osnov ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@oslobodjenje_osnov", (object)model.oslobodjenje_osnov ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@dan_nastanka", (object)model.DanNastanka ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@dan_prestanka", (object)model.DanPrestanka ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@razlog", (object)model.RazlogPrestankaId ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@kredit", model.ImaPoreskiKredit);
                    komanda.Parameters.AddWithValue("@oslobodjenje", model.ImaPoreskoOslobodjenje);
                    komanda.Parameters.AddWithValue("@napomena", (object)model.Napomena3 ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@dokazi", (object)model.PopisDokaza ?? DBNull.Value);
                    komanda.Parameters.AddWithValue("@org", (object)model.OrganizacionaJedinica ?? DBNull.Value);

                    komanda.ExecuteNonQuery();
                }

                using (var komanda = konekcija.CreateCommand())
                {
                    komanda.Transaction = transakcija;
                    komanda.CommandText = "DELETE FROM Nepokretnost WHERE prijava_id = @id";
                    komanda.Parameters.AddWithValue("@id", model.Id);
                    komanda.ExecuteNonQuery();
                }

                if (model.Nepokretnosti != null)
                {
                    foreach (var nepokretnost in model.Nepokretnosti)
                    {
                        using var komanda = konekcija.CreateCommand();
                        komanda.Transaction = transakcija;

                        komanda.CommandText = UpitIzmeniPoreskeNepokretnosti;

                        komanda.Parameters.AddWithValue("@id", Guid.NewGuid());
                        komanda.Parameters.AddWithValue("@prijava", model.Id);
                        komanda.Parameters.AddWithValue("@vrsta", nepokretnost.VrstaId);
                        komanda.Parameters.AddWithValue("@kops", nepokretnost.KatastarskaOpstinaId);
                        komanda.Parameters.AddWithValue("@ulica", nepokretnost.UlicaIbroj);
                        komanda.Parameters.AddWithValue("@parcela", nepokretnost.BrKatParcele);
                        komanda.Parameters.AddWithValue("@god", nepokretnost.GodinaIzgradnje);
                        komanda.Parameters.AddWithValue("@korisna", nepokretnost.KorisnaPovrsina);
                        komanda.Parameters.AddWithValue("@ukupna", nepokretnost.UkupnaPovrsina);
                        komanda.Parameters.AddWithValue("@udeo", nepokretnost.UdeoObveznika);
                        komanda.Parameters.AddWithValue("@kredit", nepokretnost.ImaPoreskiKredit);
                        komanda.Parameters.AddWithValue("@oslobodjenje", nepokretnost.ImaPoreskoOslobodjenje);
                        komanda.Parameters.AddWithValue("@napomena", nepokretnost.Napomena);
                        komanda.Parameters.AddWithValue("@rbr", nepokretnost.Rbr);

                        komanda.ExecuteNonQuery();
                    }
                }

                transakcija.Commit();
            }
            catch
            {
                transakcija.Rollback();
                throw;
            }
        }
    }
}