using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using FilePlayer.ViewModels;
using FilePlayer.Model;

using Microsoft.Practices.Prism.PubSubEvents;
using System.IO;
using System.Diagnostics;

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

            viewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    
                    PerformViewAction(this, viewEventArgs);
                }
            );
                        //ItemListViewModel.SendAction += PerformViewAction;

        }

        void PerformViewAction(object sender, ItemListViewEventArgs e)
        {
            switch(e.action)
            {
                case "":
                    
                case "ITEMLIST_MOVE_UP":
                    MoveUp();
                    break;
                case "ITEMLIST_MOVE_DOWN":
                    MoveDown();
                    break;
                case "CONFIRM_OPEN":
                    AddListShade();
                    break;
                case "PAUSE_OPEN":
                    AddListShade();
                    break;
                case "CONFIRM_CLOSE":
                    RemoveListShade();
                    if(e.addlInfo[0] == "YES")
                    {
                        ItemListViewModel.OpenSelectedItemInApp();
                        ItemListViewModel.SetControllerState("ITEM_PLAY");
                    }
                    else
                    {
                        ItemListViewModel.SetControllerState("ITEMLIST_BROWSE");
                    }
                    break;
                case "PAUSE_CLOSE":
                    RemoveListShade();
                    break;

            }
        }

        private void AddListShade()
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


        private void RemoveListShade()
        {
            itemlist.Dispatcher.Invoke((Action)delegate
            {
                if (this.Effect != null)
                    this.Effect = null;
            });
        }

        private void Itemlist_Loaded(object sender, RoutedEventArgs e)
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