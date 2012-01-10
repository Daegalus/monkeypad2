using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using MarkdownDeep;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MonkeyPad2.Notes;
using MonkeyPad2.Processors;
using MonkeyPad2.Requests;
using MonkeyPad2.Tags;

namespace MonkeyPad2.UI
{
    public partial class NoteView : PhoneApplicationPage
    {
        private readonly IList _appBarButtonsEdit;
        private readonly IList _appBarButtonsView;
        private readonly string _postHtml = "</body></html>";

        private readonly string _preHtml =
            "<html><head><style>" + MarkdownCSS.CSS + "</style></head><body>";

        public Dispatcher RootDispatcher = ((App) Application.Current).RootFrame.Dispatcher;
        private Note _currentNote;
        private WebBrowser _wb = new WebBrowser();

        public NoteView()
        {
            InitializeComponent();

            _appBarButtonsView = new List<ApplicationBarIconButton>();
            _appBarButtonsEdit = new List<ApplicationBarIconButton>();

            foreach (object button in ApplicationBar.Buttons)
            {
                _appBarButtonsView.Add(button);
            }

            var saveButton = new ApplicationBarIconButton(new Uri("/icons/appbar.save.rest.png", UriKind.Relative));
            saveButton.IsEnabled = true;
            saveButton.Text = "Save";
            saveButton.Click += AppBarSaveClick;

            var cancelButton = new ApplicationBarIconButton(new Uri("/icons/appbar.cancel.rest.png", UriKind.Relative));
            cancelButton.IsEnabled = true;
            cancelButton.Text = "Cancel";
            cancelButton.Click += AppBarCancelClick;

            _appBarButtonsEdit.Add(saveButton);
            _appBarButtonsEdit.Add(cancelButton);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Call the base implementation
            base.OnNavigatedTo(e);

            //TODO Add proper stylesheet to make it look nice.

            String noteKey = "";
            UpdateLayout();
            bool found = false;
            if (NavigationContext.QueryString.TryGetValue("key", out noteKey))
            {
                foreach (Note note in App.ViewModel.NoteIndex.Data)
                {
                    if (noteKey == note.Key)
                    {
                        _currentNote = note;
                        break;
                    }
                }

                if (_currentNote.SystemTags.Contains("markdown"))
                {
                    NoteBox.Visibility = Visibility.Collapsed;
                    NoteBox.Text = _currentNote.Content;
                    EditBox.Text = _currentNote.Content;

                    var markdown = new Markdown {ExtraMode = true, SafeMode = false};
                    string converted = markdown.Transform(_currentNote.Content);

                    NoteBrowser.NavigateToString(_preHtml + converted + _postHtml);
                    NoteBrowser.Visibility = Visibility.Visible;
                }
                else
                {
                    NoteBox.Text = _currentNote.Content;
                    EditBox.Text = _currentNote.Content;
                }

                Date.Text = NoteUtils.MonkeyPadDateFormatShort(NoteUtils.FromUnixEpochTime(_currentNote.ModifyDate));
                TagList.Text = TagUtils.tagsToString(_currentNote.Tags);
            }
        }

        public void EditNote()
        {
            if (NoteBox.Visibility == Visibility.Visible)
            {
                NoteBox.Visibility = Visibility.Collapsed;
            }
            if (NoteBrowser.Visibility == Visibility.Visible)
            {
                NoteBrowser.Visibility = Visibility.Collapsed;
            }
            EditBox.Visibility = Visibility.Visible;
        }

