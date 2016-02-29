using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using FilePlayer.ViewModels;
using FilePlayer.Model;
using System.Collections;

using Microsoft.Practices.Prism.PubSubEvents;
using System.Linq;

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

        Stack shadeStack = new Stack();

        public ItemListView()
        {
            InitializeComponent();
            this.iEventAggregator = Event.EventInstance.EventAggregator;
            viewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );

            ItemListViewModel = new ItemListViewModel(Event.EventInstance.EventAggregator);
            this.DataContext = ItemListViewModel;
            




                      

        }

        void PerformViewAction(object sender, ViewEventArgs e)
        {
            int numMoves;
            switch(e.action)
            {
                case "ITEMLIST_EMPTY":
                    if (errorMessage.Visibility == Visibility.Hidden)
                    {
                        errorMessage.Visibility = Visibility.Visible;
                    }
                    break;
                case "ITEMLIST_UPDATED":
                    if (errorMessage.Visibility == Visibility.Visible)
                    {
                        errorMessage.Visibility = Visibility.Hidden;
                    }
                    break;
                case "ITEMLIST_MOVE_UP":
                    numMoves = Int32.Parse(e.addlInfo[0]);
                    MoveUp(numMoves);
                    break;
                case "ITEMLIST_MOVE_DOWN":
                    numMoves = Int32.Parse(e.addlInfo[0]);
                    MoveDown(numMoves);
                    break;
                case "ITEMLIST_MOVE_LEFT":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        ItemListViewModel.SetPreviousLists(filterControl.fileFilterText.Text, filterControl.filterTypeText.Text);
                        SelectFirstItem();
                    });
                    break;
                case "ITEMLIST_MOVE_RIGHT":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        
                        ItemListViewModel.SetNextLists(filterControl.fileFilterText.Text, filterControl.filterTypeText.Text);
                        SelectFirstItem();
                    });
                    break;
                case "BUTTONDIALOG_OPEN":
                    
                    AddListShade();
                    break;
                case "BUTTONDIALOG_CLOSE":
                    RemoveListShade();
                    break;
                case "CONTROLLER_NOTFOUND_OPEN":
                    AddListShade();
                    break;
                case "CONTROLLER_NOTFOUND_CLOSE":
                    RemoveListShade();
                    break;
                case "BUTTONDIALOG_SELECT":
                    RemoveListShade();
                    break;
                case "GIANTBOMB_UPLOAD_START":
                    AddListShade();
                    break;
                case "GIANTBOMB_UPLOAD_COMPLETE":
                    RemoveListShade();
                    break;
                case "OPEN_FILTER":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        if (ItemListViewModel.ItemLists.GetConsoleCount() > 0)
                        {
                            filterControl.Visibility = Visibility.Visible;
                            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new String[] { "FILTER_MAIN" }));
                        }
                    });
                    break;
                case "CLOSE_FILTER":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        filterControl.Visibility = Visibility.Hidden;
                    });
                    break;
                case "FILTER_ACTION":
                    switch (e.addlInfo[0])
                    {
                        case "FILTER_RESET":
                            break;
                    }
                    break;
                case "FILTER_LIST":
                    ItemListViewModel.AllItemNames = ItemListViewModel.ItemLists.GetItemNames(ItemListViewModel.ItemLists.CurrConsole, filterControl.GetFilterFile(), filterControl.GetFilterType());
                    ItemListViewModel.AllItemPaths = ItemListViewModel.ItemLists.GetItemFilePaths(ItemListViewModel.ItemLists.CurrConsole, filterControl.GetFilterFile(), filterControl.GetFilterType());
                    SelectFirstItem();
                    break;
            }
        }

        private void AddListShade()
        {
            itemlist.Dispatcher.Invoke((Action)delegate
            {
                if (this.Effect == null)
                {
                    BlurEffect blur = new BlurEffect();
                    blur.Radius = 20;
                    this.Effect = blur;

                }

                shadeStack.Push(0);
            });
        }


        private void RemoveListShade()
        {
            itemlist.Dispatcher.Invoke((Action)delegate
            {
                shadeStack.Pop();

                if (shadeStack.Count == 0)
                {
                    if (this.Effect != null)
                    {
                        this.Effect = null;
                    }
                }
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
                    if(itemlist.CurrentCell.IsValid)
                    {
                        itemlist.SelectedCells.Remove(itemlist.CurrentCell);
                    }
                    DataGridCellInfo newCell = new DataGridCellInfo(itemlist.Items[newSelectedIndex], itemlist.Columns[0]);

                    itemlist.CurrentCell = newCell;
                    itemlist.SelectedCells.Add(itemlist.CurrentCell);
                    itemlist.ScrollIntoView(itemlist.CurrentCell.Item, itemlist.CurrentCell.Column);
                    
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

        public void MoveUp(int numMove)
        {
            if ((itemlist.Items.Count > 0) && (itemlist.Columns.Count > 0))
            {
                int selectedIndex = ItemListViewModel.SelectedItemIndex; 
                int newSelectedIndex = selectedIndex - numMove;
                int minIndex = 0;

                if (newSelectedIndex < minIndex)
                {
                    newSelectedIndex = minIndex;
                }

                itemlist.Dispatcher.Invoke((Action)delegate
                {
                    itemlist.SelectedCells.Remove(itemlist.CurrentCell);

                    itemlist.CurrentCell = new DataGridCellInfo(itemlist.Items[newSelectedIndex], itemlist.Columns[0]);
                    itemlist.SelectedCells.Add(itemlist.CurrentCell);

                    itemlist.ScrollIntoView(itemlist.CurrentCell.Item, itemlist.CurrentCell.Column);
                });

                ItemListViewModel.SelectedItemIndex = newSelectedIndex;
            }
        }

        public void MoveUp2(int numMove)
        {
            if ((itemlist.Items.Count > 0) && (itemlist.Columns.Count > 0))
            {
                int selectedIndex = ItemListViewModel.SelectedItemIndex;
                int newSelectedIndex = selectedIndex - numMove;
                int minIndex = 0;

                if (newSelectedIndex < minIndex)
                {
                    newSelectedIndex = minIndex;
                }

                ItemListViewModel.SelectedItemIndex = newSelectedIndex;
                

                itemlist.Dispatcher.Invoke((Action)delegate
                {
                    itemlist.ScrollIntoView(itemlist.SelectedCells.ElementAt(0));
                });
            }
        }

        public void MoveDown(int numMove)
        {

            if ((itemlist.Items.Count > 0) && (itemlist.Columns.Count > 0))
            {
                int selectedIndex = ItemListViewModel.SelectedItemIndex;
                int newSelectedIndex = selectedIndex + numMove;
                int maxIndex = itemlist.Items.Count - 1;

                if (newSelectedIndex > maxIndex)
                {
                    newSelectedIndex = maxIndex;
                }

                itemlist.Dispatcher.Invoke((Action)delegate
                {
                    itemlist.SelectedCells.Remove(itemlist.CurrentCell);

                    itemlist.CurrentCell = new DataGridCellInfo(itemlist.Items[newSelectedIndex], itemlist.Columns[0]);
                    itemlist.SelectedCells.Add(itemlist.CurrentCell);

                    itemlist.ScrollIntoView(itemlist.CurrentCell.Item, itemlist.CurrentCell.Column);
                });

                ItemListViewModel.SelectedItemIndex = newSelectedIndex;
            }
        }

        public void MoveDown2(int numMove)
        {

            if ((itemlist.Items.Count > 0) && (itemlist.Columns.Count > 0))
            {
                int selectedIndex = ItemListViewModel.SelectedItemIndex;
                int newSelectedIndex = selectedIndex + numMove;
                int maxIndex = itemlist.Items.Count - 1;

                if (newSelectedIndex > maxIndex)
                {
                    newSelectedIndex = maxIndex;
                }

                ItemListViewModel.SelectedItemIndex = newSelectedIndex;

                itemlist.Dispatcher.Invoke((Action)delegate
                {
                    itemlist.ScrollIntoView(itemlist.CurrentCell.Item, itemlist.CurrentCell.Column);
                });
            }
        }


    }
}