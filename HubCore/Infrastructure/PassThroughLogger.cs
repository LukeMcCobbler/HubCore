using System;
using Newtonsoft.Json.Linq;

namespace HubCore.Infrastructure
{
    public class PassThroughLogger : ILogger
    {
        public JToken LogRequest<TParameterSummary>(Func<JToken> RequestDelegate, string infoTypeName, TParameterSummary parameterSummary)
        {
            try
            {
                var retval = RequestDelegate();
                return retval;
            }
            catch (HubOperationException hoe)
            {
                var error = new HubInfoResult() { Content = null, Error = hoe, HasError = true                };
                return JToken.FromObject(error);

                }
            catch (Exception otherTypeException)
            {
                var wrappedException = new HubOperationException($"Error when trying to access information {infoTypeName} with parameters {JToken.FromObject(parameterSummary)}", otherTypeException);
                var error = new HubInfoResult() { Content = null, Error = wrappedException, HasError = true };
                return JToken.FromObject(error);

            }
        }
    }
}