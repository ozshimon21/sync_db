using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DAL
{
    public class SqlServerCeDatabaseHandler
    {
        ConnectionState PreviousConnectionState = new ConnectionState();
        SqlCeConnection Connection;

        #region ConnectionString

        string ConnectionString()
        {
            return string.Format("DataSource='{0}'; Password='{1}'", FilePathAndName(), password);
        }

        string FilePathAndName()
        {
            return string.Format(Filepath + "\\" + Filename + ".sdf");//TODO:?
        }

        bool AccessPointReady()
        {
            return Filename.Length != 0;
        }

        #endregion

        #region Public Properties

        public string Filepath { get; set; }
        public string Filename { get; set; }
        private string password { get; set; }
        public string Password { set { password = value; } }

        #endregion


        #region Constructor

        public SqlServerCeDatabaseHandler()
        {
            Filename = "";
            Filepath = "";
            password = "";
        }

        public SqlServerCeDatabaseHandler(string Filepath, string Filename, string Password)
        {
            this.Filename = Filename;
            this.Filepath = Filepath;
            this.password = Password;
            InitiateConnection();           
        }

        void InitiateConnection()
        {
            Connection = new SqlCeConnection(ConnectionString());        
        }

        #endregion

        #region SqlExecution

        bool PrepareAndOpenConnection()
        {
            if (!AccessPointReady()) return false;
            if (!ConnectionExists()) InitiateConnection();
            OpenConnectionAndRememberPreviousState();
            return true;
        }

        bool ConnectionExists()
        {
            if (Connection == null) return false;
            return true;
        }

        void ChangeConnectionState(ConnectionState State)
        {
            try
            {
                if (State == ConnectionState.Open) Connection.Open();
                if (State == ConnectionState.Closed) Connection.Close();
            }
            catch (SqlCeException sqlexception)
            {
                MessageBox.Show(sqlexception.Message + "\n\n" + sqlexception.StackTrace, "SQL-error.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n\n" + exception.StackTrace, "Error.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void OpenConnectionAndRememberPreviousState()
        {
            PreviousConnectionState = Connection.State;
            if (Connection.State == ConnectionState.Closed) ChangeConnectionState(ConnectionState.Open);
        }

        void ReturnToPreviousConnectionState()
        {
            if (PreviousConnectionState != Connection.State) ChangeConnectionState(PreviousConnectionState);
        }

        public bool ExecuteSqlStatement(string SqlStatement)
        {
            Connection = new SqlCeConnection(ConnectionString());
            OpenConnectionAndRememberPreviousState();
            try
            {
                SqlCeCommand command = GetSqlCeCommand(Connection, SqlStatement);
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqlCeException sqlexception)
            {
                MessageBox.Show(sqlexception.Message + "\n\n" + sqlexception.StackTrace, "SQL-error.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n\n" + exception.StackTrace, "Error.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                ReturnToPreviousConnectionState();
            }
        }

        #endregion

        #region CheckDatabase

        public bool CheckDatabase()
        {
            return File.Exists(FilePathAndName());
        }

        #endregion

        #region CreateDataBase

        public bool CreateDataBase(bool CheckIfExists)
        {
            if (!AccessPointReady()) return false;
            if (CheckIfExists && CheckDatabase()) return false;
            File.Delete(FilePathAndName());
            var connectionString = ConnectionString();

            if (password != String.Empty)
                connectionString += "; Encrypt = TRUE;";

            SqlCeEngine engine = new SqlCeEngine(connectionString);
            try
            { 
                engine.CreateDatabase();          
                return true;
            }
            catch (SqlCeException sqlexception)
            {
                MessageBox.Show(sqlexception.Message + "\n\n" + sqlexception.StackTrace, "SQL-error.");
                return false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n\n" + exception.StackTrace, "Error.");
                return false;
            }
        }

        #endregion

        public DataTable Select(string selectStatement)
        {
            if (!AccessPointReady()) return null;

            SqlCeConnection connection = new SqlCeConnection(ConnectionString());
            try
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                SqlCeCommand command = GetSqlCeCommand(connection, selectStatement);
               
                if (connection.State == ConnectionState.Closed) connection.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(command);
                DataTable dataSet = new DataTable();
                adapter.Fill(dataSet);

                return dataSet;
            }
            catch (SqlCeException sqlexception)
            {
                MessageBox.Show(sqlexception.Message + "\n\n" + sqlexception.StackTrace, "SQL-error.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n\n" + exception.StackTrace, "Error.",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        SqlCeCommand GetSqlCeCommand(SqlCeConnection Connection, string SqlStatement)
        {
            SqlCeCommand command = new SqlCeCommand
                                   {
                                           Connection = Connection,
                                           CommandType = CommandType.Text,
                                           CommandText = SqlStatement
                                   };
            return command;
        }

        public void Insert(string command)
        {
            ExecuteSqlStatement(command);
        }
    }
}




