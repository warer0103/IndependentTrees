using IndependentTrees.API.DataStorage;
using IndependentTrees.API.Exceptions;
using IndependentTrees.API.Models;
using IndependentTrees.API.Providers;

namespace IndependentTrees.API.Middlewares
{
    public class ErrorHandlerMiddleware : IMiddleware
    {
        private readonly IDataStorage _dataStorage;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(
            IDataStorage dataStorage,
            IDateTimeProvider dateTimeProvider,
            ILogger<ErrorHandlerMiddleware> logger)
        {
            _dataStorage = dataStorage;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var eventID = _dataStorage.GetNextEventID();
            var createdAt = _dateTimeProvider.UtcNow;
            
            if (context.Request.HasJsonContentType())
                context.Request.EnableBuffering();

            try
            {
                await next(context);
            }
            catch (Exception exception) when (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogDebug(exception, "The request was aborted by the client.");
                context.Response.Clear();
                context.Response.StatusCode = 499; //Client Closed Request
            }
            catch (Exception exception)
            {
                await WriteExceptionAsync(
                    exception,
                    context.Request,
                    eventID,
                    createdAt);

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(GetErrorDetails(exception, eventID));
            }
        }

        private async Task WriteExceptionAsync(Exception exception, HttpRequest request, int eventID, DateTime createdAt)
        {
            try
            {
                var body = await GetBody(request);
                await _dataStorage.WriteExceptionAsync(
                        exception,
                        eventID,
                        createdAt,
                        request.Path.Value ?? string.Empty,
                        body?.ToString() ?? string.Empty,
                        request.QueryString.Value ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(eventID, exception, null);
                _logger.LogError(eventID, ex, "Can't save exception journal.");
                
            }
        }

        private async Task<object?> GetBody(HttpRequest request)
        {
            if (!request.HasJsonContentType())
                return null;
            request.Body.Position = 0;
            return await request.ReadFromJsonAsync<object>();
        }

        private ErrorDetails GetErrorDetails(Exception exception, int eventID)
        {
            switch (exception)
            {
                case SecureException secureException:
                    return  new ErrorDetails(secureException.Type, eventID.ToString(), new ErrorData(secureException.Message));
                default:
                    return new ErrorDetails("Exception", eventID.ToString(), new ErrorData($"Internal server error ID = {eventID}"));
            }
        }
    }
}
