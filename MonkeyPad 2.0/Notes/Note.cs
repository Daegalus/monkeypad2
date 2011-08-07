using System;
using System.ComponentModel;

namespace MonkeyPad2.Notes
{
    public class Note : INotifyPropertyChanged
    {
        public string Key { get; set; }
        public bool Deleted { get; set; }
        public decimal ModifyDate { get; set; }
        public decimal CreateDate { get; set; }
        public int SyncNum { get; set; }
        public int Version { get; set; }
        public int MinVersion { get; set; }
        public string ShareKey { get; set; }
        public string PublishKey { get; set; }
        public string[] SystemTags { get; set; }
        public string[] Tags { get; set; }
        public string Content { get; set; }
        private string _displaytitle;
        public string DisplayTitle
        {
            get
            {
                return _displaytitle;
            }
            set
            {
                _displaytitle = value;
                NotifyPropertyChanged("DisplayTitle");
            }
        }
        private string _displaycontent;
        public string DisplayContent
        {
            get
            {
                return _displaycontent;
            }
            set
            {
                _displaycontent = value;
                NotifyPropertyChanged("DisplayContent");
            }
        }
        private string _displaydate;
        public string DisplayDate
        {
            get
            {
                return _displaydate;
            }
            set
            {
                _displaydate = value;
                NotifyPropertyChanged("DisplayDate");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
