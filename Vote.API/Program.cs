var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = new List<string> { "http://localhost:3000", "http://127.0.0.1:3000" };
var frontendUrl = builder.Configuration["Frontend__Url"];
if (!string.IsNullOrEmpty(frontendUrl))
    allowedOrigins.Add(frontendUrl);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins(allowedOrigins.ToArray())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<CastVoteHandler>();
builder.Services.AddScoped<GetVoteReceiptHandler>();

builder.Services.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");

app.UseCors("AllowLocalhost");

app.MapControllers();

app.Run();