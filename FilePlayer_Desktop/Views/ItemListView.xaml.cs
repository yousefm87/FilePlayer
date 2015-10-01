using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using FilePlayer.ViewModels;
using FilePlayer.Model;

using Microsoft.Practices.Prism.PubSubEvents;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ItemListWindow.xaml
    /// </summary>
    public partial class ItemListView : UserControl
    {
        
        
        public ItemListViewModel ItemListViewModel { get; set; }

        private IEventAggregator iEventAggregator;
        SubscriptionToken viewActionToken;

        public ItemListView()
        {
            ItemListViewModel = new ItemListViewModel(Event.EventInstance.EventAggregator);
            this.DataContext = ItemListViewModel;
            this.iEventAggregator = Event.EventInstance.EventAggregator;

            InitializeComponent();

            viewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
                        //ItemListViewModel.SendAction += PerformViewAction;

        }

        void PerformViewAction(object sender, ViewEventArgs e)
        {
            switch(e.action)
            {
                case "":
                    
                case "MOVE_UP":
                    MoveUp();
                    break;
                case "MOVE_DOWN":
                    MoveDown();
                    break;
                case "CONFIRM_OPEN":
                    OpenConfirmationDialog();
                    break;
            }
        }

        private void OpenConfirmationDialog()
        {
            itemlist.Dispatcher.Invoke((Action)delegate
            {
                if (this.Effect == null)
                {
                    this.Effect = new BlurEffect();
                }
                else
                {
                    this.Effect = null;
                }


            });
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