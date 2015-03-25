using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbService.Client;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbService.Tests
{
    [TestClass]
    public class SqlCeSyncProviderProxyTest
    {
        SqlCeSyncProviderProxy _clientProxy;
        const string TABLE_STATIONS = "Stations";
        const string TABLE_Flight = "Flight";

        const string STATION_SCOPE = "StationScope2";
        const string CONNECTION_STRING_SERVER = "Data Source=localhost; Initial Catalog=Moked; Integrated Security=True";
        const string CONNECTION_STRING_CLIENT = @"C:\KaronDB.sdf";
        const string SQL_CE_SYNC_SERVICE_URI_NET_TCP = "net.tcp://192.168.5.6:8000/RelationalSyncContract/SqlCeSyncService/";

        private SqlConnection _serverConnection;
        private DbSyncScopeDescription _serverScopeDescription;


        [TestInitialize]
        public void Initialize()
        {
            _serverConnection = new SqlConnection(CONNECTION_STRING_SERVER);

            _serverScopeDescription = SetupServerScope();

        }


        public DbSyncScopeDescription SetupServerScope()
        {
            var scopeDesc = new DbSyncScopeDescription(STATION_SCOPE);
            var tableDesc = SqlSyncDescriptionBuilder.GetDescriptionForTable(TABLE_STATIONS, _serverConnection);
            var tableDesc2 = SqlSyncDescriptionBuilder.GetDescriptionForTable(TABLE_Flight, _serverConnection);
            scopeDesc.Tables.Add(tableDesc);
            scopeDesc.Tables.Add(tableDesc2);

            return scopeDesc;
        }

        [TestMethod]
        public void ClientProxyTest()
        {
            try
            {
                _clientProxy = new SqlCeSyncProviderProxy(SQL_CE_SYNC_SERVICE_URI_NET_TCP, STATION_SCOPE);
                _clientProxy.CreateProxy();

               var result =  _clientProxy.NeedsScope();

               if (result == false) return;

                var provider = _clientProxy.ClientProvider;


            }
            catch (Exception ex)
            {

                Assert.Fail(ex.Message);
            }
        }



//        [TestMethod]
//        public void CreateScopeDescription(DbSyncScopeDescription scopeDescription)
//        {
//            _clientProxy.CreateScopeDescription(scopeDescription);
//        }
//
//        [TestMethod]
//        public void DeleteScopeDescription(String scope)
//        {
//            _clientProxy.DeleteScopeDescription(scope);
//        }
//
//
//        [TestMethod]
//        public DbSyncScopeDescription GetScopeDescription(string scope)
//        {
//            return _clientProxy.GetScopeDescription(scope);
//        }
//
//        [TestMethod]
//        public bool NeedsScope()
//        {
//            return _clientProxy.NeedsScope();
//        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_clientProxy != null)
            {
                _clientProxy.Dispose();
            }
        }


    }
}
