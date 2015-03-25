using System;
using System.ServiceModel;
using CommonUtils;
using DbService.Common;
using DbService.Interfaces;
using Microsoft.Synchronization.Data;

namespace DbService.Client
{
    public class SqlCeSyncProviderProxy : RelationalProviderProxy
    {
        private ISqlCeSyncContract m_clientProxy;
        private RelationalSyncProvider m_clientProvider;

        private const string client_default_database_path = @"C:\KaronDB.sdf";
        
        public SqlCeSyncProviderProxy(string clientEndpoint, string scopeName, string ceDatabaseName):
            base(clientEndpoint,scopeName, ceDatabaseName)
        {
           
        }

        public SqlCeSyncProviderProxy(string clientEndpoint, string scopeName) :
            base(clientEndpoint, scopeName, client_default_database_path)
        {
            
        }

        public override void CreateProxy()
        {
            WSHttpBinding binding = new WSHttpBinding
                                    {
                                            ReaderQuotas = {MaxArrayLength = 100000},
                                            MaxReceivedMessageSize = 10485760,                                          
                                    };

            NetTcpBinding netTcpBinding = new NetTcpBinding
                                          {
                                              ReaderQuotas = { MaxArrayLength = 100000 },
                                              MaxReceivedMessageSize = 10485760,
                                          };

         
            ChannelFactory<ISqlCeSyncContract> factory = new ChannelFactory<ISqlCeSyncContract>(binding, ClientServiceEndpoint);
            base.proxy = factory.CreateChannel();
            m_clientProxy = base.proxy as ISqlCeSyncContract;


            SqlDatabase clientDatabase = new SqlDatabase { Location = hostName };

            this.proxy.Initialize(scopeName, clientDatabase.ConnectionString);
        }




        public void CreateScopeDescription(DbSyncScopeDescription scopeDescription)
        {
            m_clientProxy.CreateScopeDescription(scopeDescription);
        }

        public void DeleteScopeDescription(String scope)
        {
            m_clientProxy.DeleteScopeDescription(scope);
        }



        public DbSyncScopeDescription GetScopeDescription(string scope)
        {
            return m_clientProxy.GetScopeDescription(scope);
        }

        public bool NeedsScope()
        {
            return m_clientProxy.NeedsScope();
        }
    }
}
