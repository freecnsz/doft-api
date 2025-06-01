using System;

namespace doft.Application.DTOs;

public class ApiResponse<T>
{
    public bool Succeded { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    public ApiResponse(bool succeded, int statusCode, string message, T data)
    {
        Succeded = succeded;
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public static ApiResponse<T> Success(int statusCode, string message, T data)
    {
        return new ApiResponse<T>(true, statusCode, message, data);
    }

    public static ApiResponse<T> Error(int statusCode, string message, T data)
    {
        return new ApiResponse<T>(false, statusCode, message, data);
    }

    public static ApiResponse<T> NotFound(string message)
    {
        return new ApiResponse<T>(false, 404, message, default);
    }
    
    
}
