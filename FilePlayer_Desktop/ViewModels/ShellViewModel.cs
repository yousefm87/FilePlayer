using System;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;
using FilePlayer.Model;
using System.Threading;
using Microsoft.Practices.Prism.Commands;

namespace FilePlayer.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {

        private WindowState _shellWindowState = WindowState.Maximized;
        private ExternalAppController extAppController;
        private IEventAggregator iEventAggregator;
        private SubscriptionToken shellActionToken;
        private SubscriptionToken buttonDialogToken;
        private SubscriptionToken filterToken;
        private SubscriptionToken stateToken;

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

        //private ControllerHandler controllerHandler;
        private ControllerHandler controllerHandler;

        public string[] VerticalOptionData { get; private set; }
        public string[] CharGetterPoint { get; private set; }
        

        public bool ButtonDialogState
        {
            get { return buttonDialogState; }
            set
            {
                buttonDialogState = value;
                OnPropertyChanged("ButtonDialogState");

                SendItemListShadeEvent(value, "ButtonDialogState");
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
                    controllerNotFoundState = value;
                    OnPropertyChanged("ControllerNotFoundState");

                    SendItemListShadeEvent(value, "ControllerNotFound");
                }

            }
        }

        public bool GameRetrieverProgressState
        {
            get { return gameRetrieverProgressState; }
            set
            {
                
                gameRetrieverProgressState = value;
                OnPropertyChanged("GameRetrieverProgressState");

                SendItemListShadeEvent(value, "GameRetriever");
            }
        }

        public bool SearchGameDataState
        {
            get { return searchGameDataState; }
            set
            {
                searchGameDataState = value;
                OnPropertyChanged("SearchGameDataState");

                SendItemListShadeEvent(value, "SearchGameData");
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

        public DelegateCommand OpenAppCommand { get; private set; }

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

            stateToken = this.iEventAggregator.GetEvent<PubSubEvent<StateEventArgs>>().Subscribe(
                (stateEventArgs) =>
                {
                    controllerHandler.SetState(stateEventArgs.state, stateEventArgs.isOn);
                }
            );
            

            input = new XboxControllerInputProvider(Event.EventInstance.EventAggregator);

            gamepadThread = new Thread(new ThreadStart(input.PollGamepad));
            gamepadThread.Start();

            controllerHandler = new ControllerHandler(Event.EventInstance.EventAggregator);

            

        }


        private void ButtonDialogHandler(ButtonDialogEventArgs e)
        {
            ButtonDialogState = false;
            controllerHandler.SetState(ApplicationState.ButtonDialog, false);

            if (buttonDialogEventMap.ContainsKey(e.action))
            {
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

        private void SendItemListShadeEvent(bool setShade, string winName)
        {
            if(setShade)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ADD_SHADE", new string[]{ winName }));
            }
            else
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("REMOVE_SHADE", new string[] { winName }));
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
                { "RETURN_TO_APP", () =>
                    {
                        if (extAppController != null)
                        {
                            extAppController.MaximizeCurrentApp();
                        }
                    }
                }, //Click "Return to app"
                { "CHAR_CLOSE", () => //Close CharGetter  
                    {
                        CharGetterState = false;
                        controllerHandler.SetState(ApplicationState.CharGetter, false);
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
                        controllerHandler.SetState(ApplicationState.ButtonDialog, false);
                        ButtonDialogState = false;
                    }
                },
                { "SEARCHGAMEDATA_CLOSE", () =>
                    {
                        SearchGameDataState = false;
                        controllerHandler.SetState(ApplicationState.SearchGameData, false);
                    }
                },
                {"GAMEDATA_ADD_ITEM", () =>
                    {
                        SearchGameDataState = false;
                        controllerHandler.SetState(ApplicationState.SearchGameData, false);
                    }
                },
                { "GIANTBOMB_PLATFORM_UPLOAD_FINISH", () =>
                    {
                        GameRetrieverProgressState = false;
                    }
                },
                { "VOS_OPTION", () =>
                    {
                        VerticalOptionSelecterState = false;
                        controllerHandler.SetState(ApplicationState.VerticalOptionSelecter, false);
                    }
                },
                { "GAMEPAD_ABORT", () => 
                    {
                        gamepadThread.Abort();
                    }
                },
                { "OPEN_FILTER", () => 
                    {
                        controllerHandler.SetState(ApplicationState.FilterMain, true);
                    }
                },
                { "CLOSE_FILTER", () =>
                    {
                        controllerHandler.SetState(ApplicationState.FilterMain, false);
                    }
                },
                { "MINIMIZE_SHELL", () =>
                    {
                        ShellWindowState = WindowState.Minimized;
                    }

                }
            
            };

            eventMapParam = new Dictionary<string, Action<string[]>>()
            {
                { "BUTTONDIALOG_OPEN", (dialogInfo) =>
                    {
                        ButtonDialogType = dialogInfo[0];
                        ButtonDialogState = true;
                        controllerHandler.SetState(ApplicationState.ButtonDialog, true);

                    }
                },
                { "GAMEDATA_SEARCH", (searchData) =>
                    {
                        SearchGameDataQuery = searchData[0];
                        SearchGameDataState = true;
                    }
                },
                { "OPEN_APP", (appData) =>
                    {
                        OpenSelectedItemInApp(appData);
                    }
                }
            };

            filterEventMap = new Dictionary<string, Action<string[]>>()
            {
                { "FILTER_FILES", (point) =>
                    {
                        CharGetterPoint = point;
                        CharGetterState = true;
                        controllerHandler.SetState(ApplicationState.CharGetter, true);
                    }
                },
                { "FILTER_TYPE", (selecterData) =>
                    {
                        VerticalOptionData = selecterData;
                        VerticalOptionSelecterState = true;
                        controllerHandler.SetState(ApplicationState.VerticalOptionSelecter, true);
                    }
                }
            };

            buttonDialogEventMap = new Dictionary<string, Action>()
            {
                { "EXIT", () =>
                    {
                        if (extAppController != null)
                        {
                            extAppController.CloseCurrentApplication();
                        }

                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT"));
                    }
                },
                { "GAMEDATA_UPLOAD", () =>
                    {
                        GameRetrieverProgressState = true;
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_UPLOAD_START"));
                    }
                },
                { "FILE_DELETE_DATA", () => //Click "Delete Game Data"
                    {
                        //controllerHandler.SetState(ApplicationState.ItemlistBrowse);
                    }
                },
                { "FILE_SEARCH_DATA", () => //Click "Search Game Data"
                    {
                        controllerHandler.SetState(ApplicationState.SearchGameData, true);
                    }
                },
                { "RETURN_TO_APP", () => //Click "Return to app"
                    {
                        ShellWindowState = WindowState.Minimized;
                        extAppController.MaximizeCurrentApp();
                        controllerHandler.SetState(ApplicationState.ItemPlay, true);
                    }
                },
                { "CLOSE_APP", () => //Click "Close App"
                    {
                        extAppController.CloseCurrentApplication();
                        ShellWindowState = WindowState.Maximized;
                        controllerHandler.SetState(ApplicationState.ItemPlay, false);
                    }
                },
                { "UPDATE_ITEMLISTS", () =>
                    {
                        //controllerHandler.SetState(ApplicationState.ItemlistBrowse);
                    }
                }
            };
        }

        public void OpenSelectedItemInApp(string[] appData)
        {
            string appPath = appData[0];
            string filePath = appData[1];
            string args = appData[2];
            string caption = appData[3];

            controllerHandler.SetState(ApplicationState.None, true);
            ShellWindowState = WindowState.Minimized;

            extAppController = new ExternalAppController(appPath, filePath, args, caption);
            OpenAppCommand = new DelegateCommand(extAppController.OpenSelectedItemInApp, extAppController.CanOpenSelectedItem);

            if (OpenAppCommand.CanExecute())
            {
                OpenAppCommand.Execute();

                Thread.Sleep(5000);
            }
            controllerHandler.SetState(ApplicationState.None, false);
            controllerHandler.SetState(ApplicationState.ItemPlay, true);


        }
    }
}
