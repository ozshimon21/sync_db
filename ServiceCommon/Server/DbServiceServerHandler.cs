using System.Data.SqlClient;
using System.ServiceModel;
using CommonUtils;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace DbService.Server
{
    public class DbServiceServerHandler
    {


        public static SqlSyncProvider ConfigureSqlSyncProvider(string scopeName)
        {
            var provider = new SqlSyncProvider {ScopeName = scopeName};

            //Service should know list of adapters for given scope name.
            //Sample only shows for 'Sales' scope
            switch (scopeName.ToLower())
            {
                case "sales":

                    break;
                default:
                    throw new FaultException<WebSyncFaultException>(new WebSyncFaultException("Invalid SQL Scope name", null));
            }

            return provider;
        }

        public SqlSyncProvider ConfigureSqlSyncProvider(string scopeName, string hostName)
        {
            var provider = new SqlSyncProvider
            {
                ScopeName = scopeName
            };

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = hostName,
                IntegratedSecurity = true,
                InitialCatalog = "Moked",
                ConnectTimeout = 1
            };

            provider.Connection = new SqlConnection(builder.ToString());


            //create anew scope description and add the appropriate tables to this scope
            var scopeDesc = new DbSyncScopeDescription(scopeName/*SyncUtils.ScopeName*/);

            //class to be used to provision the scope defined above
            var serverConfig = new SqlSyncScopeProvisioning((SqlConnection)provider.Connection);

            //determine if this scope already exists on the server and if not go ahead and provision
            if (!serverConfig.ScopeExists(SyncUtils.ScopeName))
            {
                //add the approrpiate tables to this scope
                scopeDesc.Tables.Add(SqlSyncDescriptionBuilder.GetDescriptionForTable("Stations", (SqlConnection)provider.Connection));

                //note that it is important to call this after the tables have been added to the scope
                serverConfig.PopulateFromScopeDescription(scopeDesc);

                //indicate that the base table already exists and does not need to be created
                serverConfig.SetCreateTableDefault(DbSyncCreationOption.Skip);

                //provision the server
                serverConfig.Apply();
            }

            return provider;
        }

    }
}
