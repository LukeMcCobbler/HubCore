using System.Linq;
using HubCore.Infrastructure;
using System.Collections.Generic;

namespace HubCore
{
    public class DBInfoRepository : IInfoRepository
    {
        ApplicationInfo _applicationInfo;
        IDataLayer _dataLayer;
        private ISettingsManager _settingsManager;
        private readonly string QUERYNAMES_SECTION="QueryNames";
        private readonly string INFOCONTEXT_GLOBALQUERY = "GetAllInfoContext";

        public InfoContext GetInfoContext(string infoTypeName)
        {
            var infoContextGlobalListQuery = _settingsManager.GetSetting(_applicationInfo.ApplicationName, QUERYNAMES_SECTION, INFOCONTEXT_GLOBALQUERY);
            var globalList = _dataLayer.Query<InfoContext>(infoContextGlobalListQuery, new List<QueryParameter>());            
            return globalList.FirstOrDefault(infoContext => infoContext.InfoTypeName == infoTypeName);
        }
        public DBInfoRepository(IDataLayer dataLayer,ISettingsManager settingsManager,ApplicationInfo applicationInfo)
        {
            _applicationInfo = applicationInfo;
            _dataLayer = dataLayer;
            _settingsManager = settingsManager;
        }
    }
}