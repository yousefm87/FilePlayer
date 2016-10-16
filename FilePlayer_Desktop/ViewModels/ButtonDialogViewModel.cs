using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;
using System.Linq;

using FilePlayer.Model;
using System;

namespace FilePlayer.ViewModels
{
    public class ButtonDialogViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        private SubscriptionToken dialogActionToken;
        private Dictionary<string, Action> EventMap { get; set; }
        
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

        private string DialogName
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
            SetButtonDialogPreset(dialogType);
            this.SelectedButtonIndex = 0;

            this.iEventAggregator = iEventAggregator;

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


        public void SetButtonDialogPreset(string dialogType)
        {
            switch (dialogType)
            {
                case "ITEM_LIST_PAUSE_OPEN":
                    this.DialogName = "ITEMLIST_PAUSE";
                    this.ButtonNames = new string[] { "Exit", "Reload Consoles", "Upload Game Data" };
                    this.ButtonResponses = new string[] { "EXIT", "UPDATE_ITEMLISTS", "GAMEDATA_UPLOAD" };
                    break;
                case "ITEM_LIST_CONFIRMATION_OPEN":
                    this.DialogName = "ITEMLIST_CONFIRMATION";
                    this.ButtonNames = new string[] { "Open", "Search For Data", "Delete Data" };
                    this.ButtonResponses = new string[] { "FILE_OPEN", "FILE_SEARCH_DATA", "FILE_DELETE_DATA" };
                    break;
                case "APP_PAUSE_OPEN":
                    this.DialogName = "APP_PAUSE";
                    this.ButtonNames = new string[] { "Return to App", "Close App", "Exit" };
                    this.ButtonResponses = new string[] { "RETURN_TO_APP", "CLOSE_APP", "EXIT" };
                    break;
                default:
                    throw new ArgumentException("Button Dialog Type '" + dialogType + "' not recognized!");
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
