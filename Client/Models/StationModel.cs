using Interfaces.Models;

namespace Client.Models
{
    public class StationModel : BaseModel, IStationModel
    {
        private int m_id;
        private string m_name;
        private int m_number;

        public int Id
        {
            get { return m_id; }

            set
            {
                m_id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                OnPropertyChanged("Name");
            }
        }

        public int Number
        {
            get { return m_number; }

            set
            {
                m_number = value;
                OnPropertyChanged("Number");
            }
        }
    }
}
