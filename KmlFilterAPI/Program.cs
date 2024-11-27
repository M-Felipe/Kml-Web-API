using KmlFilterAPI.services; // Certifique-se de usar o namespace correto

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IKmlService>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    var kmlFilePath = Path.Combine(env.ContentRootPath, "content", "DIRECIONADORES1.kml");

    if (!File.Exists(kmlFilePath))
    {
        throw new FileNotFoundException($"Arquivo KML não encontrado no caminho: {kmlFilePath}");
    }

    return new KmlService(kmlFilePath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

