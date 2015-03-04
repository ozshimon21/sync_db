using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Client.Models;
using DAL;
using Interfaces.Models;

namespace Client.DataBase
{
    public class DataBaseHelper
    {
        private const string Catalog = "KaronDB";
        private string CreateDBStatement = "Create Table Stations(Id int IDENTITY(1,1) Primary key, Name nvarchar(255), Number int);";
        private string sample_row = "Insert into Stations(Name,Number) Values('oz',1);";
        const string SelectStationCommand = "Select * from Stations";
        private string m_path = @"C:";
        private SqlServerCeDatabaseHandler m_dbHandler;



        public DataBaseHelper()
        {
            m_dbHandler = new SqlServerCeDatabaseHandler(m_path, Catalog, "");
            if (m_dbHandler.CreateDataBase(true))
            {
                m_dbHandler.ExecuteSqlStatement(CreateDBStatement);
            }
        }



        public Task<IStationModel[]> GetStations()
        {
            return Task.Factory.StartNew(() =>
                                         {
                                             var result = m_dbHandler.Select(SelectStationCommand);

                                             var models = new List<IStationModel>();
                                             foreach (DataRow row in result.Rows)
                                             {
                                                 var model = new StationModel
                                                             {
                                                                     Id = int.Parse(row["Id"].ToString()),
                                                                     Name = row["Name"].ToString(),
                                                                     Number = int.Parse(row["Number"].ToString())
                                                             };
                                                 models.Add(model);
                                             }
                                             return models.ToArray();
                                         });
        }

        public void InsertStation(IStationModel model)
        {
            var command = String.Format("INSERT INTO Stations(Name,Number) values('{0}', {1})", model.Name, model.Number);
            m_dbHandler.Insert(command);
        }
    }
}
