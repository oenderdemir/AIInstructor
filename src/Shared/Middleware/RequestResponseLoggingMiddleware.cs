using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Text.Json;
using AIInstructor.src.Auth.DTO;
using AIInstructor.src.Shared.Exceptions;

namespace AIInstructor.src.Shared.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopWatch = Stopwatch.StartNew();
            Guid guid = Guid.NewGuid();

            context.Items["RequestGuid"] = guid;
            context.Request.EnableBuffering();

            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                var request = context.Request;
                var body = await ReadRequestBody(request);

                if (request.Path.Value.Contains("/api/Auth/login"))
                {
                    body = MaskSensitiveData(body);
                }

                LogRequest(context, guid, body);

                context.Request.Body.Position = 0;

                await _next(context);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                LogResponse(guid, responseBody);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                context.Response.Body = originalBodyStream; // HATA DURUMUNDA STREAMİ GERİ AL!
                await HandleException(context, ex);
            }
            finally
            {
                context.Response.Body = originalBodyStream; // Normalde de geri alıyoruz
                stopWatch.Stop();
                LogElapsedTime(guid, stopWatch.ElapsedMilliseconds);
            }
        }


        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }

        private string MaskSensitiveData(string body)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var model = System.Text.Json.JsonSerializer.Deserialize<LoginRequestDTO>(body, options);
                if (model != null && !string.IsNullOrEmpty(model.Parola))
                {
                    model.Parola = new string('*', model.Parola.Length);
                }
                return System.Text.Json.JsonSerializer.Serialize(model);
            }
            catch (Exception ex)
            {
                Log.Warning($"Failed to mask sensitive data: {ex.Message}");
                return body; // Return original if masking fails
            }
        }

        private void LogRequest(HttpContext context, Guid guid, string body)
        {
            var logData = new
            {
                Type = "Request",
                RequestGuid = guid,
                Timestamp = DateTime.UtcNow,
                Schema = context.Request.Scheme,
                Host = context.Request.Host.Value,
                Path = context.Request.Path,
                Method = context.Request.Method,
                QueryString = context.Request.QueryString.ToString(),
                RequestBody = body,
                ClientIP = context.Connection.RemoteIpAddress?.ToString(),
                Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
            };

            Log.Information("{@LogData}", logData);
        }

        private void LogResponse(Guid guid, string responseBody)
        {
            var logData = new
            {
                Type = "Response",
                RequestGuid = guid,
                Timestamp = DateTime.UtcNow,
                ResponseBody = responseBody
            };

            Log.Information("{@LogData}", logData);
        }


        private void LogElapsedTime(Guid guid, long elapsedTime)
        {
            var logData = new
            {
                Type = "ExecutionTime",
                RequestGuid = guid,
                Timestamp = DateTime.UtcNow,
                ElapsedTime = elapsedTime + " ms"
            };

            Log.Information("{@LogData}", logData);
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            var requestGuid = context.Items["RequestGuid"] as Guid? ?? Guid.Empty;
            var code = "system_error";
            var statuscode = 500;

            var logData = new
            {
                Type = "Exception",
                RequestGuid = requestGuid,
                Timestamp = DateTime.UtcNow,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                StatusCode = statuscode
            };


            Log.Error("{@LogData}", logData);

            if (ex is BaseException baseException)
            {
                code = baseException.ErrorCode.ToString();
                statuscode = baseException.ErrorCode;
            }

            var errorMessageObject = new { Message = ex.Message, Code = code };
            var errorMessage = JsonConvert.SerializeObject(errorMessageObject);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statuscode;

            return context.Response.WriteAsync(errorMessage);
        }
    }
}
