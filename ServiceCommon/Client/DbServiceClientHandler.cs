using System.Data.SqlServerCe;
using Microsoft.Synchronization.Data.SqlServerCe;

namespace DbService.Client
{
    public class DbServiceClientHandler
    {
        public SqlCeSyncProvider ConfigureCeSyncProvider(string scopeName,SqlCeConnection sqlCeConnection)
        {
            var provider = new SqlCeSyncProvider
                           {
                                   ScopeName = scopeName, 
                                   Connection = sqlCeConnection
                           };

            //1. Register the BatchSpooled and BatchApplied events. These are fired when a provider is either enumerating or applying changes in batches.
//            provider.BatchApplied += new EventHandler<DbBatchAppliedEventArgs>(provider_BatchApplied);
//            provider.BatchSpooled += new EventHandler<DbBatchSpooledEventArgs>(provider_BatchSpooled);

            return provider;
        }

        public SqlCeSyncProvider ConfigureCeSyncProvider(SqlCeConnection sqlCeConnection)
        {
            var provider = new SqlCeSyncProvider
            {
                Connection = sqlCeConnection
            };

            //1. Register the BatchSpooled and BatchApplied events. These are fired when a provider is either enumerating or applying changes in batches.
            //            provider.BatchApplied += new EventHandler<DbBatchAppliedEventArgs>(provider_BatchApplied);
            //            provider.BatchSpooled += new EventHandler<DbBatchSpooledEventArgs>(provider_BatchSpooled);

            return provider;
        }
    }
}
