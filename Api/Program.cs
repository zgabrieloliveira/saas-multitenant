using Api.Middlewares;
using Api.Swagger;
using Infra;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES (DI) ---

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<TenantHeaderOperationFilter>();
});

// BD
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseExceptionHandler();

// --- 2. REQUEST PIPELINE ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// intercept every request to catch the TenantId
app.UseMiddleware<TenantResolverMiddleware>();

app.MapControllers();

app.Run();