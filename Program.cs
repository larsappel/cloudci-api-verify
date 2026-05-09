using CloudCiApi.Services;

const string LocalDevCorsPolicy = "LocalDev";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IQuoteStore, InMemoryQuoteStore>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(LocalDevCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// CORS must come before UseAuthorization() and MapControllers().
app.UseCors(LocalDevCorsPolicy);

app.UseAuthorization();
app.MapControllers();

app.Run();
