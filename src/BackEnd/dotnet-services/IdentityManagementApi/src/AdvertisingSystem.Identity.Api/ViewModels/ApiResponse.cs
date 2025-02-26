using System.Net;

namespace AdvertisingSystem.Identity.Api.ViewModels;

public class ApiResponse<T> where T : class
{
    public ApiResponse(HttpStatusCode status)
    {
        Status = status;
    }

    public ApiResponse(HttpStatusCode status, string message)
    {
        Status = status;
        Message = message;
    }

    public ApiResponse(HttpStatusCode status, string message, T data)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    public ApiResponse(T data)
    {
        Data = data;
        Status = HttpStatusCode.OK;
        Message = "The operation was successful";
    }

    public ApiResponse(HttpStatusCode status, string message, T data, Pagination? pagination, ApiError? error)
    {
        Status = status;
        Message = message;
        Data = data;
        Pagination = pagination;
        Error = error;
    }

    public HttpStatusCode Status { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public Pagination? Pagination { get; set; }
    public ApiError? Error { get; set; }
}

public struct ApiError
{
    public string Type { get; set; }
    public string Title { get; set; }
    public HttpStatusCode Status { get; set; }
    public string Detail { get; set; }
    public string Instance { get; set; }
}

public struct Pagination
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int PagesTotal { get; set; }
    public int TotalItems { get; set; }
}