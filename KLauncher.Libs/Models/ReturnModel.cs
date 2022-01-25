namespace KLauncher.Libs.Models
{
    public sealed class ReturnModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "未知原因";
        public T Data { get; set; }
    }
}