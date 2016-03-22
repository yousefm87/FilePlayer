using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;
using System.Linq;

using FilePlayer.Model;

namespace FilePlayer.ViewModels
{
    public class ButtonDialogPreset
    {
        public string dialogName { get; set; }
        public string[] buttonNames { get; set; }
        public string[] buttonActions { get; set; }

        public ButtonDialogPreset(string dialogType)
        {
            switch (dialogType)
            {
                case "ITEM_LIST_PAUSE_OPEN":
                    dialogName = "ITEMLIST_PAUSE";
                    buttonNames = new string[] { "Exit", "Reload Consoles", "Upload Game Data" };
                    buttonActions = new string[] { "EXIT", "UPDATE_ITEMLISTS", "GAMEDATA_UPLOAD" };
                    break;
                case "ITEM_LIST_CONFIRMATION_OPEN":
                    dialogName = "ITEMLIST_CONFIRMATION";
                    buttonNames = new string[] { "Open", "Search For Data", "Delete Data" };
                    buttonActions = new string[] { "FILE_OPEN", "FILE_SEARCH_DATA", "FILE_DELETE_DATA" };
                    break;
                case "APP_PAUSE_OPEN":
                    dialogName = "APP_PAUSE";
                    buttonNames = new string[] { "Return to App", "Close App", "Exit" };
                    buttonActions = new string[] { "RETURN_TO_APP", "CLOSE_APP", "EXIT" };
                    break;
                default:
                    dialogName = "";
                    buttonNames = new string[] { };
                    buttonActions = new string[] { };
                    break;
            }
        }
    }
        

    public class ButtonDialogViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        private SubscriptionToken dialogActionToken;
        private IEnumerable<string> buttonNames;
        private IEnumerable<string> buttonResponses;
        private int selectedButtonIndex;
        private string dialogName;
        private string closeEvent;
        

        public string CloseEvent
        {
            get { return this.closeEvent; }
            set
            {
                closeEvent = value;
                OnPropertyChanged("CloseEvent");
            }
        }

        public int SelectedButtonIndex
        {
            get { return this.selectedButtonIndex; }
            set
            {
                selectedButtonIndex = value;
                OnPropertyChanged("SelectedButtonIndex");
            }
        }
        public IEnumerable<string> ButtonNames
        {
            get { return this.buttonNames; }
            set
            {
                buttonNames = value;
                OnPropertyChanged("ButtonNames");
            }
        }

        public IEnumerable<string> ButtonResponses
        {
            get { return this.buttonResponses; }
            set
            {
                buttonResponses = value;
                OnPropertyChanged("ButtonResponses");
            }
        }

        public string DialogName
        {
            get { return this.dialogName; }
            set
            {
                dialogName = value;
                OnPropertyChanged("DialogName");
            }
        }


        public ButtonDialogViewModel(IEventAggregator iEventAggregator, string dialogType)
        {
            this.iEventAggregator = iEventAggregator;

            ButtonDialogPreset preset = new ButtonDialogPreset(dialogType);

            this.DialogName = preset.dialogName;
            this.ButtonNames = preset.buttonNames;
            this.ButtonResponses = preset.buttonActions;

            this.SelectedButtonIndex = 0;

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

        public void ReturnController()
        {
            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { CloseEvent }));
        }



        public void MoveUp()
        {
            if (SelectedButtonIndex > 0)
            {
                SelectedButtonIndex = SelectedButtonIndex - 1;
            }
        }

        public void MoveDown()
        {
            if (SelectedButtonIndex < (buttonNames.Count() - 1))
            {
                SelectedButtonIndex = SelectedButtonIndex + 1;
            }
        }

        public void SelectButton()
        {
            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_SELECT", new string[] { DialogName, ButtonResponses.ElementAt(SelectedButtonIndex) }));
        }

    }
}
