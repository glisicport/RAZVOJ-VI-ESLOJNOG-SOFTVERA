using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;

var builder = WebApplication.CreateBuilder(args);

// =========================
// SERVICES
// =========================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connString =
    builder.Configuration["KonekcioniString:KonekcioniString"];

// Repozitorijumi
builder.Services.AddScoped<VrstaPravaRepozitorijum>(_ =>
    new VrstaPravaRepozitorijum(connString));

builder.Services.AddScoped<PoreskaPrijavaRepo>(_ =>
    new PoreskaPrijavaRepo(connString));

// =========================
// CORS
// =========================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                return origin.StartsWith("http://localhost")
                    || origin.StartsWith("https://localhost");
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(); 

app.UseAuthorization();

app.MapControllers();

app.Run();