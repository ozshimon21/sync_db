using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CommonUtils;
using DbService.Client;
using DbService.Interfaces;
using DbService.Server;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServerCe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbService.Tests
{
    [TestClass]
    public class SqlCeWebSyncServiceTest
    {
        private const string DATABASE_LOCATION = @"C:\KaronDB.sdf";
        private const string TABLE = "Stations";
        private const string SCOPE_NAME = "StationsScope";
        private const string SERVER_CONN = "Data Source=localhost; Initial Catalog=SyncDB; Integrated Security=True";


        private SqlDatabase clientDatabase;
        private SqlCeSyncProviderProxy m_clientProvider;

        [TestInitialize]
        public void TestInit()
        {
            clientDatabase = new SqlDatabase { Location = DATABASE_LOCATION };

            try
            {
                m_clientProvider = new SqlCeSyncProviderProxy(SyncUtils.SqlCeSyncServiceUri, SCOPE_NAME, clientDatabase.ConnectionString);              
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [TestMethod]
        public void NeedScopeTest() 
        {
            var result = m_clientProvider.NeedsScope();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CreateScopeDescriptionTest()
        {
            DbSyncScopeDescription scopeDesc = new DbSyncScopeDescription(SCOPE_NAME);
            // get the description of the Products table from SyncDB dtabase
            var tableDesc = SqlCeSyncDescriptionBuilder.GetDescriptionForTable(TABLE, clientDatabase.Connection);
            // add the table description to the sync scope definition
            scopeDesc.Tables.Add(tableDesc);

            try
            {
                m_clientProvider.CreateScopeDescription(scopeDesc);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception while provisioning client - CreateScopeDescription:" + ex.Message);
            }           
        }

        [TestMethod]
        public void DeleteScopeDescriptionTest()
        {
            try
            {
                m_clientProvider.DeleteScopeDescription(SCOPE_NAME);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception while provisioning client - DeleteScopeDescription:" + ex.Message);
            }
        }

        [TestMethod]
        public void GetScopeTest()
        {
            var result = m_clientProvider.GetScopeDescription(SCOPE_NAME);
            Assert.IsNotNull(result);
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            if (m_clientProvider != null)
            {
                m_clientProvider.CloseProxy();
                m_clientProvider.Dispose();
            }
                
        }
    }
}
