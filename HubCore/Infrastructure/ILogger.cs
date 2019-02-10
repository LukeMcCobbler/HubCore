using Newtonsoft.Json.Linq;
using System;

namespace HubCore.Infrastructure
{
    public interface ILogger
    {
        JToken LogRequest<TParameterSummary>(Func<JToken> RequestDelegate, string infoTypeName, TParameterSummary parameterSummary);
    }
}