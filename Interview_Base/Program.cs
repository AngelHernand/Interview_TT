using Interview_Base.Extensions;
using Interview_Base.Middleware;
using Serilog;

//  Serilog bootstrap 
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Servicios 
    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy =
                System.Text.Json.JsonNamingPolicy.CamelCase;
            opts.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            // Evitar ciclos de referencia en las entidades de EF
            opts.JsonSerializerOptions.ReferenceHandler =
                System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    builder.Services.AddEndpointsApiExplorer();

    // Extensiones de configuración
    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddSwaggerDocumentation();
    builder.Services.AddValidators();
    builder.Services.AddCorsPolicy();
    builder.Services.AddInterviewServices(builder.Configuration);

    var app = builder.Build();

    // Pipeline HTTP 

    // Middleware de excepciones globales, primero para capturar todo
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UsersAPI v1");
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });
    }

    app.UseHttpsRedirection();

    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    // Middleware de auditoría, después de auth para tener el usuario
    app.UseMiddleware<AuditMiddleware>();

    app.MapControllers();

    Log.Information("UsersAPI iniciada correctamente");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar");
}
finally
{
    Log.CloseAndFlush();
}
