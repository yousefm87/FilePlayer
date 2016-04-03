using FilePlayer.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using FilePlayer.Model;
using System.Collections.Generic;
using System;

namespace FilePlayer
{

    public enum ApplicationState
    {
        None,
        Last,
        ItemlistBrowse,
        ItemPlay,
        FilterMain,
        CharGetter,
        VerticalOptionSelecter,
        ButtonDialog,
        SearchGameData
    }

    class ControllerHandler2
    {
        private IEventAggregator iEventAggregator;
        private SubscriptionToken controllerSubToken = null;
        private SubscriptionToken controllerErrorSubToken = null;
        Dictionary<string, Dictionary<string, Action>> handlerMap;

        //private ApplicationState CurrentState = ApplicationState.None;
        //private ApplicationState PreviousState = ApplicationState.None;

        private string CurrentState = "NONE";
        private string PreviousState = "NONE";

        public void SendEvent<T>(T eventArgs)
        {
            this.iEventAggregator.GetEvent<PubSubEvent<T>>().Publish(eventArgs);
        }

        public ControllerHandler2(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;
            InitMaps();

            SetControllerState("ITEMLIST_BROWSE");

            controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                (controllerEventArgs) =>
                {
                    ControllerEventHandler(controllerEventArgs);
                }
            );
        }
                

        public void SetControllerState(string state)
        {
            if (state.Equals("LAST"))
            {
                CurrentState = PreviousState;
            }
            else
            {
                PreviousState = CurrentState;
                CurrentState = state;
            }

        }

        void ControllerEventHandler(ControllerEventArgs e)
        {
            switch (e.action)
            {
                case "CONTROLLER_NOT_FOUND":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONTROLLER_NOT_FOUND", new string[] { }));
                    break;
                case "CONTROLLER_CONNECTED":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONTROLLER_CONNECTED", new string[] { }));
                    break;
                default:
                    if (handlerMap.ContainsKey(CurrentState))
                    {
                        if (handlerMap[CurrentState].ContainsKey(e.action))
                        {
                            handlerMap[CurrentState][e.action]();
                        }
                    }
                    break;

            }
        }

