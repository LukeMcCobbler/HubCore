using System.Collections.Generic;

namespace HubCore.Infrastructure
{
    public interface IDataLayer
    {
        IEnumerable<TResult> Query<TResult>(string queryName, IEnumerable<QueryParameter> queryParameters);
    }
}