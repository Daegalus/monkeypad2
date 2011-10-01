using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;

namespace MonkeyPad2.UI
{
    public partial class NoteView : PhoneApplicationPage
    {
        private Notes.Note currentNote;
        WebBrowser wb = new WebBrowser();
        public Dispatcher RootDispatcher = ((App)Application.Current).RootFrame.Dispatcher;

        public NoteView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Call the base implementation
            base.OnNavigatedTo(e);

            //TODO Add proper stylesheet to make it look nice.

            String noteKey = "";
            UpdateLayout();
            bool found = false;
            if (NavigationContext.QueryString.TryGetValue("key", out noteKey))
            {
                foreach (Notes.Note note in App.ViewModel.NoteIndex.Data)
                {
                    if (noteKey == note.Key)
                    {
                        currentNote = note;
                        break;
                    }
                }

                if (currentNote.SystemTags.Contains<String>("markdown"))
                {
                    NoteBox.Visibility = System.Windows.Visibility.Collapsed;
                    
                    MarkdownDeep.Markdown markdown = new MarkdownDeep.Markdown();
                    markdown.ExtraMode = true;
                    markdown.SafeMode = false;
                    string converted = markdown.Transform(currentNote.Content);

                    NoteBrowser.NavigateToString(converted);
                    NoteBrowser.Visibility = System.Windows.Visibility.Visible;
                    
                }
                else
                {
                    NoteBox.Text = currentNote.Content;
                }

                Date.Text = Notes.NoteUtils.MonkeyPadDateFormatShort(Notes.NoteUtils.FromUnixEpochTime(currentNote.ModifyDate));
                TagList.Text = MonkeyPad2.Tags.TagUtils.tagsToString(currentNote.Tags);
            }
        }
    }
}