using System.Net;

namespace Authentication.Shared.Dto;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public List<ApiError> Errors { get; set; }
    public List<ApiWarning> Warnings { get; set; }

    public bool HasError => Errors.Count > 0;
    public bool HasWarning => Warnings.Count > 0;
    public string GetFirstErrorMessage() => Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty;
    public int GetFirtsErrorCode() => Errors.FirstOrDefault().ErrorCode;

    public HttpStatusCode? StatusCode { get; set; }

public static ApiResponse<T> Success(T response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
        return new ApiResponse<T> { Data = response, Errors = new List<ApiError>(), Warnings = new List<ApiWarning>(), StatusCode = httpStatusCode};
    }

    public static ApiResponse<T> Error(ApiError errors)
    {
        var errorList = new List<ApiError> { errors };
        return new ApiResponse<T> { Data = default(T), Errors = errorList, Warnings = new List<ApiWarning>() };
    }
    
    public static ApiResponse<T> Warning(ApiWarning warning)
    {
        var warnings = new List<ApiWarning> { warning };
        return new ApiResponse<T> { Data = default(T), Errors = new List<ApiError>(), Warnings = warnings };
    }

}

public class ApiError
{
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string StackTrace { get; set; }
}

public class ApiWarning
{
    public string WarningCode { get; set; }
    public string WarningMessage { get; set; }
}