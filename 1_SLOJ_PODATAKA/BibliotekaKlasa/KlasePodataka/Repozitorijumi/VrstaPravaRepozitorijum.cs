using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    public class VrstaPravaRepozitorijum
    {
        private string _konekcionaVeza;

        public VrstaPravaRepozitorijum(string konekcionaVeza)
        {
            _konekcionaVeza = konekcionaVeza;
        }

        public List<VrstaPravaModel> DajSvaPrava()
        {
            var rezultat = new List<VrstaPravaModel>();

            using (var konekcija = new SqlConnection(_konekcionaVeza))
            {
                konekcija.Open();

                string sql = @"
                    SELECT Id, Sifra, Naziv
                    FROM vrsta_prava
                    ORDER BY Sifra
                ";

                using (var komanda = new SqlCommand(sql, konekcija))
                {
                    using (var reader = komanda.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new VrstaPravaModel
                            {
                                Id = reader.GetInt16(0),
                                Sifra = reader.GetString(1),
                                Naziv = reader.GetString(2)
                            };

                            rezultat.Add(item);
                        }
                    }
                }
            }

            return rezultat;
        }
    }
}