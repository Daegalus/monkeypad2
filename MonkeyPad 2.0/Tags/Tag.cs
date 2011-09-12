using System;
using System.ComponentModel;

namespace MonkeyPad2.Tags
{
    public class Tag : INotifyPropertyChanged
    {
        private string _name { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public int Index { get; set; }
        public int Version { get; set; }
        public string[] SystemTags { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void NotifyPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}