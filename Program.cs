using CloudCiApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Singleton — one in-memory store shared across every request.
builder.Services.AddSingleton<IQuoteStore, InMemoryQuoteStore>();

builder.Services.AddControllers();

// OpenAPI / Swagger surface.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger is enabled in BOTH Development AND Production for this course.
// See the Concept Deep Dive below for the trade-off.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
