using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using Microsoft.Synchronization.Data.SqlServerCe;
using DbService;
using DbService.Client;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlCeSyncProviderProxy clientProxy;
            const string tableName = "Stations";
            const string tableName2 = "Flight";

            const string STATIONSCOPE = "StationScope2";
            const string CONNECTION_STRING_SERVER = "Data Source=localhost; Initial Catalog=Moked; Integrated Security=True";
            const string CONNECTION_STRING_CLIENT = @"C:\KaronDB.sdf";
            const string SQL_CE_SYNC_SERVICE_URI = "http://192.168.5.6:8000/RelationalSyncContract/SqlCeSyncService/";
            const string SQL_CE_SYNC_SERVICE_URI_NET_TCP = "net.tcp://192.168.5.6:8000/RelationalSyncContract/SqlCeSyncService";

            

            var serverConn = new SqlConnection(CONNECTION_STRING_SERVER);
            
            var scopeDesc = new DbSyncScopeDescription(STATIONSCOPE);          
            var tableDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable(tableName, serverConn);
            var tableDesc2 = SqlSyncDescriptionBuilder.GetDescriptionForTable(tableName2, serverConn);
            scopeDesc.Tables.Add(tableDesc);
            scopeDesc.Tables.Add(tableDesc2);


//            SqlSyncScopeProvisioning serverProvision = new SqlSyncScopeProvisioning(serverConn, scopeDesc);
//            serverProvision.SetCreateTableDefault(DbSyncCreationOption.Skip);
//            serverProvision.Apply();

            Console.WriteLine("Press any key");
            Console.ReadLine();

            clientProxy = new SqlCeSyncProviderProxy(SQL_CE_SYNC_SERVICE_URI_NET_TCP, STATIONSCOPE);
            clientProxy.CreateProxy();
            var result =  clientProxy.NeedsScope();

            Console.WriteLine(result);

//            Console.WriteLine("Create Scope Description if needed - Press any key");
//            Console.ReadLine();
//
//            clientProxy.CreateScopeDescription(scopeDesc);
//
//
//            Console.ReadLine();


            var localProvider = new SqlSyncProvider(STATIONSCOPE, serverConn);

            var clientDB = new SqlDatabase { Location = CONNECTION_STRING_CLIENT };

            var remoteProvider = new SqlCeSyncProvider(STATIONSCOPE,
                    new SqlCeConnection(clientDB.ConnectionString));

            localProvider.MemoryDataCacheSize = 1;
            localProvider.BatchingDirectory = @"C:\SyncBatchFolder";
            localProvider.BatchSpooled += localProvider_BatchSpooled;
            localProvider.BatchApplied += localProvider_BatchApplied;

            remoteProvider.MemoryDataCacheSize = 1;
            remoteProvider.BatchingDirectory = @"C:\SyncBatchFolder";
            remoteProvider.BatchSpooled += localProvider_BatchSpooled;
            remoteProvider.BatchApplied += localProvider_BatchApplied;

            var orchestrator = new SyncOrchestrator
                                            {
                                                    LocalProvider = localProvider,
                                                    RemoteProvider = remoteProvider,
                                                    Direction = SyncDirectionOrder.Download,
                                                   
                                            };

            SyncOperationStatistics stats = orchestrator.Synchronize();

        }

        static void localProvider_BatchApplied(object sender, DbBatchAppliedEventArgs e)
        {
            Console.WriteLine("BatchApplied event fired: Details");
            Console.WriteLine("\tDestination Database   :" + ((RelationalSyncProvider)sender).Connection.Database);
            Console.WriteLine("\tBatch Number           :" + e.CurrentBatchNumber);
            Console.WriteLine("\tTotal Batches To Apply :" + e.TotalBatchesToApply);
        }

        static void localProvider_BatchSpooled(object sender, DbBatchSpooledEventArgs e)
        {
            Console.WriteLine("BatchSpooled event fired: Details");
            Console.WriteLine("\tSource Database :" + ((RelationalSyncProvider)sender).Connection.Database);
            Console.WriteLine("\tBatch Name      :" + e.BatchFileName);
            Console.WriteLine("\tBatch Size      :" + e.DataCacheSize);
            Console.WriteLine("\tBatch Number    :" + e.CurrentBatchNumber);
            Console.WriteLine("\tTotal Batches   :" + e.TotalBatchesSpooled);
            Console.WriteLine("\tBatch Watermark :" + ReadTableWatermarks(e.CurrentBatchTableWatermarks));
        }

        private static string ReadTableWatermarks(Dictionary<string, ulong> dictionary)
        {
            var builder = new StringBuilder();
            var dictionaryClone = new Dictionary<string, ulong>(dictionary);
            foreach (var kvp in dictionaryClone)
            {
                builder.Append(kvp.Key).Append(":").Append(kvp.Value).Append(",");
            }
            return builder.ToString();
        }   
    }
}
