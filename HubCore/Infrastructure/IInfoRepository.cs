namespace HubCore
{
    public interface IInfoRepository
    {
        InfoContext GetInfoContext(string infoTypeName);
    }
}