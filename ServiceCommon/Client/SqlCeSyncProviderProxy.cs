using System;
using System.ServiceModel;
using CommonUtils;
using DbService.Common;
using DbService.Interfaces;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;

namespace DbService.Client
{
    public class SqlCeSyncProviderProxy : RelationalProviderProxy
    {
        private ISqlCeSyncContract _clientProxy;

        private const string CLIENT_DEFAULT_DATABASE_PATH = @"C:\KaronDB.sdf";
        
        public SqlCeSyncProviderProxy(string clientEndpoint, string scopeName, string ceDatabaseName):
            base(clientEndpoint,scopeName, ceDatabaseName)
        {
           
        }

        public SqlCeSyncProviderProxy(string clientEndpoint, string scopeName) :
            base(clientEndpoint, scopeName, CLIENT_DEFAULT_DATABASE_PATH)
        {
            
        }

        public override void CreateProxy()
        {
//            WSHttpBinding binding = new WSHttpBinding
//                                    {
//                                            ReaderQuotas = {MaxArrayLength = 100000},
//                                            MaxReceivedMessageSize = 10485760,                                          
//                                    };

            var netTcpBinding = new NetTcpBinding
                                          {
                                              ReaderQuotas = { MaxArrayLength = 100000 },
                                              MaxReceivedMessageSize = 10485760,
                                              Security = new NetTcpSecurity() { Mode = SecurityMode.None}
                                          };


            var factory = new ChannelFactory<ISqlCeSyncContract>(netTcpBinding, ClientServiceEndpoint);
            base.proxy = factory.CreateChannel();
            _clientProxy = base.proxy as ISqlCeSyncContract;


            var clientDatabase = new SqlDatabase { Location = hostName };
            var conn = clientDatabase.ConnectionString;
            this.proxy.Initialize(scopeName, conn );

        }




        public void CreateScopeDescription(DbSyncScopeDescription scopeDescription)
        {
            _clientProxy.CreateScopeDescription(scopeDescription);
        }

        public void DeleteScopeDescription(String scope)
        {
            _clientProxy.DeleteScopeDescription(scope);
        }



        public DbSyncScopeDescription GetScopeDescription(string scope)
        {
            return _clientProxy.GetScopeDescription(scope);
        }

        public bool NeedsScope()
        {
            return _clientProxy.NeedsScope();
        }
    }
}
