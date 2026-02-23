using System.Security.Claims;
using Interview_Base.Services.Interfaces;

namespace Interview_Base.Middleware;

// Middleware que registra automáticamente acciones de auditoría
// para cada request HTTP que modifica datos (POST, PUT, DELETE)
public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditService auditService)
    {
        // Solo auditar métodos que modifican datos
        var method = context.Request.Method.ToUpper();
        if (method is "POST" or "PUT" or "DELETE" or "PATCH")
        {
            var userId = GetUserId(context);
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var path = context.Request.Path.Value;

            await auditService.LogAsync(
                userId,
                accion: $"{method} {path}",
                entidad: null,
                entidadId: null,
                valoresAnteriores: null,
                valoresNuevos: null,
                ip: ip);
        }

        await _next(context);
    }

    private static Guid? GetUserId(HttpContext context)
    {
        var claim = context.User.FindFirst("userId") ??
                    context.User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is not null && Guid.TryParse(claim.Value, out var id))
            return id;

        return null;
    }
}
