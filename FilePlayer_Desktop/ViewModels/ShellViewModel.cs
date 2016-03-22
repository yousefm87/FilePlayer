using System;
using Microsoft.Practices.Prism.PubSubEvents;
using Prism.Interactivity.InteractionRequest;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;
using FilePlayer.Model;

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
        private string resultMessage;

        private IEventAggregator iEventAggregator;
        private SubscriptionToken shellActionToken;

        private bool buttonDialogState = false;
        private bool charGetterState = false;
        private bool controllerNotFoundState = false;
        private bool gameRetrieverProgressState = false;
        private bool searchGameDataState = false;
        private bool verticalOptionSelecterState = false;
        private string buttonDialogType;
        private string searchGameDataQuery;

        public string[] VerticalOptionData { get; private set; }
        public string[] CharGetterPoint { get; private set; }

        public bool ButtonDialogState
        {
            get { return buttonDialogState; }
            set
            {
                buttonDialogState = value;
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
                controllerNotFoundState = value;
                OnPropertyChanged("ControllerNotFoundState");
            }
        }

        public bool GameRetrieverProgressState
        {
            get { return gameRetrieverProgressState; }
            set
            {
                gameRetrieverProgressState = value;
                OnPropertyChanged("GameRetrieverProgressState");
            }
        }

        public bool SearchGameDataState
        {
            get { return searchGameDataState; }
            set
            {
                searchGameDataState = value;
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

            shellActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }

        private void PerformViewAction(object sender, ViewEventArgs e)
        {
            switch (e.action)
            {
                case "FILTER_ACTION":
                    switch (e.addlInfo[0])
                    {
                        case "FILTER_FILES": //Selecting file filter
                            CharGetterPoint = e.addlInfo;
                            CharGetterState = true;
                            break;
                        case "FILTER_TYPE": //Selecting filter type
                            VerticalOptionData = e.addlInfo;
                            VerticalOptionSelecterState = true;
                            break;
                    }
                    break;
                case "CHAR_CLOSE": //Close CharGetter
                    CharGetterState = false;
                    break;
                case "CONTROLLER_NOT_FOUND":
                    ControllerNotFoundState = true;
                    break;
                case "CONTROLLER_CONNECTED":
                    ControllerNotFoundState = false;
                    break;
                case "BUTTONDIALOG_OPEN":
                    ButtonDialogType = e.addlInfo[0];
                    ButtonDialogState = true;
                    break;
                case "BUTTONDIALOG_CLOSE":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { "LAST" }));
                    ButtonDialogState = false;
                    break;
                case "BUTTONDIALOG_SELECT":
                    ButtonDialogState = false;
                    switch (e.addlInfo[0])
                    {
                        case "ITEMLIST_PAUSE":
                            switch (e.addlInfo[1])
                            {
                                case "EXIT": //Exit the application
                                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT"));
                                    break;
                                case "GAMEDATA_UPLOAD": //Upload from Giantbomb
                                    GameRetrieverProgressState = true;
                                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_UPLOAD_START", e.addlInfo));
                                    break;
                            }
                            break;
                        case "ITEMLIST_CONFIRMATION":
                            //buttonActions = new string[] { "FILE_OPEN", "FILE_SEARCH_DATA", "FILE_DELETE_DATA" };
                            switch (e.addlInfo[1])
                            {
                                case "FILE_OPEN":
                                    ShellWindowState = WindowState.Minimized;
                                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("OPEN_ITEM", e.addlInfo));
                                    break;
                            }
                            break;
                        case "APP_PAUSE":
                            //buttonActions = new string[] { "RETURN_TO_APP", "CLOSE_APP", "EXIT" };
                            switch (e.addlInfo[1])
                            {
                                case "RETURN_TO_APP": //Click "Return to app"
                                    ShellWindowState = WindowState.Minimized;
                                    break;
                                case "CLOSE_APP": //Click "Close App"
                                    ShellWindowState = WindowState.Maximized;
                                    break;
                                case "EXIT": // Click "Close App + FilePlayer
                                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT"));
                                    break;
                            }
                            break;
                    }
                    break;
                    case "GAMEDATA_SEARCH":
                        SearchGameDataQuery = e.addlInfo[0];
                        SearchGameDataState = true;
                        break;
                    case "SEARCHGAMEDATA_CLOSE":
                        SearchGameDataState = false;
                        break;
                    case "GAMEDATA_SEARCH_ADD":
                        SearchGameDataState = false;
                        break;
                    case "GIANTBOMB_UPLOAD_COMPLETE":
                        GameRetrieverProgressState = false;
                        break;
                    case "VOS_OPTION": //Select an option from VOS
                        VerticalOptionSelecterState = false;
                        break;
                    
            }
        }
    }
}
