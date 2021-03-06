﻿using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;

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
        private Dictionary<string, Action<string[]>> eventMap;

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
            Init();
            this.iEventAggregator = iEventAggregator;

            eventMap = new Dictionary<string, Action<string[]>>()
            {
                { "GIANTBOMB_PLATFORM_UPLOAD_START", UpdatePlatformInfo },
                { "GIANTBOMB_GAME_UPLOAD_START", UpdateGameInfo }
            };

            progressToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );


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
            if(eventMap.ContainsKey(e.action))
            {
                eventMap[e.action](e.addlInfo);
            }

        }

        public void UpdatePlatformInfo(string[] platformInfo)
        {
            PlatformName = platformInfo[0];
            PlatformNumerator = platformInfo[1];
            PlatformDenominator = platformInfo[2];
            PlatformPercentage = platformInfo[3];
        }

        public void UpdateGameInfo(string[] gameInfo)
        {
            GameName = gameInfo[0];
            GameNumerator = gameInfo[1];
            GameDenominator = gameInfo[2];
            GamePercentage = gameInfo[3];
        }
    }
}