using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.KlasePodatakaEF.KontekstEF;
using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;
using BibliotekaKlasa.TehnoloskeKlase;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RVS_Aplikacija.Validatori;

var builder = WebApplication.CreateBuilder(args);

// Dodavanje MVC podrške
builder.Services.AddControllersWithViews();

// Registracija DbContext-a (Entity Framework) sa SQL Server konekcijom
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        "Data Source=DESKTOP-8HAA12I;Initial Catalog=PORESKO;Integrated Security=True;Trust Server Certificate=True"));

// Registracija repozitorijuma za Dependency Injection
builder.Services.AddScoped<KonekcijaKlasa>(servisProvajder =>
{
    var konfiguracija = servisProvajder.GetRequiredService<IConfiguration>();
    var konekcioniString = konfiguracija.GetConnectionString("KonekcioniString");
    var konekcijaObjekat = new KonekcijaKlasa(konekcioniString);
    konekcijaObjekat.OtvoriKonekciju();
    return konekcijaObjekat;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin(); // samo za dev
    });
});

builder.Services.AddScoped<MestoRepoziturijum>(); 
builder.Services.AddScoped<KorisnikRepo>();


// Dodavanje cookie autentifikacije
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Nalog/Prijava";
        options.LogoutPath = "/Nalog/OdjaviSe";
        options.AccessDeniedPath = "/Nalog/Prijava";
    });

//validacija za registracija Fluent + viewModel
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegistracijaValidator>();
//Za sesiju
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpClient();
var app = builder.Build();

// Konfiguracija middleware-a
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Pocetna/Greska");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseSession();

// Obavezno zbog autentifikacije / logina
app.UseAuthentication();
app.UseAuthorization();

// Mapiranje ruta
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pocetna}/{action=Index}/{id?}");

app.Run();
