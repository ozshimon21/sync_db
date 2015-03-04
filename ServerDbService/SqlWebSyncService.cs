using System.Data.SqlClient;
using System.ServiceModel;
using CommonUtils;
using DbServiceCommon;
using DbServiceCommon.Interfaces;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace DbServiceServerSide
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]
    public class SqlWebSyncService : RelationalWebSyncService, ISqlSyncContract
    {
        SqlSyncProvider dbProvider;
        protected override RelationalSyncProvider ConfigureProvider(string scopeName, string hostName)
        {
            SynchronizationHelper helper = new SynchronizationHelper();
            this.dbProvider = helper.ConfigureSqlSyncProvider(scopeName, hostName);
            return this.dbProvider;
        }

        #region ISqlSyncContract Members

        public DbSyncScopeDescription GetScopeDescription()
        {
            Log("GetSchema: {0}", this.peerProvider.Connection.ConnectionString);
           
            DbSyncScopeDescription scopeDesc = SqlSyncDescriptionBuilder.GetDescriptionForScope(SyncUtils.ScopeName, (SqlConnection)this.dbProvider.Connection);
            return scopeDesc;
        }

        #endregion
    }
}
