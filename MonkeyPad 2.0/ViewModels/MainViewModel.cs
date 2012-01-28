using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using MonkeyPad2.Comparers;
using MonkeyPad2.Notes;
using MonkeyPad2.Processors;
using MonkeyPad2.Requests;
using MonkeyPad2.Tags;
using Index = MonkeyPad2.Notes.Index;

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
        public Dispatcher RootDispatcher = ((App) Application.Current).RootFrame.Dispatcher;
        public SortableObservableCollection<Tag> Tags = new SortableObservableCollection<Tag>();
        public SortableObservableCollection<Note> Trashed = new SortableObservableCollection<Note>();
        private int _globalcounter;

        public int GlobalCounter
        {
            get { return _globalcounter; }
            set
            {
                _globalcounter = value;
                NotifyPropertyChanged("GlobalCounter");
            }
        }

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
            IsDataLoaded = true;
        }

        public void GetLogin()
        {
            HttpWebRequest request = RequestFactory.CreateLoginRequest(Email, Password);
            request.BeginGetResponse(result =>
                                         {
                                             WebResponse response = request.EndGetResponse(result);

                                             var streamReader = new StreamReader(response.GetResponseStream());
                                             string content = streamReader.ReadToEnd();

                                             App.ViewModel.AuthToken = content;
                                             App.ViewModel.Password = "";
                                             GetIndex();
                                         }, request);
        }

        public void GetIndex(decimal since = 0, string mark = null)
        {
            HttpWebRequest request = RequestFactory.CreateListRequest(50, since, mark, Email, AuthToken);
            request.BeginGetResponse(result =>
                                         {
                                             WebResponse response = request.EndGetResponse(result);
                                             App.ViewModel.GlobalDone = true;
                                             var streamReader = new StreamReader(response.GetResponseStream());
                                             string content = streamReader.ReadToEnd();

                                             var workIndex = JsonProcessor.FromJson<Index>(content);
                                             NoteProcessor.ProcessIndex(workIndex, false);
                                             LastCall = NoteUtils.CurrentTimeEpoch();
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
                                                 WebResponse response = request.EndGetResponse(result);

                                                 var streamReader = new StreamReader(response.GetResponseStream());
                                                 string content = streamReader.ReadToEnd();

                                                 var workNote = JsonProcessor.FromJson<Note>(content);
                                                 Note returnedNote = NoteProcessor.ProcessNote(workNote);
                                                 UpdateLists(returnedNote);

                                                 if (GlobalCounter == NoteIndex.Count)
                                                 {
                                                     //UpdateLists();
                                                 }
                                             }, null);
            }
        }

        public void UpdateLists(Note note)
        {
            //foreach (var note in NoteIndex.Data)
            //{
            var temp = new List<string>(note.SystemTags);
            if (note.SystemTags.Length > 0 && temp.Contains("pinned"))
            {
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                                                                {
                                                                    int index =
                                                                        (new List<Note>(Pinned)).BinarySearch(note2,
                                                                                                              new ModifyDateComparer
                                                                                                                  ());
                                                                    if (index < 0)
                                                                    {
                                                                        Pinned.Insert(~index, note2);
                                                                    }
                                                                    else
                                                                    {
                                                                        Pinned.Insert(index, note2);
                                                                    }
                                                                    GlobalCounter++;
                                                                    NotifyPropertyChanged("Pinned");
                                                                }), note);
            }
            else if (note.Deleted)
            {
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                                                                {
                                                                    int index =
                                                                        (new List<Note>(Trashed)).BinarySearch(note2,
                                                                                                               new ModifyDateComparer
                                                                                                                   ());
                                                                    if (index < 0)
                                                                    {
                                                                        Trashed.Insert(~index, note2);
                                                                    }
                                                                    else
                                                                    {
                                                                        Trashed.Insert(index, note2);
                                                                    }
                                                                    GlobalCounter++;
                                                                    NotifyPropertyChanged("Trashed");
                                                                }), note);
            }
            else
            {
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                                                                {
                                                                    int index =
                                                                        (new List<Note>(Notes)).BinarySearch(note2,
                                                                                                             new ModifyDateComparer
                                                                                                                 ());
                                                                    if (index < 0)
                                                                    {
                                                                        Notes.Insert(~index, note2);
                                                                    }
                                                                    else
                                                                    {
                                                                        Notes.Insert(index, note2);
                                                                    }
                                                                    GlobalCounter++;
                                                                    NotifyPropertyChanged("Notes");
                                                                }), note);
            }
            //}
        }

        public void SortList(SortableObservableCollection<Note> noteList)
        {
            RootDispatcher.BeginInvoke(
                new Action<SortableObservableCollection<Note>>((noteList2) => noteList2.Sort(new ModifyDateComparer())),
                noteList);
        }

        public void MovePinned(Note pinnedNote)
        {
            if (!pinnedNote.SystemTags.Contains("pinned"))
            {
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                                                                {
                                                                    Pinned.Remove(note2);
                                                                    NotifyPropertyChanged("Pinned");
                                                                }), pinnedNote);
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                                                                {
                                                                    int index =
                                                                        (new List<Note>(Notes)).BinarySearch(note2,
                                                                                                             new ModifyDateComparer
                                                                                                                 ());
                                                                    if (index < 0)
                                                                    {
                                                                        Notes.Insert(~index, note2);
                                                                    }
                                                                    else
                                                                    {
                                                                        Notes.Insert(index, note2);
                                                                    }
                                                                    NotifyPropertyChanged("Notes");
                                                                }), pinnedNote);
            }

            if (pinnedNote.SystemTags.Contains("pinned"))
            {
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                                                                {
                                                                    Notes.Remove(note2);
                                                                    NotifyPropertyChanged("Notes");
                                                                }), pinnedNote);
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                                                                {
                                                                    int index =
                                                                        (new List<Note>(Pinned)).BinarySearch(note2,
                                                                                                              new ModifyDateComparer
                                                                                                                  ());
                                                                    if (index < 0)
                                                                    {
                                                                        Pinned.Insert(~index, note2);
                                                                    }
                                                                    else
                                                                    {
                                                                        Pinned.Insert(index, note2);
                                                                    }
                                                                    NotifyPropertyChanged("Pinned");
                                                                }), pinnedNote);
            }
        }

        public void MoveTrashed(Note trashedNote)
        {
            if (!trashedNote.Deleted)
            {
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                {
                    Trashed.Remove(note2);
                    NotifyPropertyChanged("Trashed");
                }), trashedNote);
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                {
                    int index =
                        (new List<Note>(Notes)).BinarySearch(note2,
                                                             new ModifyDateComparer
                                                                 ());
                    if (index < 0)
                    {
                        Notes.Insert(~index, note2);
                    }
                    else
                    {
                        Notes.Insert(index, note2);
                    }
                    NotifyPropertyChanged("Notes");
                }), trashedNote);
            }

            if (trashedNote.Deleted)
            {
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                {
                    Notes.Remove(note2);
                    NotifyPropertyChanged("Notes");
                }), trashedNote);
                RootDispatcher.BeginInvoke(new Action<Note>((note2) =>
                {
                    int index =
                        (new List<Note>(Trashed)).BinarySearch(note2,
                                                              new ModifyDateComparer
                                                                  ());
                    if (index < 0)
                    {
                        Trashed.Insert(~index, note2);
                    }
                    else
                    {
                        Trashed.Insert(index, note2);
                    }
                    NotifyPropertyChanged("Trashed");
                }), trashedNote);
            }
        }

        public void GetMark()
        {
            GetIndex(0, NoteIndex.Mark);
        }

        public void Refresh()
        {
            GetIndex(LastCall, null);
        }

        public void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}