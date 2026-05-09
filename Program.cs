using CloudCiApi.Middleware;
using CloudCiApi.Services;
using Microsoft.OpenApi.Models;

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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "API key required to call CloudCiApi. Paste the key generated for your environment."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// CORS must come before the API-key gate so OPTIONS preflights short-circuit at CORS
// and don't get a 401 from the missing X-Api-Key header.
app.UseCors(LocalDevCorsPolicy);

// Gate every endpoint behind the API key.
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();
