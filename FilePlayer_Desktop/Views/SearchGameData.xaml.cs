﻿using FilePlayer.Model;
using FilePlayer.ViewModels;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.PubSubEvents;
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
using System.Windows.Shapes;
using FilePlayer.Views;
using GiantBomb.Api.Model;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for SearchGameData.xaml
    /// </summary>
    public partial class SearchGameData : MetroWindow
    {

        private IEventAggregator iEventAggregator;
        public SubscriptionToken dialogActionToken;

        public string[] buttonActions;
        public Button[] buttons;
        public int selectedButtonIndex;

        public SearchGameDataViewModel SearchGameDataViewModel { get; set; }

        public string GameQuery = "";

        public SearchGameData(string gameQuery)
        {
            InitializeComponent();
            iEventAggregator = Event.EventInstance.EventAggregator;

            SearchGameDataViewModel = new SearchGameDataViewModel(Event.EventInstance.EventAggregator, gameQuery);
            this.DataContext = SearchGameDataViewModel;

            GameQuery = gameQuery;

            Init();

            this.Topmost = true;

            dialogActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }

        public void Init()
        {
            int maxColCount = -1;

            for (int i = 0; i < SearchGameDataViewModel.GameData.Count(); i++) //Add Row Definitions
            {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength();
                gameGrid.RowDefinitions.Add(gridRow);


                if(SearchGameDataViewModel.GameData.ElementAt(i).GameReleases.Count() > maxColCount) //Get Column Count
                {
                    maxColCount = SearchGameDataViewModel.GameData.ElementAt(i).GameReleases.Count();
                }
            }

            maxColCount++; //for game column

            for (int i = 0; i < maxColCount; i++) //Add Column Definitions
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gameGrid.ColumnDefinitions.Add(gridCol);
            }

            for (int i = 0; i < SearchGameDataViewModel.GameData.Count(); i++) //Add Game as first column
            {
                GameRetriever.GameData currGame = SearchGameDataViewModel.GameData.ElementAt(i);
                SearchGameItem currGameItem = new SearchGameItem(currGame.GameName, currGame.ImageURL);


                Grid.SetRow(currGameItem, i);
                Grid.SetColumn(currGameItem, 0);

                gameGrid.Children.Add(currGameItem);
            }

            for (int i = 0; i < SearchGameDataViewModel.GameData.Count(); i++) //Add Releases in second+ columns
            {
                for (int j = 0; j < SearchGameDataViewModel.GameData.ElementAt(i).GameReleases.Count(); j++)
                {
                    GameRetriever.GameData rel = SearchGameDataViewModel.GameData.ElementAt(i).GameReleases.ElementAt(j);
                    SearchGameItem currGameItem = new SearchGameItem(rel.GameName, rel.ImageURL);


                    Grid.SetRow(currGameItem, i);
                    Grid.SetColumn(currGameItem, j + 1);

                    gameGrid.Children.Add(currGameItem);
                }
            }

            SetItemSelected(SearchGameDataViewModel.SelectedRow, SearchGameDataViewModel.SelectedCol, true); //select first item
        }

        public void SelectFirstItem()
        {
            SearchGameItem element = gameGrid.Children.OfType<SearchGameItem>().
            Where(e => Grid.GetColumn(e) == 0 && Grid.GetRow(e) == 0).First();

            this.Dispatcher.Invoke((Action)delegate
            {
                element.SetResourceReference(Control.StyleProperty, "SelectedStyle");
            });
        }

        public bool SetItemSelected(int row, int col, bool selected)
        {


            bool itemFound = false;

            this.Dispatcher.Invoke((Action)delegate
            {
                SearchGameItem element = gameGrid.Children.OfType<SearchGameItem>().Where(e => Grid.GetColumn(e) == col && Grid.GetRow(e) == row).FirstOrDefault();

                itemFound = (element != null);
                if (itemFound)
                {
                    if (selected)
                    {
                        element.BringIntoView();
                        element.SetResourceReference(Control.StyleProperty, "SelectedStyle");
                    }
                    else
                    {
                        element.SetResourceReference(Control.StyleProperty, "UnselectedStyle");
                    }
                }
                
            });

            return itemFound;
        }


        void PerformViewAction(object sender, ViewEventArgs e)
        {
            switch (e.action)
            {
                case "SEARCHGAMEDATA_MOVE_LEFT":
                    if (SetItemSelected(SearchGameDataViewModel.SelectedRow, SearchGameDataViewModel.SelectedCol - 1, true))
                    {
                        SetItemSelected(SearchGameDataViewModel.SelectedRow, SearchGameDataViewModel.SelectedCol, false);
                        SearchGameDataViewModel.SelectedCol = SearchGameDataViewModel.SelectedCol - 1;
                    }
                    break;
                case "SEARCHGAMEDATA_MOVE_RIGHT":
                    if (SetItemSelected(SearchGameDataViewModel.SelectedRow, SearchGameDataViewModel.SelectedCol + 1, true))
                    {
                        SetItemSelected(SearchGameDataViewModel.SelectedRow, SearchGameDataViewModel.SelectedCol, false);
                        SearchGameDataViewModel.SelectedCol = SearchGameDataViewModel.SelectedCol + 1;
                    }
                    break;
                case "SEARCHGAMEDATA_MOVE_UP":
                    if (SetItemSelected(SearchGameDataViewModel.SelectedRow - 1, SearchGameDataViewModel.SelectedCol, true))
                    {
                        SetItemSelected(SearchGameDataViewModel.SelectedRow, SearchGameDataViewModel.SelectedCol, false);
                        SearchGameDataViewModel.SelectedRow = SearchGameDataViewModel.SelectedRow - 1;
                    }
                    break;
                case "SEARCHGAMEDATA_MOVE_DOWN":
                    if (SetItemSelected(SearchGameDataViewModel.SelectedRow + 1, SearchGameDataViewModel.SelectedCol, true))
                    {
                        SetItemSelected(SearchGameDataViewModel.SelectedRow, SearchGameDataViewModel.SelectedCol, false);
                        SearchGameDataViewModel.SelectedRow = SearchGameDataViewModel.SelectedRow + 1;
                    }
                    break;
                case "SEARCHGAMEDATA_SELECT":
                    SelectItem();
                    break;

            }

        }


        public void SelectItem()
        {
            string itemName = GameQuery;
            string itemDesc;
            string itemRel;
            string itemImgLoc;

            if (SearchGameDataViewModel.SelectedCol == 0)
            {
                itemDesc = SearchGameDataViewModel.GameData.ElementAt(SearchGameDataViewModel.SelectedRow).GameDescription;
                itemRel = SearchGameDataViewModel.GameData.ElementAt(SearchGameDataViewModel.SelectedRow).ReleaseDate;
                itemImgLoc = SearchGameDataViewModel.GameData.ElementAt(SearchGameDataViewModel.SelectedRow).ImageURL;
            }
            else
            {
                itemDesc = SearchGameDataViewModel.GameData.ElementAt(SearchGameDataViewModel.SelectedRow).GameReleases.ElementAt(SearchGameDataViewModel.SelectedCol - 1).GameDescription;
                itemRel = SearchGameDataViewModel.GameData.ElementAt(SearchGameDataViewModel.SelectedRow).GameReleases.ElementAt(SearchGameDataViewModel.SelectedCol - 1).ReleaseDate;
                itemImgLoc = SearchGameDataViewModel.GameData.ElementAt(SearchGameDataViewModel.SelectedRow).GameReleases.ElementAt(SearchGameDataViewModel.SelectedCol - 1).ImageURL;
            }

            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GAMEDATA_SEARCH_ADD", new String[] { itemName, itemDesc, itemRel, itemImgLoc }));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
            if (dialogActionToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Unsubscribe(dialogActionToken);
            }
        }
    }
}