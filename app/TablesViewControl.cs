using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Data.SqlClient;

namespace SyncApplication
{
    /// <summary>
    /// Custom control class that has two tabs to view the Orders and Order_Details table. Each new CE client
    /// added to the form with have this dynamic control added to its Controls and hence will display data from
    /// these two tables.
    /// </summary>
    public partial class TablesViewControl : UserControl
    {
        //use a delegate to load a dataset asynchronously
        internal delegate void ReadTableValuesAsyncDelegate(IDbConnection connection, string tableName);

        internal ReadTableValuesAsyncDelegate readTableValues;

        static object lockObject = new object();

        public TablesViewControl()
        {
            InitializeComponent();
            this.readTableValues = AsyncReadTableValueFromConnection;
        }

        public TablesViewControl(string name) : this()
        {
            this.name = name;
        }

        public DbDataAdapter ordersDataAdapter;
        public DbDataAdapter orderDetailsDataAdapter;
        public string name = "Server";

        //TODO fix asynxc bugs and make sure table values are populated asynchronoulsy
        public void ReadTableValues(IDbConnection connection, string tableName)
        {
            this.readTableValues.BeginInvoke(connection, tableName, EndAsyncReadTableValueFromConnection, this);
        }

        void EndAsyncReadTableValueFromConnection(IAsyncResult ar)
        {
            TablesViewControl tvc = ar.AsyncState as TablesViewControl;
            try
            {
                tvc.readTableValues.EndInvoke(ar);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Unable to read table values for '{0}'. Check you connection string and retry. Error: {1}", tvc.name, e.ToString()));
            }
        }

        /// <summary>
        /// Reads values for specified table name from specified connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        public void AsyncReadTableValueFromConnection(IDbConnection connection, string tableName)
        {
            string commandString = string.Format("Select order_id, {0} from {1}", (tableName == "orders") ? "order_date" : "order_details_id, product, quantity"
                                                                    , tableName);
            DbDataAdapter dataAdapter = null;
            if (connection is SqlCeConnection)
            {
                dataAdapter = new SqlCeDataAdapter(commandString, (SqlCeConnection)connection);
            }
            else
            {
                dataAdapter = new SqlDataAdapter(commandString, ((ICloneable)(SqlConnection)connection).Clone() as SqlConnection);
            }


            DataTable dataTable = new DataTable();
            lock (lockObject)
            {
                dataAdapter.Fill(dataTable);
            }

            if (tableName == "orders")
            {
                ordersDataAdapter = dataAdapter;
                //Set the Insert/Update/Delete commands for the Orders tables
                InitAdapterCommands("orders", ordersDataAdapter, connection);
                ordersDataGrid.DataSource = dataTable;
            }
            else
            {
                orderDetailsDataAdapter = dataAdapter;
                InitAdapterCommands("order_details", orderDetailsDataAdapter, connection);
                orderDetailsDataGrid.DataSource = dataTable;
            }
        }

