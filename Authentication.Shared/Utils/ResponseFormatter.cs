using Authentication.Shared.Dto;
using Shared.Messages;

namespace Authentication.Shared.Utils;

public static class ResponseFormatter
{
    public static ApiResponse<T> MakeResponse<T>(this T value, IMessageHandlerService messageHandlingService)
    {
        var response =  ApiResponse<T>.Success(value);
        
        if (!messageHandlingService.HasErrors) return response;

        var errors = GetErrors(messageHandlingService);
        var warnings = GetWarnings(messageHandlingService);
            
        response.Errors = errors;
        response.Warnings = warnings;
        
        return response;
    }

    private static List<ApiError> GetErrors(IMessageHandlerService messageHandlingService)
        => messageHandlingService.GetErrors()
            .Select(e => new ApiError
            {
                ErrorCode = int.TryParse(e.Code, out var code) ? code : default(int),
                ErrorMessage = e.Message,
                StackTrace = e.StackTrace
            })
            .ToList();
    
    private static List<ApiWarning> GetWarnings(IMessageHandlerService messageHandlingService)
        => messageHandlingService.GetWarnings()
            .Select(e => new ApiWarning() { WarningCode = e.Code, WarningMessage = e.Message })
            .ToList();
}