using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using MonkeyPad2.Notes;

namespace MonkeyPad2.UI
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            Loaded += MainPage_Loaded;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                RebindLIsts();
                App.ViewModel.LoadData("clean");
            }
        }

        private void RebindLIsts()
        {
            notesListBox.ItemsSource = null;
            pinnedListBox.ItemsSource = null;
            trashedListBox.ItemsSource = null;
            tagsListtBox.ItemsSource = null;
            notesListBox.ItemsSource = App.ViewModel.Notes;
            pinnedListBox.ItemsSource = App.ViewModel.Pinned;
            trashedListBox.ItemsSource = App.ViewModel.Trashed;
            tagsListtBox.ItemsSource = App.ViewModel.Tags;
        }

        private void notesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox.SelectedIndex > -1)
            {
                Note item = App.ViewModel.Notes[listBox.SelectedIndex];
                NavigationService.Navigate(new Uri("/UI/NoteView.xaml?key=" + item.Key, UriKind.Relative));
                listBox.SelectedIndex = -1;
            }
        }

        private void pinnedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox.SelectedIndex > -1)
            {
                Note item = App.ViewModel.Pinned[listBox.SelectedIndex];
                NavigationService.Navigate(new Uri("/UI/NoteView.xaml?key=" + item.Key, UriKind.Relative));
                listBox.SelectedIndex = -1;
            }
        }

        private void trashedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox.SelectedIndex > -1)
            {
                Note item = App.ViewModel.Trashed[listBox.SelectedIndex];
                NavigationService.Navigate(new Uri("/UI/NoteView.xaml?key=" + item.Key, UriKind.Relative));
                listBox.SelectedIndex = -1;
            }
        }

        private void AppBarNewClick(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/UI/NewView.xaml", UriKind.Relative));
        }

        private void AppBarRefreshClick(object sender, System.EventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void AppBarMoreClick(object sender, System.EventArgs e)
        {
        	App.ViewModel.GetMark();
        }

        private void AppBarrLogoutClick(object sender, System.EventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void AppBarSettingsClick(object sender, System.EventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void AppBarAboutClick(object sender, System.EventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }
    }
}