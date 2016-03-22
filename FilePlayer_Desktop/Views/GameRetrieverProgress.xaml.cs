using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Interactivity.InteractionRequest;
using System.Windows.Media;
using Microsoft.Practices.Prism.PubSubEvents;
using FilePlayer.Model;
using FilePlayer.ViewModels;
using System.Windows.Data;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for GameRetrieverProgress.xaml
    /// </summary>
    public partial class GameRetrieverProgress : Window
    {

        private IEventAggregator iEventAggregator;
        public SubscriptionToken progressActionToken;

        public string[] buttonActions;
        public Button[] buttons;
        
        public GameRetrieverProgressViewModel GameRetrieverProgressViewModel { get; set; }

        public GameRetrieverProgress()
        {
            InitializeComponent();

            GameRetrieverProgressViewModel = new GameRetrieverProgressViewModel(Event.EventInstance.EventAggregator);
            
            this.DataContext = GameRetrieverProgressViewModel;

            this.Topmost = true;
            this.Topmost = false;

            iEventAggregator = Event.EventInstance.EventAggregator;

        }

    }
}