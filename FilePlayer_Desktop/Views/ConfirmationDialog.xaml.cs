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
        public Button[] buttons;
        public string[] buttonActions;
        public int selectedButtonIndex;


        Brush selectedButtonBackground = Brushes.DodgerBlue;
        Brush selectedButtonForeground = Brushes.White;
        Brush unselectedButtonBackground = Brushes.AliceBlue;
        Brush unselectedButtonForeground = Brushes.Black;

        // Both the FinishInteraction and Notification properties will be set by the PopupWindowAction
        // when the popup is shown.
        public Action FinishInteraction { get; set; }
        public INotification Notification { get; set; }

        public ConfirmationDialogViewModel ConfirmationDialogViewModel { get; set; }

        public ConfirmationDialog()
        {
            ConfirmationDialogViewModel = new ConfirmationDialogViewModel(Event.EventInstance.EventAggregator);
            InitializeComponent();
            Init();

            confirmViewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Subscribe(
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
            buttons = new Button[] { noButton, yesButton };
            buttonActions = new string[] { "NO", "YES" };

            for (int i = 0; i < buttons.Length; i++)
            {
                bool isSelected = (i == selectedButtonIndex);
                SetButtonSelected(buttons[i], isSelected);
            }


        }




        void PerformViewAction(object sender, ConfirmationViewEventArgs e)
        {
            switch (e.action)
            {
                case "CONFIRM_MOVE_LEFT":
                    MoveLeft();
                    break;
                case "CONFIRM_MOVE_RIGHT":
                    MoveRight();
                    break;
                case "CONFIRM_SELECT_BUTTON":
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

        public void MoveLeft()
        {
            if (selectedButtonIndex != 0)
            {
                SetButtonSelected(buttons[selectedButtonIndex--], false);
                SetButtonSelected(buttons[selectedButtonIndex], true);
            }
        }

        public void MoveRight()
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

                if (this.FinishInteraction != null)
                {
                    this.FinishInteraction();
                }
                string response = buttonActions[selectedButtonIndex];
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONFIRM_CLOSE", new string[] { response }));

                Init();
            });
        }
    }
}
