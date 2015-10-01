using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Interactivity.InteractionRequest;
using System.Windows.Media;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : UserControl, IInteractionRequestAware
    {
        public ConfirmationDialog()
        {

            InitializeComponent();

            SetSelectedButton("nobutton");
        }

        Brush selectedButtonBackground = Brushes.DodgerBlue;
        Brush selectedButtonForeground = Brushes.White;
        Brush unselectedButtonBackground = Brushes.AliceBlue;
        Brush unselectedButtonForeground = Brushes.Black;





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
                    noButton.Background = unselectedButtonBackground;
                    noButton.Foreground = unselectedButtonForeground;
                    yesButton.Background = selectedButtonBackground;
                    yesButton.Foreground = selectedButtonForeground;
                    break;
                case "nobutton":
                    noButton.Background = selectedButtonBackground;
                    noButton.Foreground = selectedButtonForeground;
                    yesButton.Background = unselectedButtonBackground;
                    yesButton.Foreground = unselectedButtonForeground;
                    break;
            }

        }
        
    }
}
