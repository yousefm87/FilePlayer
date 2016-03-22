using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilePlayer;
using System.Collections.ObjectModel;

namespace FilePlayer.ViewModels
{
    public class SearchGameDataViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        private ObservableCollection<List<GameRetriever.GameData>> gameData;
        private int selectedRow;
        private int selectedCol;
        public int SelectedRow
        {
            get { return this.selectedRow; }
            set
            {
                selectedRow = value;
                OnPropertyChanged("SelectedRow");
            }
        }

        public int SelectedCol
        {
            get { return this.selectedCol; }
            set
            {
                selectedCol = value;
                OnPropertyChanged("SelectedCol");
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

        public SearchGameDataViewModel(IEventAggregator iEventAggregator, string gameQuery)
        {
            this.iEventAggregator = iEventAggregator;
            SelectedCol = 0;
            SelectedRow = 0;

            
            GameData = GameRetriever.GetGameDataSetLists(gameQuery);
        }


    }
}