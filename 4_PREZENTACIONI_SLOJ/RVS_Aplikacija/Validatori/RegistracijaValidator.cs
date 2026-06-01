namespace RVS_Aplikacija.Validatori
{
    using FluentValidation;
    using RVS_Aplikacija.ViewModels;

    public class RegistracijaValidator : AbstractValidator<RegistracijaViewModel>
    {
        public RegistracijaValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email je obavezan")
                .EmailAddress().WithMessage("Email nije validan");

            RuleFor(x => x.Lozinka)
                .NotEmpty().WithMessage("Lozinka je obavezna")
                .MinimumLength(6).WithMessage("Lozinka mora imati najmanje 6 karaktera");

            RuleFor(x => x.PotvrdaLozinke)
                .Equal(x => x.Lozinka)
                .WithMessage("Lozinke moraju da se poklapaju");

            RuleFor(x => x.Tip)
                .NotEmpty().WithMessage("Tip lica je obavezan")
                .Must(x => x == "F" || x == "P")
                .WithMessage("Tip mora biti 'F' ili 'P'");

            When(x => x.Tip == "F", () =>
            {
                RuleFor(x => x.Ime)
                    .NotEmpty().WithMessage("Ime je obavezno");

                RuleFor(x => x.Prezime)
                    .NotEmpty().WithMessage("Prezime je obavezno");

                RuleFor(x => x.Jmbg)
                    .NotEmpty().WithMessage("JMBG je obavezan")
                    .Length(13).WithMessage("JMBG mora imati 13 cifara");
            });

            When(x => x.Tip == "P", () =>
            {
                RuleFor(x => x.Naziv)
                    .NotEmpty().WithMessage("Naziv firme je obavezan");

                RuleFor(x => x.MaticniBroj)
                    .NotEmpty().WithMessage("Matični broj je obavezan")
                    .Length(8).WithMessage("Matični broj mora imati 8 cifara");
            });
        }
    }
}