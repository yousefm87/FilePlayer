using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
