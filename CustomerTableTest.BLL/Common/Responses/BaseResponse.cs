namespace CustomerTableTest.Common.Responses;
public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<ErrorItem>? Errors { get; set; }

        public static BaseResponse<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message ?? string.Empty };

        public static BaseResponse<T> Fail(string message, List<ErrorItem>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }

    public class ErrorItem
    {
        public string? Code { get; set; }
        public string? Field { get; set; }
        public string Description { get; set; } = string.Empty;
    }


