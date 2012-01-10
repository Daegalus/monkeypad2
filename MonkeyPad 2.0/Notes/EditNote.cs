using System;
using System.ComponentModel;

namespace MonkeyPad2.Notes
{
    public class EditNote : INotifyPropertyChanged
    {
        public string key { get; set; }
        public bool deleted { get; set; }
        public int syncnum { get; set; }
        public int version { get; set; }
        public string[] systemtags { get; set; }
        public string[] tags { get; set; }
        public string content { get; set; }

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