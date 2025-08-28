using Promise.ProductManagementSystem.Data;

namespace Promise.ProductManagementSystem.Middleware
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorLoggingMiddleware> _logger;
        public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An unhandled exception occurred.");

                var auditLog = new Models.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    ExceptionType = ex.GetType().ToString()
                };
                db.AuditLogs.Add(auditLog);
                await db.SaveChangesAsync();

                // Set the response status code and message
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
            }
        }
    }
}
