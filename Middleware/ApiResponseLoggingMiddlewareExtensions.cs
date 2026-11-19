namespace FirstApplication.Middleware
{
    public static class ApiResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiResponseLogging(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            return app.UseMiddleware<ApiResponseLoggingMiddleware>();
        }
    }
}
