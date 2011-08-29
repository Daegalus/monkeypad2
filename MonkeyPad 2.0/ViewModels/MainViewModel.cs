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

        // Global Variables
        public string AuthToken = "";
        public string Email = "yulian@kuncheff.com";
        public volatile bool GlobalDone;
        public decimal LastCall;
        public string Mark = "";
        public Index NoteIndex;
        public SortableObservableCollection<Note> Notes = new SortableObservableCollection<Note>();
        public string Password = "#3817ilj3";
        public SortableObservableCollection<Note> Pinned = new SortableObservableCollection<Note>();
        public SortableObservableCollection<Note> Trashed = new SortableObservableCollection<Note>();

        public bool IsDataLoaded { get; private set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void LoadData(string mode)
        {
            if (AuthToken == "")
                GetLogin();

            else
                switch (mode)
                {
                    case "clean":
                        GetIndex();
                        break;
                    case "refresh":
                        GetIndex(LastCall);
                        break;
                    case "more":
                        GetIndex(0, NoteIndex.Mark);
                        break;
                }

            //GetData();
        }

        public void GetLogin()
        {
            HttpWebRequest request = RequestFactory.CreateLoginRequest(Email, Password);
            request.BeginGetResponse(result =>
                                         {
                                             var response = (HttpWebResponse)
                                                            ((HttpWebRequest) result.AsyncState).EndGetResponse(result);

                                             var streamReader = new StreamReader(response.GetResponseStream());
                                             string content = streamReader.ReadToEnd();

                                             App.ViewModel.AuthToken = content;
                                             App.ViewModel.Password = "";
                                             GetIndex();
                                         }, request);
        }

        public void GetIndex(decimal since = 0, string mark = null)
        {
            HttpWebRequest request = RequestFactory.CreateListRequest(50, 0, null, Email, AuthToken);
            request.BeginGetResponse(result =>
                                         {
                                             var response = (HttpWebResponse)
                                                            ((HttpWebRequest) result.AsyncState).EndGetResponse(result);
                                             App.ViewModel.GlobalDone = true;
                                             var streamReader = new StreamReader(response.GetResponseStream());
                                             string content = streamReader.ReadToEnd();

                                             var workIndex = JsonProcessor.FromJson<Index>(content);
                                             NoteProcessor.ProcessIndex(workIndex, false);
                                             GetData();
                                         }, null);
        }

        public void GetData()
        {
            foreach (Note note in NoteIndex.Data)
            {
                HttpWebRequest request = RequestFactory.CreateNoteRequest("GET", note, Email, AuthToken);
                request.BeginGetResponse(result =>
                                             {
                                                 var response = (HttpWebResponse)
                                                                ((HttpWebRequest) result.AsyncState).EndGetResponse(
                                                                    result);

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