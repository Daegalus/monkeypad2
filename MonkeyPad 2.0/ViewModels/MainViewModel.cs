using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using MonkeyPad2.Notes;

namespace MonkeyPad2
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Lists
        public Notes.Index NoteIndex;
        public Notes.SortableObservableCollection<Notes.Note> Notes = new Notes.SortableObservableCollection<Note>();
        public Notes.SortableObservableCollection<Notes.Note> Pinned = new Notes.SortableObservableCollection<Note>();
        public Notes.SortableObservableCollection<Notes.Note> Trashed = new Notes.SortableObservableCollection<Note>();

        // Global Variables
        public string email = "";
        public string password = "";
        public string authToken = "";
        public decimal lastCall = 0;
        public string mark = "";

        public bool IsDataLoaded { get; private set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void LoadData(string mode)
        {
            if (mode == "clean")
                GetIndex();
            else if (mode == "refresh")
                GetIndex(lastCall);
            else if (mode == "more")
                GetIndex(0, NoteIndex.Mark);
            GetData();
        }

        public void GetIndex(decimal since = 0, string mark = null)
        {
            HttpWebRequest request = Requests.RequestFactory.CreateLoginRequest(email, password);
            HttpWebResponse response = null;
            bool done = false;
            request.BeginGetResponse(result =>
                                         {
                                             response =
                                                 (HttpWebResponse)
                                                 ((HttpWebRequest) result.AsyncState).EndGetResponse(result);
                                             done = true;
                                         }, null);
            while (!done) ;

            var streamReader = new StreamReader(response.GetResponseStream());
            string content = streamReader.ReadToEnd();

            var workIndex = Processors.JsonProcessor.FromJson<Notes.Index>(content);
            Processors.NoteProcessor.ProcessIndex(workIndex);
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}