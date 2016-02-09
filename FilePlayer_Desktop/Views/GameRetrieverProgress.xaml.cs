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

            iEventAggregator = Event.EventInstance.EventAggregator;
            progressActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }

        public void Init()
        {
            
            

        }

        void PerformViewAction(object sender, ViewEventArgs e)
        {
            switch (e.action)
            {

            }

        }


        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
            if (progressActionToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Unsubscribe(progressActionToken);
            }
        }
    }
}