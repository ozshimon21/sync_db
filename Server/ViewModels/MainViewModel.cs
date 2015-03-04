namespace Server.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            StationViewModel = new StationViewModel();
        }

        public StationViewModel StationViewModel { get; set; }
    }
}
