using FilePlayer.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using FilePlayer.Model;
using System.Collections.Generic;
using System;
using System.Collections;

namespace FilePlayer
{

    public enum ApplicationState
    {
        None = 0,
        ControllerNotFound=1,
        ItemlistBrowse = 2,
        ItemPlay = 3,
        FilterMain = 4,
        CharGetter = 5,
        VerticalOptionSelecter = 6,
        ButtonDialog = 7,
        SearchGameData = 8
    }

    class ControllerHandler
    {
        private IEventAggregator iEventAggregator;
        private SubscriptionToken controllerSubToken = null;
        Dictionary<ApplicationState, Dictionary<string, Action>> handlerMap;

        private Stack stateStack = new Stack();


        public void SendEvent<T>(T eventArgs)
        {
            this.iEventAggregator.GetEvent<PubSubEvent<T>>().Publish(eventArgs);
        }

        public ControllerHandler(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;
            InitMaps();
            
            SetState(ApplicationState.ItemlistBrowse, true);

            controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                (controllerEventArgs) =>
                {
                    ControllerEventHandler(controllerEventArgs);
                }
            );
        }
                

        public void SetState(ApplicationState state, Boolean isOn)
        {
            if (isOn)
            {
                stateStack.Push(state);
            }
            else
            {
                Stack states = new Stack();

                ApplicationState currState = (ApplicationState) stateStack.Pop();

                while ((stateStack.Count > 0) && (currState != state)) //in case windows are closed out of order
                {
                    states.Push(currState);
                    currState = (ApplicationState) stateStack.Pop();
                }

                for (int i = 0; i < states.Count; i++) //put remaining back in stack
                {
                    stateStack.Push(states.Pop());
                }

                if (currState != state)
                {
                    stateStack.Push(currState);
                }
            }
        }


        void ControllerEventHandler(ControllerEventArgs e)
        {
            switch (e.action)
            {
                case "CONTROLLER_NOT_FOUND":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONTROLLER_NOT_FOUND", new string[] { }));
                    if ((ApplicationState)stateStack.Peek() != ApplicationState.ControllerNotFound)
                    {
                        SetState(ApplicationState.ControllerNotFound, true);
                    }
                    break;
                case "CONTROLLER_CONNECTED":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONTROLLER_CONNECTED", new string[] { }));
                    if ((ApplicationState)stateStack.Peek() == ApplicationState.ControllerNotFound)
                    {
                        SetState(ApplicationState.ControllerNotFound, false);
                    }
                    break;
                default:
                    ApplicationState currState = (ApplicationState)stateStack.Peek();
                    if (handlerMap.ContainsKey(currState))
                    {
                        if (handlerMap[currState].ContainsKey(e.action))
                        {
                            handlerMap[currState][e.action]();
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
                { "START", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("OPEN_CONSOLE_SAMPLE")); } },
                { "BACK", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("OPEN_DATA_FOLDER")); } }
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
                { "A", () => { SendEvent<SearchGameDataEventArgs>(new SearchGameDataEventArgs("SEARCHGAMEDATA_SELECT")); } },
                { "B", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("SEARCHGAMEDATA_CLOSE")); } },
                { "X", () => { } },
                { "Y", () => { } },
                { "DUP", () => { SendEvent<SearchGameDataEventArgs>(new SearchGameDataEventArgs("SEARCHGAMEDATA_MOVE_UP")); } },
                { "DDOWN", () => { SendEvent<SearchGameDataEventArgs>(new SearchGameDataEventArgs("SEARCHGAMEDATA_MOVE_DOWN")); } },
                { "DLEFT", () => { SendEvent<SearchGameDataEventArgs>(new SearchGameDataEventArgs("SEARCHGAMEDATA_MOVE_LEFT")); } },
                { "DRIGHT", () => { SendEvent<SearchGameDataEventArgs>(new SearchGameDataEventArgs("SEARCHGAMEDATA_MOVE_RIGHT")); } },
                { "LSHOULDER", () => { } },
                { "RSHOULDER", () => { } },
                { "GUIDE", () => { SendEvent<ViewEventArgs>(new ViewEventArgs("SEARCHGAMEDATA_CLOSE")); } }
            };

            Dictionary<string, Action> NoneMap = new Dictionary<string, Action>();

            handlerMap = new Dictionary<ApplicationState, Dictionary<string, Action>>()
            {
                { ApplicationState.ItemlistBrowse, ItemlistViewEventMap },
                { ApplicationState.None, NoneMap },
                { ApplicationState.ItemPlay, ItemPlayingEventMap },
                { ApplicationState.FilterMain, FilterEventMap },
                { ApplicationState.CharGetter, CharGetterEventMap },
                { ApplicationState.VerticalOptionSelecter, VerticalOptionSelecterEventMap },
                { ApplicationState.ButtonDialog, ButtonDialogEventMap },
                { ApplicationState.SearchGameData, SearchGameDataEventMap }

            };
        }
        
    }
}
