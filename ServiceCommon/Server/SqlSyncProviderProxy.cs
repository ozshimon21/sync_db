using System.ServiceModel;
using DbService.Common;
using DbService.Interfaces;
using Microsoft.Synchronization.Data;

namespace DbService.Server
{
    public class SqlSyncProviderProxy : RelationalProviderProxy
    {
        ISqlSyncContract dbProxy;
        public SqlSyncProviderProxy(string serverEndpoint, string scopeName, string hostName)
            : base(serverEndpoint,scopeName, hostName)
        { }

        public override void CreateProxy()
        {
            WSHttpBinding binding = new WSHttpBinding
                                    {
                                            ReaderQuotas = {MaxArrayLength = 10485760},
                                            MaxReceivedMessageSize = 10485760
                                    };
            ChannelFactory<ISqlSyncContract> factory = new ChannelFactory<ISqlSyncContract>(binding, new EndpointAddress("http://192.168.5.6:8000/RelationalSyncContract/SqlCeSyncService/"/*SyncUtils.SqlSyncServiceUri*/));
            base.proxy = factory.CreateChannel();
            this.dbProxy = base.proxy as ISqlSyncContract;
        }

        public DbSyncScopeDescription GetScopeDescription()
        {
            return this.dbProxy.GetScopeDescription();
        }
    }
}
