using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Interactivity.InteractionRequest;
using System.Windows.Media;
using Microsoft.Practices.Prism.PubSubEvents;
using FilePlayer.Model;
using FilePlayer.ViewModels;


namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ItemListPauseView.xaml
    /// </summary>
    public partial class GameRetrieverProgress : IInteractionRequestAware
    {

        private IEventAggregator iEventAggregator;
        public SubscriptionToken pauseViewActionToken;

        public string[] buttonActions;
        public Button[] buttons;

        // Both the FinishInteraction and Notification properties will be set by the PopupWindowAction
        // when the popup is shown.
        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }

        public GameRetrieverProgressViewModel GameRetrieverProgressViewModel { get; set; }

        public GameRetrieverProgress()
        {

            GameRetrieverProgressViewModel = new GameRetrieverProgressViewModel(Event.EventInstance.EventAggregator);

            InitializeComponent();
            Init();
            this.Topmost = true;

            pauseViewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ItemListPauseViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }

        public void Init()
        {
            iEventAggregator = Event.EventInstance.EventAggregator;


        }

        void PerformViewAction(object sender, ItemListPauseViewEventArgs e)
        {
            switch (e.action)
            {
                

            }

        }


        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
            if (pauseViewActionToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ItemListPauseViewEventArgs>>().Unsubscribe(pauseViewActionToken);
            }
        }
    }
}