        private void InitAdapterCommands(string tableName, DbDataAdapter adapter, IDbConnection connection)
        {
            DataTableMapping tableMapping = new DataTableMapping();
            tableMapping.SourceTable = "Table";
            if (tableName.Equals("orders", StringComparison.InvariantCultureIgnoreCase))
            {
                tableMapping.DataSetTable = "orders";
                tableMapping.ColumnMappings.Add("order_id", "order_id");
                tableMapping.ColumnMappings.Add("order_date", "order_date");
                adapter.TableMappings.Add(tableMapping);
                adapter.DeleteCommand = CastAndSetConnection(connection);
                adapter.DeleteCommand.CommandText = "DELETE FROM [orders] WHERE (([order_id] = @Original_order_id) AND ((@IsNull" +
                    "_order_date = 1 AND [order_date] IS NULL) OR ([order_date] = @Original_order_dat" +
                    "e)))";
                adapter.DeleteCommand.CommandType = CommandType.Text;
                adapter.DeleteCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_order_id", DbType.Int32));
                adapter.DeleteCommand.Parameters.Add(CheckAndCreateParameter(connection, "@IsNull_order_date", DbType.Int32));
                adapter.DeleteCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_order_date", DbType.DateTime));
                adapter.InsertCommand = new SqlCommand();
                adapter.InsertCommand = CastAndSetConnection(connection);
                adapter.InsertCommand.CommandText = "INSERT INTO [orders] ([order_id], [order_date]) VALUES (@order_id, @order_d" +
                    "ate)";
                adapter.InsertCommand.CommandType = CommandType.Text;
                adapter.InsertCommand.Parameters.Add(CheckAndCreateParameter(connection, "@order_id", DbType.Int32));
                adapter.InsertCommand.Parameters.Add(CheckAndCreateParameter(connection, "@order_date", DbType.DateTime));
                adapter.UpdateCommand = new SqlCommand();
                adapter.UpdateCommand = CastAndSetConnection(connection);
                adapter.UpdateCommand.CommandText = @"UPDATE [orders] SET [order_id] = @order_id, [order_date] = @order_date WHERE (([order_id] = @Original_order_id) AND ((@IsNull_order_date = 1 AND [order_date] IS NULL) OR ([order_date] = @Original_order_date)))";
                adapter.UpdateCommand.CommandType = CommandType.Text;
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@order_id", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@order_date", DbType.DateTime));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_order_id", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@IsNull_order_date", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_order_date", DbType.DateTime));
            }
            else
            {
                tableMapping.DataSetTable = "order_details";
                tableMapping.ColumnMappings.Add("order_id", "order_id");
                tableMapping.ColumnMappings.Add("order_details_id", "order_details_id");
                tableMapping.ColumnMappings.Add("product", "product");
                tableMapping.ColumnMappings.Add("quantity", "quantity");
                adapter.TableMappings.Add(tableMapping);
                adapter.DeleteCommand = CastAndSetConnection(connection);
                adapter.DeleteCommand.CommandText = @"DELETE FROM [order_details] WHERE (([order_id] = @Original_order_id) AND ([order_details_id] = @Original_order_details_id) AND ((@IsNull_product = 1 AND [product] IS NULL) OR ([product] = @Original_product)) AND ((@IsNull_quantity = 1 AND [quantity] IS NULL) OR ([quantity] = @Original_quantity)))";
                adapter.DeleteCommand.CommandType = CommandType.Text;
                adapter.DeleteCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_order_id", DbType.Int32));
                adapter.DeleteCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_order_details_id", DbType.Int32));
                adapter.DeleteCommand.Parameters.Add(CheckAndCreateParameter(connection, "@IsNull_product", DbType.Int32));
                adapter.DeleteCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_product", DbType.String));
                adapter.DeleteCommand.Parameters.Add(CheckAndCreateParameter(connection, "@IsNull_quantity", DbType.Int32));
                adapter.DeleteCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_quantity", DbType.Int32));
                adapter.InsertCommand = new SqlCommand();
                adapter.InsertCommand = CastAndSetConnection(connection);
                adapter.InsertCommand.CommandText = @"INSERT INTO [order_details] ([order_id], [order_details_id], [product], [quantity]) VALUES (@order_id, @order_details_id, @product, @quantity)";
                adapter.InsertCommand.CommandType = CommandType.Text;
                adapter.InsertCommand.Parameters.Add(CheckAndCreateParameter(connection, "@order_id", DbType.Int32));
                adapter.InsertCommand.Parameters.Add(CheckAndCreateParameter(connection, "@order_details_id", DbType.Int32));
                adapter.InsertCommand.Parameters.Add(CheckAndCreateParameter(connection, "@product", DbType.String));
                adapter.InsertCommand.Parameters.Add(CheckAndCreateParameter(connection, "@quantity", DbType.Int32));
                adapter.UpdateCommand = new SqlCommand();
                adapter.UpdateCommand = CastAndSetConnection(connection);
                adapter.UpdateCommand.CommandText = @"UPDATE [order_details] SET [order_id] = @order_id, [order_details_id] = @order_details_id, [product] = @product, [quantity] = @quantity WHERE (([order_id] = @Original_order_id) AND ([order_details_id] = @Original_order_details_id) AND ((@IsNull_product = 1 AND [product] IS NULL) OR ([product] = @Original_product)) AND ((@IsNull_quantity = 1 AND [quantity] IS NULL) OR ([quantity] = @Original_quantity)))";
                adapter.UpdateCommand.CommandType = CommandType.Text;
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@order_id", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@order_details_id", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@product", DbType.String));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@quantity", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_order_id", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_order_details_id", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@IsNull_product", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_product", DbType.String));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@IsNull_quantity", DbType.Int32));
                adapter.UpdateCommand.Parameters.Add(CheckAndCreateParameter(connection, "@Original_quantity", DbType.Int32));
            }
        }

        private static DbParameter CheckAndCreateParameter(IDbConnection connection, string paramName, DbType paramType)
        {
            DbParameter parameter = null;
            if (connection is SqlConnection)
            {
                parameter = new SqlParameter();
                parameter.DbType = paramType;
            }
            else
            {
                parameter = new SqlCeParameter();
                parameter.DbType = paramType;
            }
            parameter.ParameterName = paramName;
            return parameter;
        }

        private static DbCommand CastAndSetConnection(IDbConnection connection)
        {
            DbCommand command;
            if (connection is SqlCeConnection)
            {
                command = new SqlCeCommand();
                command.Connection = (SqlCeConnection)connection;
            }
            else
            {
                command = new SqlCommand();
                command.Connection = (SqlConnection)connection;
            }
            return command;
        }

        //Update table values
        public void UpdateValues()
        {
            try
            {
                if (this.ordersDataAdapter != null)
                {
                    this.ordersDataAdapter.Update(((DataTable)ordersDataGrid.DataSource).GetChanges());
                }
                if (this.orderDetailsDataAdapter != null)
                {
                    this.orderDetailsDataAdapter.Update((DataTable)orderDetailsDataGrid.DataSource);
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString(), string.Format("Error in updating Table changes in Client '{0}'", name), MessageBoxButtons.OK);
            }
        }
    }
}
