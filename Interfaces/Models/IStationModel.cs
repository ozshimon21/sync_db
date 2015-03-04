using System;
using System.ComponentModel;

namespace Interfaces.Models
{
    public interface IStationModel : INotifyPropertyChanged
    {
        int Id { get; set; }
        String Name { get; set; }
        int Number { get; set; }
    }
}
