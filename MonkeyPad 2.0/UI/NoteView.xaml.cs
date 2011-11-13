using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using MarkdownDeep;
using Microsoft.Phone.Controls;
using MonkeyPad2.Notes;
using MonkeyPad2.Tags;

namespace MonkeyPad2.UI
{
    public partial class NoteView : PhoneApplicationPage
    {
        public Dispatcher RootDispatcher = ((App) Application.Current).RootFrame.Dispatcher;
        private Note _currentNote;
        private WebBrowser _wb = new WebBrowser();

        private readonly string _preHtml =
            "<html><head><style>"+MarkdownCSS.CSS+"</style></head><body>";
        private readonly string _postHtml = "</body></html>";

        public NoteView()
        {
            InitializeComponent();
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
            var found = false;
            if (NavigationContext.QueryString.TryGetValue("key", out noteKey))
            {
                foreach (var note in App.ViewModel.NoteIndex.Data)
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

                    var markdown = new Markdown {ExtraMode = true, SafeMode = false};
                    var converted = markdown.Transform(_currentNote.Content);

                    NoteBrowser.NavigateToString(_preHtml + converted + _postHtml);
                    NoteBrowser.Visibility = Visibility.Visible;
                }
                else
                {
                    NoteBox.Text = _currentNote.Content;
                }

                Date.Text = NoteUtils.MonkeyPadDateFormatShort(NoteUtils.FromUnixEpochTime(_currentNote.ModifyDate));
                TagList.Text = TagUtils.tagsToString(_currentNote.Tags);
            }
        }

        public void EditNote()
        {
            if(NoteBox.Visibility == Visibility.Visible)
            {
                NoteBox.Visibility = Visibility.Collapsed;
            }
            if (NoteBrowser.Visibility == Visibility.Visible)
            {
                NoteBrowser.Visibility = Visibility.Collapsed;
            }
            EditBox.Visibility = Visibility.Visible;
        }
    }
}