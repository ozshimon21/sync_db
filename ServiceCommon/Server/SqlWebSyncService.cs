using System.Data.SqlClient;
using System.ServiceModel;
using CommonUtils;
using DbService.Common;
using DbService.Interfaces;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace DbService.Server
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]
    public class SqlWebSyncService : RelationalWebSyncService, ISqlSyncContract
    {
        SqlSyncProvider dbProvider;
        protected override RelationalSyncProvider ConfigureProvider(string scopeName, string hostName)
        {
            var helper = new DbServiceServerHandler();
            this.dbProvider = helper.ConfigureSqlSyncProvider(scopeName, hostName);
            return this.dbProvider;
        }     

        #region ISqlSyncContract Members

        public void CreateScopeDescription(DbSyncScopeDescription scopeDescription)
        {
            Log("CreateScopeDescription: {0}", this.peerProvider.Connection.ConnectionString);
            SqlSyncScopeProvisioning prov = new SqlSyncScopeProvisioning((SqlConnection)this.peerProvider.Connection, scopeDescription);
            prov.Apply();
        }

        public DbSyncScopeDescription GetScopeDescription()
        {
            Log("GetSchema: {0}", this.peerProvider.Connection.ConnectionString);

            DbSyncScopeDescription scopeDesc = SqlSyncDescriptionBuilder.GetDescriptionForScope(SyncUtils.ScopeName, (SqlConnection)this.dbProvider.Connection);
            return scopeDesc;
        }

        public bool NeedsScope()
        {
            Log("NeedsSchema: {0}", this.peerProvider.Connection.ConnectionString);
            SqlSyncScopeProvisioning prov = new SqlSyncScopeProvisioning((SqlConnection)this.peerProvider.Connection);

            return !prov.ScopeExists(this.peerProvider.ScopeName);
        }

        #endregion
    }
}
