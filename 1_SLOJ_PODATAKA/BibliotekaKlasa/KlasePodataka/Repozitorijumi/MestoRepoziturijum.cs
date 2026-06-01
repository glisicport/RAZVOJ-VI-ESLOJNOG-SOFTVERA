using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    public class MestoRepoziturijum : TabelaKlasa
    {

        public MestoRepoziturijum(KonekcijaKlasa konekcija) : base(konekcija, "jls") { }

        public bool DodajMesto(MestoModel model)
        {
            string upit = $@"
        INSERT INTO mesto (id, naziv, ptt)
        VALUES ('{model.Id}', '{model.Naziv}', {model.Ptt});
    ";

            return this.IzvrsiAzuriranje(upit);
        }

        public bool IzmeniMesto(MestoModel model)
        {
            string upit = $@"
        UPDATE mesto
        SET naziv = '{model.Naziv}',
            ptt = {model.Ptt}
        WHERE id = '{model.Id}';
    ";

            return this.IzvrsiAzuriranje(upit);
        }
        public bool ObrisiMesto(Guid id)
        {
            string upit = $@"
        DELETE FROM mesto
        WHERE id = '{id}';
    ";

            return this.IzvrsiAzuriranje(upit);
        }
        public List<KatastarskaOpstina> DajSveOpstine()
        {
            List<KatastarskaOpstina> opstine = new List<KatastarskaOpstina>();

            string upit = "select id, naziv from katastarska_opstina;";
            DataSet kolekcijaIzBazePodataka = this.DajPodatke(upit);

            foreach (DataRow citacKolekcije in kolekcijaIzBazePodataka.Tables[0].Rows)
            {
                opstine.Add(new KatastarskaOpstina
                {
                    Id = Guid.Parse(citacKolekcije["id"].ToString()),
                    Naziv = citacKolekcije["naziv"].ToString()
                });
            }

            return opstine;
        }
        public List<PravaModel> DajSveVrsteObjekata()
        {
            List<PravaModel> vrste = new List<PravaModel>();

            string upit = "select id, naziv from vrsta_nepokretnosti;";
            DataSet kolekcijaIzBazePodataka = this.DajPodatke(upit);

            foreach (DataRow citacKolekcije in kolekcijaIzBazePodataka.Tables[0].Rows)
            {
                vrste.Add(new PravaModel
                {
                    Id = (short.Parse(citacKolekcije["id"].ToString())),
                    Naziv = citacKolekcije["naziv"].ToString()
                });
            }

            return vrste;
        }
        public List<MestoModel> DajSvaMesta()
        {
            List<MestoModel> mesta = new List<MestoModel>();

            string upit = "SELECT id, naziv, ptt FROM mesto;";
            DataSet kolekcijaIzBazePodataka = this.DajPodatke(upit);

            foreach (DataRow red in kolekcijaIzBazePodataka.Tables[0].Rows)
            {
                mesta.Add(new MestoModel
                {
                    Id = Guid.Parse(red["id"].ToString()),
                    Naziv = red["naziv"].ToString(),
                    Ptt = int.Parse(red["ptt"].ToString())
                });
            }

            return mesta;
        }
        public List<JlsModel> DajSveJLS()
        {
            List<JlsModel> jedinice_lokalne_samouprave = new List<JlsModel>();

            string upit = "select id, naziv from jls;";
            DataSet kolekcijaIzBazePodataka = this.DajPodatke(upit);

            foreach (DataRow citacKolekcije in kolekcijaIzBazePodataka.Tables[0].Rows)
            {
                jedinice_lokalne_samouprave.Add(new JlsModel
                {
                    Id = Guid.Parse(citacKolekcije["id"].ToString()),
                    Naziv = citacKolekcije["naziv"].ToString()
                });
            }

            return jedinice_lokalne_samouprave;
        }
    }
}