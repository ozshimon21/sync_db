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
        private DbSyncScopeDescription m_scopeDescription;
        private EndpointAddress ClientServiceEndpoint { get; set; }
    

        public SqlCeSyncProviderProxy(string clientEndpoint, string scopeName, string ceDatabaseName):
            base(scopeName, ceDatabaseName)
        {
            ClientServiceEndpoint = new EndpointAddress(clientEndpoint);           
        }


        protected override void CreateProxy()
        {
            WSHttpBinding binding = new WSHttpBinding
                                    {
                                            ReaderQuotas = {MaxArrayLength = 100000},
                                            MaxReceivedMessageSize = 10485760
                                    };
            ChannelFactory<ISqlCeSyncContract> factory = new ChannelFactory<ISqlCeSyncContract>(binding, ClientServiceEndpoint);
            base.proxy = factory.CreateChannel();
            m_clientProxy = base.proxy as ISqlCeSyncContract;
        }


        public void CreateScopeDescription(DbSyncScopeDescription scopeDescription)
        {
            m_clientProxy.CreateScopeDescription(scopeDescription);
        }

        public DbSyncScopeDescription GetScopeDescription()
        {
            return m_clientProxy.GetScopeDescription();
        }

        public bool NeedsScope()
        {
            return m_clientProxy.NeedsScope();
        }
    }
}
