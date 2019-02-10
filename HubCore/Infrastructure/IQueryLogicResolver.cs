using Newtonsoft.Json.Linq;

namespace HubCore.Infrastructure
{
    public interface IQueryLogicResolver
    {
        JToken PerformQuery(string QueryLogic,string[] InfoParameters);
    }
}