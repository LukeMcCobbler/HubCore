using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
namespace HubCore.Infrastructure
{
    public class CommonDataLayer : IDataLayer
    {
        private readonly string _connectionString;
        private readonly ISettingsManager _settingsManager;
        private readonly ApplicationInfo _applicationInfo;
        private readonly string _connectionStringKey;
        private readonly string CONNECTION_STRINGS_SECTION = "ConnectionStrings";

        public IEnumerable<TResult> Query<TResult>(string queryName, IEnumerable<QueryParameter> queryParameters)
        {
            var conn = GetConnection();
            var retval=conn.Query<TResult>(queryName, commandType: CommandType.StoredProcedure);
            return retval;
        }
        public virtual IDbConnection GetConnection()
        {
            var connectionString=_settingsManager.GetSetting(_applicationInfo.ApplicationName, CONNECTION_STRINGS_SECTION, _connectionStringKey);
            if(connectionString.StartsWith("MSSQL_"))
            {
                return new SqlConnection(connectionString.Substring(6));
            }
            throw new System.Exception($"Not a supported connection type:{connectionString.Substring(0,5)}");
        }
        public CommonDataLayer(ISettingsManager settingsManager,ApplicationInfo applicationInfo, string connectionStringKey)
        {            
            _settingsManager = settingsManager;
            _applicationInfo = applicationInfo;
            _connectionStringKey = connectionStringKey;
        }
        
        
    }
}