using RecruitmentTest.Application;
using RecruitmentTest.Common;
using RecruitmentTest.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.Configure<ApiOptions>(
    builder.Configuration.GetSection(nameof(ApiOptions)));

builder.Services.AddHttpClient();

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseWebSockets();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAll();
}

app.Run();
