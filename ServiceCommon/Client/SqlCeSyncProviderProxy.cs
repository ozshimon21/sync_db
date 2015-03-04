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
        ISqlCeSyncContract ceProxy;
        DbSyncScopeDescription scopeDescription;
        public SqlCeSyncProviderProxy(string scopeName, string ceDatabaseName, DbSyncScopeDescription scopeDescription): base(scopeName, ceDatabaseName)
        {
            this.scopeDescription = scopeDescription;
        }


        protected override void CreateProxy()
        {
            WSHttpBinding binding = new WSHttpBinding
                                    {
                                            ReaderQuotas = {MaxArrayLength = 100000},
                                            MaxReceivedMessageSize = 10485760
                                    };
            ChannelFactory<ISqlCeSyncContract> factory = new ChannelFactory<ISqlCeSyncContract>(binding, new EndpointAddress(SyncUtils.CeSyncServiceUri));
           // base.proxy = factory.CreateChannel();
            //this.ceProxy  = base.proxy as ISqlCeSyncContract;
        }

        //
        //        public override void BeginSession(Microsoft.Synchronization.SyncProviderPosition position, Microsoft.Synchronization.SyncSessionContext syncSessionContext)
        //        {
        //            base.BeginSession(position, syncSessionContext);
        //        }


//
//        public void CreateScopeDescription(DbSyncScopeDescription scopeDescription)
//        {
//            this.ceProxy.CreateScopeDescription(scopeDescription);
//        }
//
//        public bool NeedsScope()
//        {
//            return this.ceProxy.NeedsScope();
//        }
//
//        public void GenerateSnapshot(String destinationFileName)
//        {
//            this.ceProxy.GenerateSnapshot(destinationFileName);
//        }
    }
}
