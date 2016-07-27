using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using FilePlayer.ViewModels;
using FilePlayer.Model;
using System.Collections;

using Microsoft.Practices.Prism.PubSubEvents;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ItemListWindow.xaml
    /// </summary>
    public partial class ItemListView : UserControl
    {
        public ItemListViewModel ItemListViewModel { get; set; }
        private IEventAggregator iEventAggregator;
        public Dictionary<string, Action> PropertyChangedMap;

        public ItemListView()
        {
            InitializeComponent();
            this.iEventAggregator = Event.EventInstance.EventAggregator;

            ItemListViewModel = new ItemListViewModel(Event.EventInstance.EventAggregator);
            this.DataContext = ItemListViewModel;

            SetPropertyChangedMap();
            ItemListViewModel.PropertyChanged += PropertyChangedHandler;

        }


        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChangedMap.ContainsKey(e.PropertyName))
            {
                PropertyChangedMap[e.PropertyName]();
            }
        }

        private void SetPropertyChangedMap()
        {
            PropertyChangedMap = new Dictionary<string, Action>()
            {
                { "SelectedItemIndex", () => {
                    SetItemSelected(ItemListViewModel.SelectedItemIndex);
                }},
                { "FilterVisibility", () => {
                    filterControl.ItemListFilterViewModel.FilterVisibility = ItemListViewModel.FilterVisibility;
                }}
            };
        }

        private void Itemlist_Loaded(object sender, RoutedEventArgs e)
        {
            SetItemSelected(ItemListViewModel.SelectedItemIndex); 
        }

        public void SetItemSelected(int newSelectedIndex)
        {
            itemlist.Dispatcher.Invoke((Action)delegate
            {
                if ((itemlist.Items.Count > 0) && (itemlist.Columns.Count > 0))
                {
                    if (itemlist.CurrentCell.IsValid)
                    {
                        itemlist.SelectedCells.Remove(itemlist.CurrentCell);
                    }

                    itemlist.CurrentCell = new DataGridCellInfo(itemlist.Items[newSelectedIndex], itemlist.Columns[0]);
                    itemlist.SelectedCells.Add(itemlist.CurrentCell);

                    itemlist.ScrollIntoView(itemlist.CurrentCell.Item, itemlist.CurrentCell.Column);
                }
            });
            
        }

    }
}