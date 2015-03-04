using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace DAL
{
    public class SqlServerDatabaseHandler
    {
        public string ConnectionString { get; set; }
        public string MachineName { get; set; }
        public string Catalog { get; set; }

        public SqlServerDatabaseHandler(string machineName, string catalog)
        {
            MachineName = machineName;
            Catalog = catalog;

            ConnectionString = CreateConnection(MachineName, Catalog);
        }

        public string CreateConnection(string machineName, string catalog)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder();

                builder["Data Source"] = machineName;
                builder["integrated Security"] = true;
                builder["Initial Catalog"] = catalog;


                return builder.ConnectionString;

            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

       
        public DataTable Select(string command)
        {
            var dataAdapter = new SqlDataAdapter(command, ConnectionString);
            var dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            return dataTable;
        }

        public void Insert(string command)
        {
            var dataAdapter = new SqlDataAdapter();
            var sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            dataAdapter.InsertCommand = new SqlCommand(command, sqlConnection);
            dataAdapter.InsertCommand.ExecuteNonQuery();
        }
    }
}
