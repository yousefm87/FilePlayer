using System;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;
using FilePlayer.Model;
using System.Threading;

namespace FilePlayer.ViewModels
{


    public class ConfirmationCommand 
    {
        private readonly Action<object> _handler;

        public ConfirmationCommand(Action<object> handler)
        {
            _handler = handler;
        }


        public bool CanExecute(object parameter)
        {
            return (parameter != null);
        }

        
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _handler(parameter);
        }
    }


    public class PauseCommand : ICommand
    {
        private readonly Action<object> _handler;

        public PauseCommand(Action<object> handler)
        {
            _handler = handler;
        }


        public bool CanExecute(object parameter)
        {
            return (parameter != null);
        }


        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _handler(parameter);
        }
    }

    public class ShellViewModel : ViewModelBase
    {

        private WindowState _shellWindowState = WindowState.Maximized;

        private IEventAggregator iEventAggregator;
        private SubscriptionToken shellActionToken;
        private SubscriptionToken buttonDialogToken;
        private SubscriptionToken filterToken;
        private Dictionary<string, Action> eventMap;
        private Dictionary<string, Action<string[]>> eventMapParam;
        private Dictionary<string, Action> buttonDialogEventMap;
        private Dictionary<string, Action<string[]>> filterEventMap;

        private bool buttonDialogState = false;
        private bool charGetterState = false;
        private bool controllerNotFoundState = false;
        private bool gameRetrieverProgressState = false;
        private bool searchGameDataState = false;
        private bool verticalOptionSelecterState = false;
        private string buttonDialogType;
        private string searchGameDataQuery;
        private XboxControllerInputProvider input;
        private Thread gamepadThread;
        private ControllerHandler controllerHandler;

        public string[] VerticalOptionData { get; private set; }
        public string[] CharGetterPoint { get; private set; }

        public bool ButtonDialogState
        {
            get { return buttonDialogState; }
            set
            {
                buttonDialogState = value;
                SendItemListShadeEvent(value);
                OnPropertyChanged("ButtonDialogState");
            }
        }

        public string ButtonDialogType
        {
            get { return buttonDialogType; }
            set
            {
                buttonDialogType = value;
            }
        }

        public bool CharGetterState
        {
            get { return charGetterState; }
            set
            {
                charGetterState = value;
                OnPropertyChanged("CharGetterState");
            }
        }

        public bool ControllerNotFoundState
        {
            get { return controllerNotFoundState; }
            set
            {
                if (value != controllerNotFoundState)
                {
                    SendItemListShadeEvent(value);
                    controllerNotFoundState = value;

                    OnPropertyChanged("ControllerNotFoundState");
                }

            }
        }

        public bool GameRetrieverProgressState
        {
            get { return gameRetrieverProgressState; }
            set
            {
                gameRetrieverProgressState = value;
                SendItemListShadeEvent(value);
                OnPropertyChanged("GameRetrieverProgressState");
            }
        }

        public bool SearchGameDataState
        {
            get { return searchGameDataState; }
            set
            {
                searchGameDataState = value;
                SendItemListShadeEvent(value);
                OnPropertyChanged("SearchGameDataState");
            }
        }
        

        public string SearchGameDataQuery
        {
            get { return searchGameDataQuery; }
            set
            {
                searchGameDataQuery = value;
                OnPropertyChanged("SearchGameDataQuery");
            }
        }

        public bool VerticalOptionSelecterState
        {
            get { return verticalOptionSelecterState; }
            set
            {
                verticalOptionSelecterState = value;
                OnPropertyChanged("VerticalOptionSelecterState");
            }
        }

        public WindowState ShellWindowState
        {
            get { return _shellWindowState; }
            set
            {
                _shellWindowState = value;
                OnPropertyChanged("ShellWindowState");
            }
        }

        

        public ShellViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;

            InitializeEventMaps();

            shellActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) => 
                {
                    EventHandler(viewEventArgs);
                }
            );


            

            buttonDialogToken = this.iEventAggregator.GetEvent<PubSubEvent<ButtonDialogEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    ButtonDialogHandler(viewEventArgs);
                }
            );



            filterToken = this.iEventAggregator.GetEvent<PubSubEvent<ItemListFilterEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    FilterHandler(viewEventArgs);
                }
            );



            input = new XboxControllerInputProvider(Event.EventInstance.EventAggregator);

            gamepadThread = new Thread(new ThreadStart(input.PollGamepad));
            gamepadThread.Start();

            controllerHandler = new ControllerHandler(Event.EventInstance.EventAggregator);

        }


        private void ButtonDialogHandler(ButtonDialogEventArgs e)
        {
            if (buttonDialogEventMap.ContainsKey(e.action))
            {
                ButtonDialogState = false;
                buttonDialogEventMap[e.action]();
            }
        }

        private void FilterHandler(ItemListFilterEventArgs e)
        {
            if (filterEventMap.ContainsKey(e.action))
            {
                filterEventMap[e.action](e.addlInfo);
            }
        }

        private void SendItemListShadeEvent(bool setShade)
        {
            if(setShade)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ADD_SHADE"));
            }
            else
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("REMOVE_SHADE"));
            }
        }

        private void EventHandler(ViewEventArgs e)
        {
            if (eventMap.ContainsKey(e.action))
            {
                eventMap[e.action]();
            }

            if (eventMapParam.ContainsKey(e.action))
            {
                eventMapParam[e.action](e.addlInfo);
            }

        }

        private void InitializeEventMaps()
        {
            eventMap = new Dictionary<string, Action>() 
            {
                { "CHAR_CLOSE", () => //Close CharGetter  
                    {
                        CharGetterState = false;
                        controllerHandler.SetControllerState("LAST");
                    }
                },
                { "CONTROLLER_NOT_FOUND", () => 
                    {
                        ControllerNotFoundState = true;
                    }
                },
                { "CONTROLLER_CONNECTED", () => 
                    {
                        ControllerNotFoundState = false;
                    }
                },
                { "BUTTONDIALOG_CLOSE", () =>
                    {
                        controllerHandler.SetControllerState("LAST");
                        ButtonDialogState = false;
                    }
                },
                { "SEARCHGAMEDATA_CLOSE", () =>
                    {
                        SearchGameDataState = false;
                        controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    }
                },
                {"GAMEDATA_ADD_ITEM", () =>
                    {
                        SearchGameDataState = false;
                        controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    }
                },
                { "GIANTBOMB_UPLOAD_COMPLETE", () =>
                    {
                        GameRetrieverProgressState = false;
                    }
                },
                { "VOS_OPTION", () =>
                    {
                        VerticalOptionSelecterState = false;
                        controllerHandler.SetControllerState("LAST");
                    }
                },
                { "GAMEPAD_ABORT", () => { gamepadThread.Abort(); }}
            
            };

            eventMapParam = new Dictionary<string, Action<string[]>>()
            {
                { "SET_CONTROLLER_STATE", (controllerInfo) =>
                    {
                        controllerHandler.SetControllerState(controllerInfo[0]);
                    }
                },
                { "BUTTONDIALOG_OPEN", (dialogInfo) =>
                    {
                        ButtonDialogType = dialogInfo[0];
                        ButtonDialogState = true;
                    }
                },
                { "GAMEDATA_SEARCH", (searchData) =>
                    {
                        SearchGameDataQuery = searchData[0];
                        SearchGameDataState = true;
                    }
                }
            };

            filterEventMap = new Dictionary<string, Action<string[]>>()
            {
                { "FILTER_FILES", (point) =>
                    {
                        CharGetterPoint = point;
                        CharGetterState = true;
                        controllerHandler.SetControllerState("CHAR_GETTER");
                    }
                },
                { "FILTER_TYPE", (selecterData) =>
                    {
                        VerticalOptionData = selecterData;
                        VerticalOptionSelecterState = true;
                        controllerHandler.SetControllerState("VERTICAL_OPTION_SELECTER");
                    }
                }
            };

            buttonDialogEventMap = new Dictionary<string, Action>()
            {
                { "EXIT", () =>
                    {
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT"));
                    }
                },
                { "GAMEDATA_UPLOAD", () =>
                    {
                        GameRetrieverProgressState = true;
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_UPLOAD_START"));
                    }
                },
                { "FILE_OPEN", () =>
                    {
                        ShellWindowState = WindowState.Minimized;
                        controllerHandler.SetControllerState("ITEM_PLAY");
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("MAXIMIZE_CURR_APP"));
                    }
                },
                { "FILE_DELETE_DATA", () => //Click "Delete Game Data"
                    {
                        controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    }
                },
                { "FILE_SEARCH_DATA", () => //Click "Search Game Data"
                    {
                        controllerHandler.SetControllerState("SEARCH_GAME_DATA");
                    }
                },
                { "RETURN_TO_APP", () => //Click "Return to app"
                    {
                        ShellWindowState = WindowState.Minimized;
                        controllerHandler.SetControllerState("ITEM_PLAY");
                    }
                },
                { "CLOSE_APP", () => //Click "Close App"
                    {
                        ShellWindowState = WindowState.Maximized;
                        controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    }
                }
            };
        }
    }
}
