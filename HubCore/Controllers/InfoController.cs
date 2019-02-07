using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HubCore.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HubCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private IInfoRepository _infoRepository;
        private Dictionary<QueryLogicType, IQueryLogicResolver> _queryLogicResolvers;
        [Route("GetInfo/{infoTypeName}/{*infoParameters}")]
        public IActionResult GetInfo(string infoTypeName, params string[] infoParameters)
        {
            var infoContext = _infoRepository.GetInfoContext(infoTypeName);
            if(!_queryLogicResolvers.ContainsKey(infoContext.QueryLogicType))
            {
                throw new InfoProviderException($"Cannot find a way to resolve {infoContext.QueryLogicType}");
            }
            var queryLogicResolver = _queryLogicResolvers[infoContext.QueryLogicType];
            var result = queryLogicResolver.PerformQuery(infoContext.QueryLogic, infoParameters);
            return Ok(new HubInfoResult { Content = result, HasError = false, Error = null });
        }
        public InfoController(IInfoRepository infoRepository, Dictionary<QueryLogicType, IQueryLogicResolver> queryLogicResolvers)
        {
            _infoRepository = infoRepository;
            _queryLogicResolvers = queryLogicResolvers;
        }

    }
}