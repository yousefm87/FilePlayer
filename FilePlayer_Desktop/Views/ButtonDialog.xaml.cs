using FilePlayer.Model;
using FilePlayer.ViewModels;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ButtonDialog.xaml
    /// </summary>
    public partial class ButtonDialog : MetroWindow
    {
        private IEventAggregator iEventAggregator;
        public SubscriptionToken dialogActionToken;

        public string[] buttonActions;
        public Button[] buttons;
        public int selectedButtonIndex;

        public ButtonDialogViewModel ButtonDialogViewModel { get; set; }

        public ButtonDialog() : this("", new string[] { }, new string[] { }, "")
        { }

        public ButtonDialog(string dialogName, string[] buttonNames, string[] buttonResponses, string closeEvent)
        {
            iEventAggregator = Event.EventInstance.EventAggregator;

            ButtonDialogViewModel = new ButtonDialogViewModel(Event.EventInstance.EventAggregator, dialogName, buttonNames, buttonResponses, closeEvent);
            this.DataContext = ButtonDialogViewModel;

            InitializeComponent();

            this.Topmost = true;

            SetSelectedButton(0);

            dialogActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }

        void PerformViewAction(object sender, ViewEventArgs e)
        {
            switch (e.action)
            {
                case "BUTTONDIALOG_MOVE_UP":
                    MoveUp();
                    break;
                case "BUTTONDIALOG_MOVE_DOWN":
                    MoveDown();
                    break;
                case "BUTTONDIALOG_SELECT_BUTTON":
                    SelectButton();
                    break;
            }

        }



        public void MoveUp()
        {
            if (ButtonDialogViewModel.SelectedButtonIndex > 0)
            {
                SetSelectedButton(ButtonDialogViewModel.SelectedButtonIndex - 1);
            }
        }
         
        public void MoveDown()
        {
            if (ButtonDialogViewModel.SelectedButtonIndex < (buttonGrid.Items.Count - 1))
            {
                SetSelectedButton(ButtonDialogViewModel.SelectedButtonIndex + 1);
            }
        }
              

        public void SelectButton()
        {
            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_SELECT", new string[] { ButtonDialogViewModel.DialogName, ButtonDialogViewModel.ButtonResponses.ElementAt(ButtonDialogViewModel.SelectedButtonIndex) }));
            //this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { ButtonDialogViewModel.CloseEvent }));
        }




        public void SetSelectedButton(int buttonIndex)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                ButtonDialogViewModel.SelectedButtonIndex = buttonIndex;
            });
        }



        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
            if (dialogActionToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Unsubscribe(dialogActionToken);
            }
        }
    }


}
