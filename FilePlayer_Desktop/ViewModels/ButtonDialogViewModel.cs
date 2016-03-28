using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;
using System.Linq;

using FilePlayer.Model;
using System;

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
        public Dictionary<string, Action> EventMap { get; private set; }
        
        private IEnumerable<string> buttonNames;
        private IEnumerable<string> buttonResponses;
        private int selectedButtonIndex;
        private string dialogName;

        public int SelectedButtonIndex
        {
            get { return selectedButtonIndex; }
            set
            {
                selectedButtonIndex = value;
                OnPropertyChanged("SelectedButtonIndex");
            }
        }
        public IEnumerable<string> ButtonNames
        {
            get { return buttonNames; }
            set
            {
                buttonNames = value;
                OnPropertyChanged("ButtonNames");
            }
        }

        public IEnumerable<string> ButtonResponses
        {
            get { return buttonResponses; }
            set
            {
                buttonResponses = value;
                OnPropertyChanged("ButtonResponses");
            }
        }

        public string DialogName
        {
            get { return dialogName; }
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

            EventMap = new Dictionary<string, Action>()
            {
                { "BUTTONDIALOG_MOVE_UP", MoveUp},
                { "BUTTONDIALOG_MOVE_DOWN", MoveDown},
                { "BUTTONDIALOG_SELECT_BUTTON", SelectButton}
            };

            dialogActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    EventHandler(viewEventArgs);
                }
            );
        }

        void EventHandler(ViewEventArgs e)
        {
            if (EventMap.ContainsKey(e.action))
            {
                EventMap[e.action]();
            }
        }

        public void OnWindowClosed(object sender, EventArgs e)
        {
            if (dialogActionToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Unsubscribe(dialogActionToken);
            }
        }


        private void MoveUp()
        {
            if (SelectedButtonIndex > 0)
            {
                SelectedButtonIndex = SelectedButtonIndex - 1;
            }
        }

        private void MoveDown()
        {
            if (SelectedButtonIndex < (buttonNames.Count() - 1))
            {
                SelectedButtonIndex = SelectedButtonIndex + 1;
            }
        }

        private void SelectButton()
        {
            iEventAggregator.GetEvent<PubSubEvent<ButtonDialogEventArgs>>().Publish(new ButtonDialogEventArgs(ButtonResponses.ElementAt(SelectedButtonIndex), new string[] { }));
        }

    }
}
