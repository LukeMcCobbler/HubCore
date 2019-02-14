namespace HubCore.Infrastructure
{
    public class HubInfoResult
    {
        public string Content { get; set; }
        public bool HasError { get; set; }
        public HubOperationException Error { get; set; }
        public LogId[] LogIds { get; set; }
    }
}