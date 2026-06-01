using Microsoft.AspNetCore.Identity;

namespace BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije
{
    public static class FunkcijeLozinke
    {
        private static readonly PasswordHasher<object> hasher = new();

        public static string HashujLozinku(string lozinka)
        {
            return hasher.HashPassword(null!, lozinka);
        }

        public static bool ProveriLozinku(string lozinka, string hashLozinke)
        {
            var rezultat = hasher.VerifyHashedPassword(null!, hashLozinke, lozinka);
            return rezultat == PasswordVerificationResult.Success;
        }
    }
}