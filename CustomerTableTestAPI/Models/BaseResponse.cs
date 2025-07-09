namespace CustomerTableTestAPI.Models
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; }
    }
    public class BaseResponse<T> : BaseResponse
    {
        public BaseResponse()
        {
        }
        public BaseResponse(T response)
        {
            Success = true;
            Data = response;
        }
        public T Data { get; set; }
    }
}
