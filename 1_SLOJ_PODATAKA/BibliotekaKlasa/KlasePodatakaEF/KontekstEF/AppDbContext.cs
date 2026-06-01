using Microsoft.EntityFrameworkCore;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;

namespace BibliotekaKlasa.KlasePodatakaEF.KontekstEF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opcije)
            : base(opcije)
        {
        }

        public DbSet<LiceModel> Lica { get; set; }
        public DbSet<KorisnikModel> Korisnici { get; set; }
    }
}