using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;

namespace CommonUtils
{
    /// <summary>
    /// Static class holding some const strings
    /// </summary>
    public static class SyncUtils
    {
        public static string ScopeName = "stations";
        public static string[] SyncAdapterTables = new string[] { "orders", "order_details" };
        public static string[] SyncAdapterTablePrimaryKeys = new string[] { "order_id", "order_Details_id" };
        public static int TombstoneAgingInHours = 10;
        public static string SqlSyncServiceUri = "http://localhost:8000/RelationalSyncContract/SqlSyncService/";
        public static string SqlCeSyncServiceUri = "http://localhost:8000/RelationalSyncContract/SqlCeSyncService/";
        public static string SqlCeSyncServiceUriNetTcp = "net.tcp://localhost:8008/RelationalSyncContract/SqlCeSyncService/";
    }

    /// <summary>
    /// Enum that denotes what mode a client is added to the gui
    /// </summary>
    public enum CEDatabaseCreationMode
    {
        FullInitialization,
        SnapshotInitialization,
    }

    /// <summary>
    /// Utility class that holds information about a single CE client database
    /// </summary>
    public class SqlDatabase
    {
        string dbName;
        SqlCeConnection connection;

        public string Name
        {
            get { return dbName; }
            set { dbName = value; }
        }
        string dbLocation;

        public string Location
        {
            get { return dbLocation; }
            set { dbLocation = value; }
        }
        CEDatabaseCreationMode creationMode;

        public CEDatabaseCreationMode CreationMode
        {
            get { return creationMode; }
            set { creationMode = value; }
        }

        public SqlCeConnection Connection
        {
            //string conn = @"DataSource='C:\\KaronDB.sdf'; Password=''";
            get 
            {
                if (connection == null)
                {
                    connection = new SqlCeConnection(String.Format(@"DataSource='{0}'; Password='{1}'", dbLocation,string.Empty));
                } 




                return connection;
            }
        }

        public string ConnectionString
        {
            get { return Connection.ConnectionString; }
        }
    }
}
