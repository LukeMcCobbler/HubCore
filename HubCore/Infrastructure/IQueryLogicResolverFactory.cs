namespace HubCore.Infrastructure
{
    public interface IQueryLogicResolverFactory
    {
        IQueryLogicResolver getQueryLogicResolver(QueryLogicType QueryLogicType);
    }
}