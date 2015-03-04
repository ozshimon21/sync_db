using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Client.Auxiliary.Commands;
using Client.DataBase;
using Client.Models;
using Interfaces.Models;

namespace Client.ViewModels
{
    public class StationViewModel : BaseViewModel
    {
        private DataBaseHelper m_dbHelper;

        private bool m_canExecute = true;

        public ICommand LoadStationCommand { get; set; }
        public ICommand AddStationCommand { get; set; }
        public ICommand DeleteStationCommand { get; set; }
        public ICommand UpdateStationCommand { get; set; }

        public ICommand StartServiceCommand { get; set; }


        public ObservableCollection<IStationModel> StationList { get; set; }

        public void ChangeCanExecute(object obj)
        {
            m_canExecute = !m_canExecute;
        }

        public StationViewModel()
        {
            m_dbHelper = new DataBaseHelper();

            LoadStationCommand = new RelayCommand(LoadStationsList);
            AddStationCommand = new RelayCommand(AddStationsList);
            DeleteStationCommand = new RelayCommand(DeleteStationsList);
            UpdateStationCommand = new RelayCommand(UpdateStationsList);

            StartServiceCommand = new RelayCommand(StartServerService);

            //LoadStationCommand = new Apex.MVVM.AsynchronousCommand(LoadStationsList);

            //LoadStationCommand = new AsyncRelayCommand(LoadStationsList);

            StationList = new ObservableCollection<IStationModel>();

            IsExecuting = false;
        }

        private void UpdateStationsList(object obj)
        {
            throw new NotImplementedException();
        }

        private void DeleteStationsList(object obj)
        {
            throw new NotImplementedException();
        }

        private void AddStationsList(object obj)
        {
            StationModel model = new StationModel{Name = "test", Number = 5};
            m_dbHelper.InsertStation(model);

            LoadStationsList(null);
        }

        private void StartServerService(object obj)
        {
            //DbService.WebServiceHostLauncher.OpenServices();
        }

   

        public async void LoadStationsList(Object obj)
        {
            IsExecuting = true;

            var stations = await m_dbHelper.GetStations();

            StationList.Clear();
            foreach (var station in stations)
            {
                StationList.Add(station);
            }

            IsExecuting = false;
        }



        private bool m_isExecuting;
        public bool IsExecuting
        {
            get { return m_isExecuting;}
            set
            {
                m_isExecuting = value;
                OnPropertyChanged("IsExecuting");
            }
        }
    }

}
