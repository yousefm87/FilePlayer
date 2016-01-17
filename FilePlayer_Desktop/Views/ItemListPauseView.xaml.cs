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
    public partial class ItemListPauseView : IInteractionRequestAware
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

        public ItemListPauseViewModel ItemListPauseViewModel { get; set; }

        public ItemListPauseView()
        {

            ItemListPauseViewModel = new ItemListPauseViewModel(Event.EventInstance.EventAggregator);

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

            selectedButtonIndex = 0;
            buttons = new Button[] { closePauseButton, exitButton, updateGameDataButton };
            buttonActions = new string[] { "ITEMLIST_PAUSE_CLOSE", "EXIT", "GET_DATA_FROM_GIANTBOMB"};

            for (int i = 0; i < buttons.Length; i++)
            {
                bool isSelected = (i == selectedButtonIndex);
                SetButtonSelected(buttons[i], isSelected);
            }
        }

        void PerformViewAction(object sender, ItemListPauseViewEventArgs e)
        {
            switch (e.action)
            {
                case "ITEMLIST_PAUSE_MOVE_UP":
                    MoveUp();
                    break;
                case "ITEMLIST_PAUSE_MOVE_DOWN":
                    MoveDown();
                    break;
                case "ITEMLIST_PAUSE_SELECT_BUTTON":
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
                });
            }
            else
            {
                buttonPanel.Dispatcher.Invoke((Action)delegate
                {
                    button.SetResourceReference(Control.StyleProperty, "unselBtnStyle");
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
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ITEMLIST_PAUSE_CLOSE", new string[] { response }));
            });
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
