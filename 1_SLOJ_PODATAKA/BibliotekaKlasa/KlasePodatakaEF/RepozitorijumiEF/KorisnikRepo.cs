using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodatakaEF.KontekstEF;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF
{
    public class KorisnikRepo
    {
        private readonly AppDbContext _appKontekst;

        public KorisnikRepo(AppDbContext appKontekst)
        {
            _appKontekst = appKontekst;
        }
        public async Task<KorisnikModel?> DohvatiPoId(Guid id)
        {
            return await _appKontekst.Korisnici
                .Include(k => k.Lice)
                .ThenInclude(l => l.adresa)
                .FirstOrDefaultAsync(k => k.Id == id);
        }
        public async Task IzmeniKorisnika(KorisnikModel model)
        {
            var korisnik = await _appKontekst.Korisnici
                .Include(k => k.Lice)
                .ThenInclude(l => l.adresa)
                .FirstOrDefaultAsync(k => k.Id == model.Id);

            if (korisnik == null)
                return;

            // Korisnik

            // Lice
            korisnik.Lice.Ime = model.Lice?.Ime;
            korisnik.Lice.Prezime = model.Lice?.Prezime;
            korisnik.Lice.Naziv = model.Lice?.Naziv;
            korisnik.Lice.Telefon = model.Lice?.Telefon;

            // Adresa
            if (korisnik.Lice.adresa != null)
            {
                korisnik.Lice.adresa.MestoId = model.Lice.adresa.MestoId;
                korisnik.Lice.adresa.UlicaIBroj = model.Lice.adresa.UlicaIBroj;
                korisnik.Lice.adresa.Stan = model.Lice.adresa.Stan;
            }

            await _appKontekst.SaveChangesAsync();
        }
        public async Task Dodaj(KorisnikModel korisnik, LiceModel lice)
        {
            using var transakcija = await _appKontekst.Database.BeginTransactionAsync();

            try
            {
                korisnik.Lice = lice; 

                await _appKontekst.Korisnici.AddAsync(korisnik);

                await _appKontekst.SaveChangesAsync();

                await transakcija.CommitAsync();
            }
            catch
            {
                await transakcija.RollbackAsync();
                throw;
            }
        }
        public async Task<KorisnikInfoModel?> DohvatiPoEmailu(string email)
        {
            return await _appKontekst.Korisnici
                .Where(k => k.Email == email)
                .Select(k => new KorisnikInfoModel
                {
                    Email = k.Email,
                    LozinkaHash = k.LozinkaHash,
                    Id = k.Id,
                    Rola = k.Rola,
                    ImePrezimeIliNaziv =
                        (!string.IsNullOrWhiteSpace(k.Lice.Ime) && !string.IsNullOrWhiteSpace(k.Lice.Prezime))
                            ? k.Lice.Ime + " " + k.Lice.Prezime
                            : k.Lice.Naziv
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> PostojiKorisnikSaEmailom(string email)
        {
            return await _appKontekst.Korisnici.AnyAsync(k => k.Email == email);
        }
        public async Task<bool> PostojiMaticniBrojVecUpisan(string maticniBroj)
        {
            return await _appKontekst.Lica.AnyAsync(l => l.Pib == maticniBroj);
        }
        public async Task<bool> PostojiJMBGVecUpisan(string Jmbg)
        {
            return await _appKontekst.Lica.AnyAsync(l => l.Jmbg == Jmbg);
        }
        public async Task<bool> PostojiImeFirmeVecUpisano(string ime)
        {
            return await _appKontekst.Lica.AnyAsync(l => l.Naziv == ime);
        }
        public void Izmeni(KorisnikModel kEObjekat)
        {
            if (kEObjekat == null) return;

            _appKontekst.Korisnici.Update(kEObjekat);
            _appKontekst.SaveChanges();
        }
        public void Obrisi(KorisnikModel kEObjekat)
        {
            if (kEObjekat == null) return;

            _appKontekst.Korisnici.Remove(kEObjekat);
            _appKontekst.SaveChanges();
        }
        public List<KorisnikModel> DajSve()
        {

            var lista = _appKontekst.Korisnici.ToList();
            lista.Sort((a, b) => a.Lice.Ime.CompareTo(b.Lice.Ime));
            return lista;
        }

        public KorisnikModel? DajPoID(int id)
        {
            return _appKontekst.Korisnici.Find(id);
        }

    }
}
