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
    /// Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : UserControl, IInteractionRequestAware
    {

        private IEventAggregator iEventAggregator;
        public SubscriptionToken confirmViewActionToken;

        public ConfirmationDialog()
        {
            iEventAggregator = Event.EventInstance.EventAggregator;
            InitializeComponent();

            SetSelectedButton("nobutton");

            confirmViewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }

        Brush selectedButtonBackground = Brushes.DodgerBlue;
        Brush selectedButtonForeground = Brushes.White;
        Brush unselectedButtonBackground = Brushes.AliceBlue;
        Brush unselectedButtonForeground = Brushes.Black;



        void PerformViewAction(object sender, ConfirmationViewEventArgs e)
        {
            switch (e.action)
            {
                case "CONFIRM_MOVE_LEFT":
                    SetSelectedButton("NOBUTTON");
                    break;
                case "CONFIRM_MOVE_RIGHT":
                    SetSelectedButton("YESBUTTON");
                    break;
                case "CONFIRM_SELECT_BUTTON":

                    

                    this.Dispatcher.Invoke((Action)delegate
                    {
                        string response;

                        if (this.FinishInteraction != null)
                            this.FinishInteraction();
                    
                        if (yesButton.Background == Brushes.DodgerBlue)
                        {
                            response = "YES";
                        }
                        else
                        {
                            response = "NO";
                        }
                    
                        this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("CONFIRM_CLOSE", new string[] { response }));
                    });
                    break;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.FinishInteraction != null)
                this.FinishInteraction();
        }

        // Both the FinishInteraction and Notification properties will be set by the PopupWindowAction
        // when the popup is shown.
        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }

        public void SetSelectedButton(string selectedButton)
        {
            switch (selectedButton.ToLower())
            {
                case "yesbutton":

                    buttonPanel.Dispatcher.Invoke((Action)delegate
                    {
                        noButton.Background = unselectedButtonBackground;
                        noButton.Foreground = unselectedButtonForeground;
                        yesButton.Background = selectedButtonBackground;
                        yesButton.Foreground = selectedButtonForeground;
                    });
                    break;
                case "nobutton":
                    buttonPanel.Dispatcher.Invoke((Action)delegate
                    {
                        noButton.Background = selectedButtonBackground;
                        noButton.Foreground = selectedButtonForeground;
                        yesButton.Background = unselectedButtonBackground;
                        yesButton.Foreground = unselectedButtonForeground;
                    });
                    break;
            }

        }
        
    }
}
