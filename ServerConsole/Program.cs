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
            const string TABLE_STATION = "Stations";

            const string SYNC_SCOPE = "Stations";
            const string CONNECTION_STRING_SERVER = "Data Source=localhost; Initial Catalog=Moked; Integrated Security=True";
            const string CONNECTION_STRING_CLIENT = @"C:\KaronDB.sdf";
            const string SQL_CE_SYNC_SERVICE_URI_METADATA = "http://192.168.5.10:8001/RelationalSyncContract/SqlCeSyncService/";
            const string SQL_CE_SYNC_SERVICE_URI_NET_TCP_REMOTE = "net.tcp://192.168.5.6:8008/RelationalSyncContract/SqlCeSyncService";
            const string SQL_CE_SYNC_SERVICE_URI_NET_TCP_LOCAL = "net.tcp://localhost:8008/RelationalSyncContract/SqlCeSyncService/";

            Console.WriteLine("Try Connecting Server... {0}", CONNECTION_STRING_SERVER);
            var serverConnection = new SqlConnection(CONNECTION_STRING_SERVER);
            serverConnection.Open();          
            serverConnection.Close();
            Console.WriteLine("Server Connected Successfuly!");

            Console.WriteLine("Create server provider... for Scope name:{0}", SYNC_SCOPE);
            var localProvider = new SqlSyncProvider(SYNC_SCOPE, serverConnection)
            {
                MemoryDataCacheSize = 1,
                BatchingDirectory = @"C:\SyncBatchFolder"
            };

            localProvider.BatchSpooled += localProvider_BatchSpooled;
            localProvider.BatchApplied += localProvider_BatchApplied;


            var scopeDesc = new DbSyncScopeDescription(SYNC_SCOPE);
            var tableDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable(TABLE_STATION, serverConnection);
            scopeDesc.Tables.Add(tableDesc);

            Console.WriteLine("Check whether Server scope:[{0}] is exists...",SYNC_SCOPE);
            var serverProvision = new SqlSyncScopeProvisioning(serverConnection);
            var isServerScopeExist = serverProvision.ScopeExists(SYNC_SCOPE);
            Console.WriteLine("Server scope:[{0}] is {1}", SYNC_SCOPE, isServerScopeExist ? "exist" : "is not exist");
            if (!isServerScopeExist)
            {
                Console.WriteLine("start provisioning server with scope:[{0}] ...", SYNC_SCOPE);
                serverProvision.SetCreateTableDefault(DbSyncCreationOption.Skip);
                serverProvision.PopulateFromScopeDescription(scopeDesc);
                serverProvision.Apply();
            }

            Console.WriteLine("Try connecting client by proxy URI: [{0}]", SQL_CE_SYNC_SERVICE_URI_NET_TCP_LOCAL);
            var clientProxy = new SqlCeSyncProviderProxy(SQL_CE_SYNC_SERVICE_URI_NET_TCP_LOCAL, SYNC_SCOPE);
            clientProxy.CreateProxy();
            Console.WriteLine("Client proxy Connected Successfuly!");


            Console.WriteLine("Check whether Client scope:[{0}] is exists...", SYNC_SCOPE);
            var isClientNeedScope = clientProxy.NeedsScope();
            Console.WriteLine("Client scope:[{0}] is {1}", SYNC_SCOPE, isServerScopeExist ? "exist" : "is not exist");
            if (isClientNeedScope)
            {
                Console.WriteLine("start provisioning client with scope:[{0}] ...", SYNC_SCOPE);

                Console.WriteLine("Getting scope description from Server:[{0}]", CONNECTION_STRING_SERVER);
              
                
                clientProxy.CreateScopeDescription(scopeDesc);
            }

            var orchestrator = new SyncOrchestrator
                                            {
                                                    LocalProvider = localProvider,
                                                    RemoteProvider = clientProxy,
                                                    Direction = SyncDirectionOrder.Download,                                            
                                            };

            var syncStats = orchestrator.Synchronize();

            // print statistics
            Console.WriteLine("Start Time: " + syncStats.SyncStartTime);
            Console.WriteLine(String.Empty);
            Console.WriteLine("Download Changes Applied: " + syncStats.DownloadChangesApplied);
            Console.WriteLine("Download Changes Failed: " + syncStats.DownloadChangesFailed);
            Console.WriteLine("Download Changes Total: " + syncStats.DownloadChangesTotal);
            Console.WriteLine(String.Empty);
            Console.WriteLine("Upload Changes Applied: " + syncStats.UploadChangesApplied);
            Console.WriteLine("Upload Changes Failed: " + syncStats.UploadChangesFailed);
            Console.WriteLine("Upload Changes Total: " + syncStats.UploadChangesTotal);
            Console.WriteLine(String.Empty);
            Console.WriteLine("Complete Time: " + syncStats.SyncEndTime);
            Console.WriteLine(String.Empty);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

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
