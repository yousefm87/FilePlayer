using System.Windows.Controls;
using FilePlayer.ViewModels;
using FilePlayer.Model;


namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for SearchGameItem.xaml
    /// </summary>
    public partial class SearchGameItem : UserControl
    {

        SearchGameItemViewModel SearchGameItemViewModel { get; set; }
        public SearchGameItem(string _itemName, string _itemImage)
        {
            InitializeComponent();
            SearchGameItemViewModel = new SearchGameItemViewModel(Event.EventInstance.EventAggregator, _itemName, _itemImage);
            this.DataContext = SearchGameItemViewModel;
            
        }
    }
}
