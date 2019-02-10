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
        private IQueryLogicResolverFactory _queryLogicResolverFactory;
        private ILogger _logger;
        [Route("GetInfo/{infoTypeName}/{*infoParameters}")]
        public IActionResult GetInfo(string infoTypeName, string infoParameters)
        {
            var retval = _logger.LogRequest(
                () =>
                {
                    var infoContext = _infoRepository.GetInfoContext(infoTypeName);
                    var queryLogicResolver = _queryLogicResolverFactory.getQueryLogicResolver(infoContext.QueryLogicType);
                    var infoParameterArray = (infoParameters ?? "").Split('/', StringSplitOptions.RemoveEmptyEntries);
                    var result = queryLogicResolver.PerformQuery(infoContext.QueryLogic, infoParameterArray);
                    return result;
                }, infoTypeName, new { getInfoParameters = infoParameters });
            return Ok(retval);
        }
        public InfoController(IInfoRepository infoRepository, IQueryLogicResolverFactory queryLogicResolverFactory, ILogger logger)
        {
            _infoRepository = infoRepository;
            _queryLogicResolverFactory = queryLogicResolverFactory;
            _logger = logger;
        }

    }
}