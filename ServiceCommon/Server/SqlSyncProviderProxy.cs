using System.ServiceModel;
using CommonUtils;
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

        protected override void CreateProxy()
        {
            WSHttpBinding binding = new WSHttpBinding
                                    {
                                            ReaderQuotas = {MaxArrayLength = 10485760},
                                            MaxReceivedMessageSize = 10485760
                                    };
            ChannelFactory<ISqlSyncContract> factory = new ChannelFactory<ISqlSyncContract>(binding, new EndpointAddress(SyncUtils.SqlSyncServiceUri));
            base.proxy = factory.CreateChannel();
            this.dbProxy = base.proxy as ISqlSyncContract;
        }

        public DbSyncScopeDescription GetScopeDescription()
        {
            return this.dbProxy.GetScopeDescription();
        }
    }
}