        private void AppBarTrashClick(object sender, EventArgs e)
        {
            var innerNote = new Note();
            innerNote.Key = _currentNote.Key;
            innerNote.Version = _currentNote.Version;

            if (_currentNote.SystemTags.Contains("pinned"))
            {
                var list = new List<string>(_currentNote.SystemTags);
                list.Remove("pinned");
                innerNote.SystemTags = list.ToArray();
            }

            innerNote.Deleted = true;

            HttpWebRequest request = RequestFactory.CreateNoteRequest("POST", innerNote, App.ViewModel.Email,
                                                                      App.ViewModel.AuthToken);
            request.BeginGetResponse(result =>
                                         {
                                             WebResponse response = request.EndGetResponse(result);

                                             var streamReader = new StreamReader(response.GetResponseStream());
                                             string content = streamReader.ReadToEnd();

                                             var workNote = JsonProcessor.FromJson<Note>(content);
                                             Note returnedNote = NoteProcessor.ProcessNote(workNote);
                                             _currentNote.Deleted = returnedNote.Deleted;
                                             _currentNote.Version = returnedNote.Version;
                                             _currentNote.MinVersion = returnedNote.MinVersion;
                                             _currentNote.ModifyDate = returnedNote.ModifyDate;

                                             Date.Text =
                                                 NoteUtils.MonkeyPadDateFormatShort(
                                                     NoteUtils.FromUnixEpochTime(_currentNote.ModifyDate));
                                         }, request);
        }

        private void AppBarPinClick(object sender, EventArgs e)
        {
            var innerNote = new EditNote();
            innerNote.key = _currentNote.Key;
            innerNote.version = _currentNote.Version;
            //innerNote.Content = _currentNote.Content;
            innerNote.syncnum = _currentNote.SyncNum;
            innerNote.systemtags = _currentNote.SystemTags;
            innerNote.tags = _currentNote.Tags;


            if (!_currentNote.SystemTags.Contains("pinned"))
            {
                var list = new List<string>(_currentNote.SystemTags);
                list.Add("pinned");
                innerNote.systemtags = list.ToArray();
            }

            if (_currentNote.SystemTags.Contains("pinned"))
            {
                var list = new List<string>(_currentNote.SystemTags);
                list.Remove("pinned");
                innerNote.systemtags = list.ToArray();
            }

            innerNote.deleted = false;

            HttpWebRequest request = RequestFactory.CreateNoteRequest("POST", innerNote, App.ViewModel.Email,
                                                                      App.ViewModel.AuthToken);
            request.BeginGetResponse(result =>
                                         {
                                             WebResponse response = request.EndGetResponse(result);

                                             var streamReader = new StreamReader(response.GetResponseStream());
                                             string content = streamReader.ReadToEnd();

                                             var workNote = JsonProcessor.FromJson<Note>(content);
                                             //var returnedNote = NoteProcessor.ProcessNote(workNote);
                                             _currentNote.SystemTags = workNote.SystemTags;
                                             _currentNote.Version = workNote.Version;
                                             _currentNote.MinVersion = workNote.MinVersion;
                                             _currentNote.ModifyDate = workNote.ModifyDate;
                                             if (!String.IsNullOrEmpty(workNote.Content))
                                             {
                                                 _currentNote.Content = workNote.Content;
                                                 _currentNote = NoteProcessor.ProcessNote(_currentNote);
                                             }

                                             RootDispatcher.BeginInvoke(
                                                 new Action<string>((modifyDate) => { Date.Text = modifyDate; }),
                                                 NoteUtils.MonkeyPadDateFormatShort(
                                                     NoteUtils.FromUnixEpochTime(_currentNote.ModifyDate)));

                                             App.ViewModel.MovePinned(_currentNote);
                                         }, request);
        }

        private void AppBarEditClick(object sender, EventArgs e)
        {
            NoteBox.Visibility = Visibility.Collapsed;
            NoteBrowser.Visibility = Visibility.Collapsed;
            EditBox.Visibility = Visibility.Visible;

            ApplicationBar.Buttons.Clear();
            foreach (object button in _appBarButtonsEdit)
            {
                ApplicationBar.Buttons.Add(button);
            }
        }

        private void AppBarEmailClick(object sender, EventArgs e)
        {
            // TODO: Add event handler implementation here.
        }

        private void AppBarSaveClick(object sender, EventArgs e)
        {
        }

        private void AppBarCancelClick(object sender, EventArgs e)
        {
            EditBox.Visibility = Visibility.Collapsed;
            EditBox.Text = _currentNote.Content;
            if (_currentNote.SystemTags.Contains("markdown"))
                NoteBrowser.Visibility = Visibility.Visible;
            else
                NoteBox.Visibility = Visibility.Visible;


            ApplicationBar.Buttons.Clear();
            foreach (object button in _appBarButtonsView)
            {
                ApplicationBar.Buttons.Add(button);
            }
        }
    }
}