        public void InitMaps()
        {

            Dictionary<string, Action> ItemlistViewEventMap = new Dictionary<string, Action>()
            {
                { "A", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("BUTTONDIALOG_OPEN", new string[] { "ITEM_LIST_CONFIRMATION_OPEN" })); } },
                { "B", () => { } },
                { "Y", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("OPEN_FILTER", new string[] { "" })); } },
                { "DUP", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("ITEMLIST_MOVE_UP", new string[] { 1.ToString() })); } },
                { "DDOWN", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("ITEMLIST_MOVE_DOWN", new string[] { 1.ToString() })); } },
                { "DLEFT", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("ITEMLIST_MOVE_LEFT", new string[] { 1.ToString() })); } },
                { "DRIGHT", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("ITEMLIST_MOVE_RIGHT", new string[] { 1.ToString() })); } },
                { "LSHOULDER", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("ITEMLIST_MOVE_UP", new string[] { 10.ToString() })); } },
                { "RSHOULDER", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("ITEMLIST_MOVE_DOWN", new string[] { 10.ToString() })); } },
                { "GUIDE", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("BUTTONDIALOG_OPEN", new string[] { "ITEM_LIST_PAUSE_OPEN" })); } },
                { "START", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("OPEN_CONSOLE_SAMPLE")); } }
            };

            Dictionary<string, Action> ButtonDialogEventMap = new Dictionary<string, Action>()
            {
                { "A", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("BUTTONDIALOG_SELECT_BUTTON")); } },
                { "B", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("BUTTONDIALOG_CLOSE")); } },
                { "X", () => { } },
                { "Y", () => { } },
                { "DUP", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("BUTTONDIALOG_MOVE_UP")); } },
                { "DDOWN", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("BUTTONDIALOG_MOVE_DOWN")); } },
                { "DLEFT", () => { } },
                { "DRIGHT", () => { } },
                { "LSHOULDER", () => { } },
                { "RSHOULDER", () => { } },
                { "GUIDE", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("BUTTONDIALOG_CLOSE")); } }
            };

            Dictionary<string, Action> ItemPlayingEventMap = new Dictionary<string, Action>()
            {
                { "A", () => { } },
                { "B", () => { } },
                { "X", () => { } },
                { "Y", () => { } },
                { "DUP", () => { } },
                { "DDOWN", () => { } },
                { "DLEFT", () => { } },
                { "DRIGHT", () => { } },
                { "LSHOULDER", () => { } },
                { "RSHOULDER", () => { } },
                { "GUIDE", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("BUTTONDIALOG_OPEN", new string[] { "APP_PAUSE_OPEN" })); } }
            };

            Dictionary<string, Action> FilterEventMap = new Dictionary<string, Action>()
            {
                { "A", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("FILTER_SELECT_CONTROL", new string[] { "" })); } },
                { "B", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("CLOSE_FILTER", new string[] { "" })); } },
                { "X", () => { } },
                { "Y", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("CLOSE_FILTER", new string[] { "" })); } },
                { "DUP", () => { } },
                { "DDOWN", () => { } },
                { "DLEFT", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("FILTER_MOVE_LEFT")); } },
                { "DRIGHT", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("FILTER_MOVE_RIGHT")); } },
                { "LSHOULDER", () => { } },
                { "RSHOULDER", () => { } },
                { "GUIDE", () => { } }
            };

            Dictionary<string, Action> CharGetterEventMap = new Dictionary<string, Action>()
            {
                { "A", () => { SendEvent<CharGetterEventArgs>(new CharGetterEventArgs("CHAR_SELECT", new string[] { "" })); } },
                { "B", () => { SendEvent<CharGetterEventArgs>(new CharGetterEventArgs("CHAR_CLOSE", new string[] { "" })); } },
                { "X", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("CHAR_BACK", new string[] { "" })); } },
                { "Y", () => { } },
                { "DUP", () => { SendEvent<CharGetterEventArgs>(new CharGetterEventArgs("CHAR_MOVE_UP")); } },
                { "DDOWN", () => { SendEvent<CharGetterEventArgs>(new CharGetterEventArgs("CHAR_MOVE_DOWN")); } },
                { "DLEFT", () => { SendEvent<CharGetterEventArgs>(new CharGetterEventArgs("CHAR_MOVE_LEFT")); } },
                { "DRIGHT", () => { SendEvent<CharGetterEventArgs>(new CharGetterEventArgs("CHAR_MOVE_RIGHT")); } },
                { "LSHOULDER", () => { SendEvent<CharGetterEventArgs>(new CharGetterEventArgs("CHAR_SWITCHCHARSET_LEFT")); } },
                { "RSHOULDER", () => { SendEvent<CharGetterEventArgs>(new CharGetterEventArgs("CHAR_SWITCHCHARSET_RIGHT")); } },
                { "GUIDE", () => { } }
            };

            Dictionary<string, Action> VerticalOptionSelecterEventMap = new Dictionary<string, Action>()
            {
                { "A", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("VOS_SELECT", new string[] { "" })); } },
                { "B", () => { } },
                { "X", () => { } },
                { "Y", () => { } },
                { "DUP", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("VOS_MOVE_UP")); } },
                { "DDOWN", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("VOS_MOVE_DOWN")); } },
                { "DLEFT", () => { } },
                { "DRIGHT", () => { } },
                { "LSHOULDER", () => { } },
                { "RSHOULDER", () => { } },
                { "GUIDE", () => { } }
            };

            Dictionary<string, Action> SearchGameDataEventMap = new Dictionary<string, Action>()
            {
                { "A", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("SEARCHGAMEDATA_SELECT", new string[] { "" })); } },
                { "B", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("SEARCHGAMEDATA_CLOSE", new string[] { "" })); } },
                { "X", () => { } },
                { "Y", () => { } },
                { "DUP", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("SEARCHGAMEDATA_MOVE_UP")); } },
                { "DDOWN", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("SEARCHGAMEDATA_MOVE_DOWN")); } },
                { "DLEFT", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("SEARCHGAMEDATA_MOVE_LEFT")); } },
                { "DRIGHT", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("SEARCHGAMEDATA_MOVE_RIGHT")); } },
                { "LSHOULDER", () => { } },
                { "RSHOULDER", () => { } },
                { "GUIDE", () => { } }
            };

            Dictionary<string, Action> NoneMap = new Dictionary<string, Action>();

            handlerMap = new Dictionary<string, Dictionary<string, Action>>()
            {
                { "ITEMLIST_BROWSE", ItemlistViewEventMap },
                { "NONE", NoneMap },
                { "ITEM_PLAY", ItemPlayingEventMap },
                { "FILTER_MAIN", FilterEventMap },
                { "CHAR_GETTER", CharGetterEventMap },
                { "VERTICAL_OPTION_SELECTER", VerticalOptionSelecterEventMap },
                { "BUTTON_DIALOG", ButtonDialogEventMap },
                { "SEARCH_GAME_DATA", SearchGameDataEventMap }

            };
        }
        
    }
}
