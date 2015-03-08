using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;
using DbService.Common;
using DbService.Interfaces;
using DbService.Server;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using Microsoft.Synchronization.Data.SqlServerCe;

namespace DbService.Client
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SqlCeWebSyncService : RelationalWebSyncService, ISqlCeSyncContract
    {
        SqlCeSyncProvider dbProvider;

        #region ISqlSyncContract Members

        protected override RelationalSyncProvider ConfigureProvider(string scopeName, string clientStringConn)
        {
            var helper = new DbServiceClientHandler();

            SqlCeConnection clientConn = new SqlCeConnection(clientStringConn);
            dbProvider = helper.ConfigureCeSyncProvider(scopeName, clientConn);

            return dbProvider;
        }

        public void CreateScopeDescription(DbSyncScopeDescription scopeDescription)
        {
            // create CE provisioning object based on the ProductsScope
            SqlCeSyncScopeProvisioning clientProvision =
                    new SqlCeSyncScopeProvisioning(dbProvider.Connection as SqlCeConnection, scopeDescription);

            // starts the provisioning process
            clientProvision.Apply();
        }

        public void DeleteScopeDescription(String scopeName)
        {
            // create CE provisioning object based on the ProductsScope

            SqlCeSyncScopeDeprovisioning clientDeprovision =
                    new SqlCeSyncScopeDeprovisioning(dbProvider.Connection as SqlCeConnection);

            // starts the provisioning process
            clientDeprovision.DeprovisionScope(scopeName);
        }

        public DbSyncScopeDescription GetScopeDescription(string ScopeName)
        {
            Log("GetSchema: {0}", this.peerProvider.Connection.ConnectionString);

            var scopeDesc = SqlCeSyncDescriptionBuilder.GetDescriptionForScope(ScopeName, dbProvider.Connection as SqlCeConnection);
            return scopeDesc;
        }

        public bool NeedsScope()
        {
            Log("NeedsSchema: {0}", this.peerProvider.Connection.ConnectionString);

            SqlCeSyncScopeProvisioning prov = null;
            if (dbProvider == null || dbProvider.Connection == null) return false;

            prov = new SqlCeSyncScopeProvisioning((SqlCeConnection)dbProvider.Connection);

            return !prov.ScopeExists(dbProvider.ScopeName);
        }

        #endregion

    }
}
