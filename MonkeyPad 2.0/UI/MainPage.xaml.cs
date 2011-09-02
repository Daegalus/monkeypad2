using System.Windows;
using Microsoft.Phone.Controls;

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
    }
}