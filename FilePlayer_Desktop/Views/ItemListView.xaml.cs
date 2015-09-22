using System;
using System.Windows;
using System.Windows.Controls;
using FilePlayer.ViewModels;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ItemListWindow.xaml
    /// </summary>
    public partial class ItemListView : UserControl
    {
        
        
        public ItemListViewModel ItemListViewModel { get; set; }

        public ItemListView()
        {
            InitializeComponent();
            ItemListViewModel = new ItemListViewModel();
            this.DataContext = ItemListViewModel;

            ItemListViewModel.SendAction += PerformViewAction;
            
        }

        void PerformViewAction(object sender, ItemListViewEventArgs e)
        {
            switch(e.action)
            {
                case "MOVEUP":
                    MoveUp();
                    break;
                case "MOVEDOWN":
                    MoveDown();
                    break;
            }
        }

        private void itemlist_Loaded(object sender, RoutedEventArgs e)
        {
            SelectFirstItem();
        }

        void SelectFirstItem()
        {
            int newSelectedIndex = 0;

            if ((itemlist.Items.Count > 0) && (itemlist.Columns.Count > 0))
            {
                //Select the first column of the first item.
                itemlist.Dispatcher.Invoke((Action)delegate
                {
                    itemlist.CurrentCell = new DataGridCellInfo(itemlist.Items[newSelectedIndex], itemlist.Columns[0]);
                    itemlist.SelectedCells.Add(itemlist.CurrentCell);
                });

                ItemListViewModel.SelectedItemIndex = newSelectedIndex;
            }
        }


        public void MoveDown()
        {
            
            if ((itemlist.Items.Count > 0) && (itemlist.Columns.Count > 0))
            {
                int selectedIndex = ItemListViewModel.SelectedItemIndex;
                int newSelectedIndex = selectedIndex + 1;
                int maxIndex = itemlist.Items.Count - 1;

                if (newSelectedIndex <= maxIndex)
                {
                    itemlist.Dispatcher.Invoke((Action)delegate
                    {
                        itemlist.SelectedCells.Remove(itemlist.CurrentCell);
                 
                        itemlist.CurrentCell = new DataGridCellInfo(itemlist.Items[newSelectedIndex], itemlist.Columns[0]);
                        itemlist.SelectedCells.Add(itemlist.CurrentCell);

                        itemlist.ScrollIntoView(itemlist.CurrentCell.Item,itemlist.CurrentCell.Column);
                    });

                    ItemListViewModel.SelectedItemIndex = newSelectedIndex;
                }
            }
        }

        public void MoveUp()
        {
            if ((itemlist.Items.Count > 0) && (itemlist.Columns.Count > 0))
            {
                int selectedIndex = ItemListViewModel.SelectedItemIndex; //int selectedIndex = itemlist.SelectedIndex;
                int newSelectedIndex = selectedIndex - 1;
                int minIndex = 0;

                if (newSelectedIndex >= minIndex)
                {
                    itemlist.Dispatcher.Invoke((Action) delegate 
                    {
                        itemlist.SelectedCells.Remove(itemlist.CurrentCell);

                        itemlist.CurrentCell = new DataGridCellInfo(itemlist.Items[newSelectedIndex], itemlist.Columns[0]);
                        itemlist.SelectedCells.Add(itemlist.CurrentCell);

                        itemlist.ScrollIntoView(itemlist.CurrentCell.Item, itemlist.CurrentCell.Column);
                    });

                    ItemListViewModel.SelectedItemIndex = newSelectedIndex;
                }

            }
        }



    }
}