using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilePlayer;
using System.Collections.ObjectModel;
using FilePlayer.Model;

namespace FilePlayer.ViewModels
{
    public class SearchGameDataViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        private SubscriptionToken searchGameDataActionToken = null;
        private Dictionary<string, Action> eventMap;

        private ObservableCollection<List<GameRetriever.GameData>> gameData;

        private string gameQuery;
        private string titleBarText = "Searching for Game Data...";

        private int selectedRow;
        private int selectedCol;

        public int MaxCols;
        public int MaxRows;

        public string TitleBarText
        {
            get { return this.titleBarText; }
            set
            {
                this.titleBarText = value;
                OnPropertyChanged("TitleBarText");
            }
        }

        public string GameQuery
        {
            get { return this.gameQuery; }
            set
            {
                gameQuery = value;
                OnPropertyChanged("GameQuery");
            }
        }

        public int SelectedRow
        {
            get { return this.selectedRow; }
            set
            {
                if ((selectedRow != value) && (selectedRow >= 0))
                {
                    selectedRow = value;
                    OnPropertyChanged("SelectedRow");
                }
            }
        }

        public int SelectedCol
        {
            get { return this.selectedCol; }
            set
            {
                if ((selectedCol != value) && (selectedCol >= 0))
                {
                    selectedCol = value;
                    OnPropertyChanged("SelectedCol");
                }
            }
        }

        public ObservableCollection<List<GameRetriever.GameData>> GameData
        {
            get { return this.gameData; }
            set
            {
                gameData = value;
                OnPropertyChanged("GameData");
            }
        }

        public SearchGameDataViewModel(string _gameQuery)
        {
            iEventAggregator = Event.EventInstance.EventAggregator;
            SelectedCol = 0;
            SelectedRow = 0;
            
            TitleBarText = "Searching for " + _gameQuery + "...";

            GameData = GameRetriever.GetGameDataSetLists(_gameQuery);
            GameQuery = _gameQuery;

            eventMap = new Dictionary<string, Action>()
            {
                { "SEARCHGAMEDATA_MOVE_LEFT", () => { SelectedCol = SelectedCol - 1; } },
                { "SEARCHGAMEDATA_MOVE_RIGHT", () => { SelectedCol = SelectedCol + 1; } },
                { "SEARCHGAMEDATA_MOVE_UP", () => { SelectedRow = SelectedRow - 1; } },
                { "SEARCHGAMEDATA_MOVE_DOWN", () => { SelectedRow = SelectedRow + 1; } },
                { "SEARCHGAMEDATA_SELECT", SelectItem }
            };

            searchGameDataActionToken = iEventAggregator.GetEvent<PubSubEvent<SearchGameDataEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    EventHandler(this, viewEventArgs);
                }
            );
        }

        private void EventHandler(object sender, SearchGameDataEventArgs e)
        {
            if (eventMap.ContainsKey(e.action))
            {
                eventMap[e.action]();
            }
        }

        public void SetRowCol(int row, int col)
        {
            selectedCol = col;
            selectedRow = row;
        }

        public void SelectItem()
        {
            string itemName = gameQuery;
            string itemDesc = GameData.ElementAt(SelectedRow).ElementAt(SelectedCol).GameDescription;
            string itemRel = GameData.ElementAt(SelectedRow).ElementAt(SelectedCol).ReleaseDate;
            string itemImgLoc = GameData.ElementAt(SelectedRow).ElementAt(SelectedCol).ImageURL;

            iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GAMEDATA_ADD_ITEM", new String[] { itemName, itemDesc, itemRel, itemImgLoc }));
        }

    }
}