using System.ComponentModel;
using System.Runtime.CompilerServices;
using DAL.Annotations;
using Interfaces.Models;

namespace Client.Models
{
    public class BaseModel : IBaseModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
