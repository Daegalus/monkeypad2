using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using MonkeyPad2.Notes;
using MonkeyPad2.Processors;
using MonkeyPad2.Requests;

namespace MonkeyPad2
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Lists
        public Index NoteIndex;
        public SortableObservableCollection<Note> Notes = new SortableObservableCollection<Note>();
        public SortableObservableCollection<Note> Pinned = new SortableObservableCollection<Note>();
        public SortableObservableCollection<Note> Trashed = new SortableObservableCollection<Note>();

        // Global Variables
        public string authToken = "";
        public string email = "yulian@kuncheff.com";
        public bool globalDone;
        public decimal lastCall;
        public string mark = "";
        public string password = "#3817ilj3";

        public bool IsDataLoaded { get; private set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void LoadData(string mode)
        {
            if (authToken == "")
                GetLogin();

            if (mode == "clean")
                GetIndex();
            else if (mode == "refresh")
                GetIndex(lastCall);
            else if (mode == "more")
                GetIndex(0, NoteIndex.Mark);

            GetData();
            int i = 1 + 1;
        }

        public void GetLogin()
        {
            HttpWebRequest request = RequestFactory.CreateLoginRequest(email, password);
            HttpWebResponse response = null;
            request.BeginGetResponse(result =>
                                         {
                                             response =
                                                 (HttpWebResponse)
                                                 ((HttpWebRequest) result.AsyncState).EndGetResponse(result);
                                             App.ViewModel.globalDone = true;
                                         }, request);
            while (!globalDone) ;
            globalDone = false;

            var streamReader = new StreamReader(response.GetResponseStream());
            string content = streamReader.ReadToEnd();

            authToken = content;
            password = "";
        }

        public void GetIndex(decimal since = 0, string mark = null)
        {
            HttpWebRequest request = RequestFactory.CreateListRequest(50, 0, null, email, authToken);
            HttpWebResponse response = null;
            request.BeginGetResponse(result =>
                                         {
                                             response =
                                                 (HttpWebResponse)
                                                 ((HttpWebRequest) result.AsyncState).EndGetResponse(result);
                                             App.ViewModel.globalDone = true;
                                         }, null);
            while (!globalDone) ;
            globalDone = false;

            var streamReader = new StreamReader(response.GetResponseStream());
            string content = streamReader.ReadToEnd();

            var workIndex = JsonProcessor.FromJson<Index>(content);
            NoteProcessor.ProcessIndex(workIndex, false);
        }

        public void GetData()
        {
            foreach (Note note in NoteIndex.Data)
            {
                HttpWebRequest request = RequestFactory.CreateNoteRequest("GET", note, email, authToken);
                HttpWebResponse response = null;
                bool done = false;
                request.BeginGetResponse(result =>
                                             {
                                                 response =
                                                     (HttpWebResponse)
                                                     ((HttpWebRequest) result.AsyncState).EndGetResponse(result);

                                                 var streamReader = new StreamReader(response.GetResponseStream());
                                                 string content = streamReader.ReadToEnd();

                                                 var workNote = JsonProcessor.FromJson<Note>(content);
                                                 NoteProcessor.ProcessNote(workNote);
                                             }, null);
            }
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