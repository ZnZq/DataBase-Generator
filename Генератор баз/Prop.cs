using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Генератор_баз
{
    public delegate string format(string souce);

    public class Prop : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool _Active;
        public bool Active
        {
            get => _Active;
            set
            {
                _Active = value;
                OnPropertyChanged();
            }
        }

        private string _Descript;
        public string Descript
        {
            get => _Descript;
            set
            {
                _Descript = value;
                OnPropertyChanged();
            }
        }

        public format Format = null;

        public Prop(bool active, string descrip, format format)
        {
            Active = active;
            Descript = descrip;
            Format = format;
        }
    }
}