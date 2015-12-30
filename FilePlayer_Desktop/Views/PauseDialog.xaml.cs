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
    public partial class PauseDialog : IInteractionRequestAware
    {

        private IEventAggregator iEventAggregator;
        public SubscriptionToken pauseViewActionToken;

        public string[] buttonActions;
        public Button[] buttons;
        public int selectedButtonIndex;
        Brush selectedButtonBackground = Brushes.DodgerBlue;
        Brush selectedButtonForeground = Brushes.White;
        Brush unselectedButtonBackground = Brushes.AliceBlue;
        Brush unselectedButtonForeground = Brushes.Black;

        // Both the FinishInteraction and Notification properties will be set by the PopupWindowAction
        // when the popup is shown.
        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }

        public PauseDialogViewModel PauseDialogViewModel { get; set; }

        public PauseDialog()
        {

            PauseDialogViewModel = new PauseDialogViewModel(Event.EventInstance.EventAggregator);

            InitializeComponent();
            Init();
            this.Topmost = true;
            
            pauseViewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }
        
        public void Init()
        {
            iEventAggregator = Event.EventInstance.EventAggregator;

            selectedButtonIndex = 0;
            buttons = new Button[] { returnToAppButton, closeAppButton, closeAllButton };
            buttonActions = new string[] { "RETURN_TO_APP", "CLOSE_APP", "CLOSE_ALL" };
            
            for (int i = 0; i < buttons.Length; i++)
            {
                bool isSelected = (i == selectedButtonIndex);
                SetButtonSelected(buttons[i], isSelected);
            }
        }

        void PerformViewAction(object sender, PauseViewEventArgs e)
        {
            switch (e.action)
            {
                case "PAUSE_MOVE_UP":
                    MoveUp();
                    break;
                case "PAUSE_MOVE_DOWN":
                    MoveDown();
                    break;
                case "PAUSE_SELECT_BUTTON":
                    SelectButton();
                    break;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.FinishInteraction != null)
                this.FinishInteraction();
        }





        public void SetButtonSelected(Button button, bool isSelected)
        {
            if (isSelected)
            {
                buttonPanel.Dispatcher.Invoke((Action)delegate
                {
                    button.SetResourceReference(Control.StyleProperty, "selBtnStyle");
                    //button.Background = selectedButtonBackground;
                    //button.Foreground = selectedButtonForeground;
                });
            }
            else
            {
                buttonPanel.Dispatcher.Invoke((Action)delegate
                {
                    button.SetResourceReference(Control.StyleProperty, "unselBtnStyle");
                    //button.Background = unselectedButtonBackground;
                    //button.Foreground = unselectedButtonForeground;
                });
            }
        }

        public void MoveUp()
        {
            if (selectedButtonIndex != 0)
            {
                SetButtonSelected(buttons[selectedButtonIndex--], false);
                SetButtonSelected(buttons[selectedButtonIndex], true);
            }
        }

        public void MoveDown()
        {
            if (selectedButtonIndex != (buttons.Length - 1))
            {
                SetButtonSelected(buttons[selectedButtonIndex++], false);
                SetButtonSelected(buttons[selectedButtonIndex], true);
            }
        }

        public void SelectButton()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                string response = buttonActions[selectedButtonIndex];
                this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("PAUSE_CLOSE", new string[] { response }));
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
            if (pauseViewActionToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Unsubscribe(pauseViewActionToken);
            }
        }
    }
}
