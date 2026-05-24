using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RC.Shared.Models.Results
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }

        public static ApiResponse<T> Ok(T data) => new()
        {
            Success = true,
            Data = data
        };

        public static ApiResponse<T> Fail(string error) => new()
        {
            Success = false,
            Error = error
        };
    }
}
