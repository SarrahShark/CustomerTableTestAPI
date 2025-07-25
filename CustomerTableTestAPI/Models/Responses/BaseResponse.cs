namespace CustomerTableTestAPI.Models.Responses
{
    /// <summary>
    /// Standard response wrapper for API responses.
    /// </summary>
    /// <typeparam name="T">Type of the data returned in the response.</typeparam>
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public BaseResponse() { }

        /// <summary>
        /// Creates a success response with data and optional message.
        /// </summary>
        public BaseResponse(T data, string? message = null, bool success = true)
        {
            Data = data;
            Message = message;
            Success = success;
        }


        /// <summary>
        /// Creates a response with a message only, usually for errors.
        /// </summary>
        public BaseResponse(string message, bool success = false)
        {
            Message = message;
            Success = success;
        }
    }
}
