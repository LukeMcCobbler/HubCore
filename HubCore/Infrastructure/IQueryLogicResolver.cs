namespace HubCore.Infrastructure
{
    public interface IQueryLogicResolver
    {
        string PerformQuery(string QueryLogic,string[] InfoParameters);
    }
}