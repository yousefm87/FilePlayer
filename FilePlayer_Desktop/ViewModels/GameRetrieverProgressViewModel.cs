using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;


namespace FilePlayer.ViewModels
{
    public class GameRetrieverProgressEventArgs : ViewEventArgs
    {
        public GameRetrieverProgressEventArgs(string action) : base(action) { }
        public GameRetrieverProgressEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }





    public class GameRetrieverProgressViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        private SubscriptionToken progressToken;

        private string platformDenominator;
        private string platformNumerator;
        private string platformPercentage;
        private string platformName;

        private string gameDenominator;
        private string gameNumerator;
        private string gamePercentage;
        private string gameName;

        public string PlatformDenominator
        {
            get { return platformDenominator; }
            set
            {
                platformDenominator = value;
                OnPropertyChanged("PlatformDenominator");
            }
        }

        public string PlatformNumerator
        {
            get { return platformNumerator; }
            set
            {
                platformNumerator = value;
                OnPropertyChanged("PlatformNumerator");
            }
        }

        public string PlatformPercentage
        {
            get { return platformPercentage; }
            set
            {
                platformPercentage = value;
                OnPropertyChanged("PlatformPercentage");
            }
        }

        public string PlatformName
        {
            get { return platformName; }
            set
            {
                platformName = value;
                OnPropertyChanged("PlatformName");
            }
        }

        public string GameDenominator
        {
            get { return gameDenominator; }
            set
            {
                gameDenominator = value;
                OnPropertyChanged("GameDenominator");
            }
        }

        public string GameNumerator
        {
            get { return gameNumerator; }
            set
            {
                gameNumerator = value;
                OnPropertyChanged("GameNumerator");
            }
        }

        public string GamePercentage
        {
            get { return gamePercentage; }
            set
            {
                gamePercentage = value;
                OnPropertyChanged("GamePercentage");
            }
        }

        public string GameName
        {
            get { return gameName; }
            set
            {
                gameName = value;
                OnPropertyChanged("GameName");
            }
        }

        public GameRetrieverProgressViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;

            progressToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe((viewEventArgs) =>
                                                                                                    {
                                                                                                        PerformViewAction(this, viewEventArgs);
                                                                                                    }
                                                                                                    );
            Init();
        }

        public void Init()
        {
            PlatformDenominator = "?";
            PlatformNumerator = "?";
            PlatformPercentage = "?";
            PlatformName = "?";

            GameDenominator = "?";
            GameNumerator = "?";
            GamePercentage = "?";
            GameName = "?";
        }


        void PerformViewAction(object sender, ViewEventArgs e)
        {
            switch (e.action)
            {

                case "GIANTBOMB_PLATFORM_UPLOAD_START":
                    PlatformName = e.addlInfo[0];
                    PlatformNumerator = e.addlInfo[1];
                    PlatformDenominator = e.addlInfo[2];
                    PlatformPercentage = e.addlInfo[3];
                    break;

                case "GIANTBOMB_GAME_UPLOAD_START":
                    //gameProgressText.Dispatcher.Invoke((Action)delegate
                    //{
                    GameName = e.addlInfo[0];
                    GameNumerator = e.addlInfo[1];
                    GameDenominator = e.addlInfo[2];
                    GamePercentage = e.addlInfo[3];
                    //});
                    break;
            }

        }
    }
}