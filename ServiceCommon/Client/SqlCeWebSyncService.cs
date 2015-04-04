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
        SqlCeSyncProvider _dbProvider;

        #region ISqlSyncContract Members

        protected override RelationalSyncProvider ConfigureProvider(string scopeName, string clientStringConn)
        {
            var clientConn = new SqlCeConnection(clientStringConn);

            _dbProvider = new SqlCeSyncProvider
            {
                ScopeName = scopeName,
                Connection = clientConn
            };

            _dbProvider.ApplyChangeFailed += dbProvider_ApplyChangeFailed;

            return _dbProvider;
        }

        void dbProvider_ApplyChangeFailed(object sender, DbApplyChangeFailedEventArgs e)
        {
            Log("dbProvider_ApplyChangeFailed: " + e.Error);
        }

        public void CreateScopeDescription(DbSyncScopeDescription scopeDescription)
        {
            // create CE provisioning object based on the ProductsScope
            var clientProvision =
                    new SqlCeSyncScopeProvisioning(_dbProvider.Connection as SqlCeConnection, scopeDescription);

            try
            {
                clientProvision.Apply();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
        }

        public void DeleteScopeDescription(String scopeName)
        {
            // create CE provisioning object based on the ProductsScope

            var clientDeprovision =
                    new SqlCeSyncScopeDeprovisioning(_dbProvider.Connection as SqlCeConnection);

            // starts the provisioning process
            //clientDeprovision.DeprovisionScope(scopeName);
            clientDeprovision.DeprovisionStore();
        }

        public DbSyncScopeDescription GetScopeDescription(string ScopeName)
        {
            Log("GetSchema: {0}", this.peerProvider.Connection.ConnectionString);

            var scopeDesc = SqlCeSyncDescriptionBuilder.GetDescriptionForScope(ScopeName, _dbProvider.Connection as SqlCeConnection);
            return scopeDesc;
        }

        public bool NeedsScope()
        {
            Log("NeedsSchema: {0}", this.peerProvider.Connection.ConnectionString);

            SqlCeSyncScopeProvisioning prov = null;
            if (_dbProvider == null || _dbProvider.Connection == null) return false;

            prov = new SqlCeSyncScopeProvisioning((SqlCeConnection)_dbProvider.Connection);

            return !prov.ScopeExists(_dbProvider.ScopeName);
        }

        #endregion

    }
}
