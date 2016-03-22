using System.Windows;
using System.Linq;
using FilePlayer.ViewModels;
using FilePlayer.Model;
using FilePlayer.Views;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Windows.Controls;
using MahApps;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;

namespace FilePlayer
{


    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {
        public ShellViewModel ShellViewModel { get; set; }
        SubscriptionToken viewActionToken;
        private IEventAggregator iEventAggregator;
        public Dictionary<string, Action> PropertyChangedMap;

        ButtonDialog buttonDialog = null;
        VerticalOptionSelecter verticalOptionSelecter = null;
        GameRetrieverProgress gameRetrieverProgress = null;
        SearchGameData searchGameData = null;
        ControllerNotFound controllerNotFound = null;


        public Shell()
        {
            this.iEventAggregator = Event.EventInstance.EventAggregator;

            InitializeComponent();
            this.Topmost = true;
            this.Topmost = false;

            ShellViewModel = new ShellViewModel(Event.EventInstance.EventAggregator);
            this.DataContext = ShellViewModel;

            SetPropertyChangedMap();
            ShellViewModel.PropertyChanged += PropertyChangedHandler;

        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChangedMap.ContainsKey(e.PropertyName))
            {
                PropertyChangedMap[e.PropertyName]();
            }
        }

        private void SetPropertyChangedMap()
        {
            PropertyChangedMap = new Dictionary<string, Action>()
            {
                { "CharGetterState", () => {
                    if (ShellViewModel.CharGetterState)
                        OpenCharGetter();
                    else
                        CloseCharGetter();
                }},
                { "ControllerNotFoundState", () => {
                    if (ShellViewModel.ControllerNotFoundState)
                        OpenControllerNotFound();
                    else
                        CloseControllerNotFound();
                }},
                { "ButtonDialogState", () => {
                    if (ShellViewModel.ButtonDialogState)
                        OpenButtonDialog();
                    else
                        CloseButtonDialog();
                }},
                { "GameRetrieverProgressState", () => {
                    if (ShellViewModel.GameRetrieverProgressState)
                        OpenGameRetrieverProgress();
                    else
                        CloseGameRetrieverProgress();
                }},
                { "SearchGameDataState", () => {
                    if (ShellViewModel.SearchGameDataState)
                        OpenSearchGameData();
                    else
                        CloseSearchGameData();
                }},
                { "VerticalOptionSelecterState", () => {
                    if (ShellViewModel.VerticalOptionSelecterState)
                        OpenVerticalOptionSelecter();
                    else
                        CloseVerticalOptionSelecter();
                }}
            };
        }
        
        
        private void OpenCharGetter()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                Canvas.SetLeft(charGetter, Double.Parse(ShellViewModel.CharGetterPoint[1]));
                Canvas.SetTop(charGetter, Double.Parse(ShellViewModel.CharGetterPoint[2]));

                charGetter.Visibility = Visibility.Visible;
            });
        }

        private void CloseCharGetter()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                charGetter.Visibility = Visibility.Hidden;
            });
        }
        

        private void OpenSearchGameData()
        {
            if (searchGameData == null)
            {
                MaximizeShell();

                this.Dispatcher.Invoke((Action)delegate
                {
                    searchGameData = new SearchGameData(ShellViewModel.SearchGameDataQuery);
                    searchGameData.ShowInTaskbar = false;
                    searchGameData.Owner = Application.Current.MainWindow;

                    while (!searchGameData.IsVisible)
                    {
                        searchGameData.Show();
                        searchGameData.MaxHeight = Application.Current.MainWindow.ActualHeight - 100;
                        searchGameData.MaxWidth = Application.Current.MainWindow.ActualWidth - 120;
                        searchGameData.Left = (Application.Current.MainWindow.ActualWidth - searchGameData.Width) / 2;
                        searchGameData.Top = (Application.Current.MainWindow.ActualHeight - searchGameData.Height) / 2;
                    }
                });
            }
        }

        private void CloseSearchGameData()
        {
            if (searchGameData != null)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    searchGameData.Close();
                });

                searchGameData = null;
            }
        }



        private void OpenControllerNotFound()
        {
            if (controllerNotFound == null)
            {
                MaximizeShell();

                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { "NONE" }));
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONTROLLER_NOTFOUND_OPEN", new string[] { })); //for shade

                this.Dispatcher.Invoke((Action)delegate
                {
                    this.Activate();
                    controllerNotFound = new ControllerNotFound();
                    controllerNotFound.ShowInTaskbar = false;
                    controllerNotFound.Owner = Application.Current.MainWindow;

                    while (!controllerNotFound.IsVisible)
                    {
                        controllerNotFound.Show();
                        controllerNotFound.MaxHeight = Application.Current.MainWindow.ActualHeight - 100;
                        controllerNotFound.MaxWidth = Application.Current.MainWindow.ActualWidth - 120;
                        controllerNotFound.Left = (Application.Current.MainWindow.ActualWidth - controllerNotFound.Width) / 2;
                        controllerNotFound.Top = (Application.Current.MainWindow.ActualHeight - controllerNotFound.Height) / 2;
                    }
                });
            }
        }

        private void CloseControllerNotFound()
        {
            if (controllerNotFound != null)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    controllerNotFound.Close();
                });

                controllerNotFound = null;

                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONTROLLER_NOTFOUND_CLOSE", new string[] { }));
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { "LAST" }));
            }
        }
            
        
        private void OpenButtonDialog()
        {
            if (buttonDialog == null)
            {
                MaximizeShell();
                

                this.Dispatcher.Invoke((Action)delegate
                {
                    this.Activate();

                    buttonDialog = new ButtonDialog(ShellViewModel.ButtonDialogType);

                    buttonDialog.ShowInTaskbar = false;
                    buttonDialog.Owner = Application.Current.MainWindow;

                    while (!buttonDialog.IsVisible)
                    {
                        buttonDialog.Show();
                        buttonDialog.Left = (Application.Current.MainWindow.ActualWidth - buttonDialog.Width) / 2;
                        buttonDialog.Top = (Application.Current.MainWindow.ActualHeight - buttonDialog.Height) / 2;
                    }
                });
            }
        }

        private void CloseButtonDialog()
        {
            if (buttonDialog != null)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    buttonDialog.Close();
                });

                buttonDialog = null;
            }
        }
        

        private void OpenVerticalOptionSelecter()
        {
            MaximizeShell();

            int arrLength = (ShellViewModel.VerticalOptionData.Length - 3) / 2;
            string[] options = new string[arrLength];
            string[] actions = new string[arrLength];

            Array.Copy(ShellViewModel.VerticalOptionData, 3, options, 0, arrLength);
            Array.Copy(ShellViewModel.VerticalOptionData, 3 + arrLength, actions, 0, arrLength);
            dynamicCanvas.Dispatcher.Invoke((Action)delegate
            {
                verticalOptionSelecter = new VerticalOptionSelecter(options, actions);
                dynamicCanvas.Children.Add(verticalOptionSelecter);
            
                verticalOptionSelecter.Visibility = Visibility.Visible;
                Canvas.SetLeft(verticalOptionSelecter, Double.Parse(ShellViewModel.VerticalOptionData[1]));
                Canvas.SetTop(verticalOptionSelecter, Double.Parse(ShellViewModel.VerticalOptionData[2]));

            });
        }

        private void CloseVerticalOptionSelecter()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                dynamicCanvas.Children.Remove(verticalOptionSelecter);
                verticalOptionSelecter = null;
            });

        }

        private void OpenGameRetrieverProgress()
        {
            MaximizeShell();

            this.Dispatcher.Invoke((Action)delegate
            {
                gameRetrieverProgress = new GameRetrieverProgress();
                gameRetrieverProgress.ShowInTaskbar = false;
                gameRetrieverProgress.Owner = Application.Current.MainWindow;
                
                while (!gameRetrieverProgress.IsVisible)
                {
                    gameRetrieverProgress.Show();
                    gameRetrieverProgress.Left = (Application.Current.MainWindow.ActualWidth - gameRetrieverProgress.Width) / 2;
                    gameRetrieverProgress.Top = (Application.Current.MainWindow.ActualHeight - gameRetrieverProgress.Height) / 2;
                }
            });
        }


        private void CloseGameRetrieverProgress()
        {
            if (gameRetrieverProgress != null)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    gameRetrieverProgress.Close();
                });

                gameRetrieverProgress = null;
            }
        }


        public bool MaximizeShell()
        {
            bool winMaxed = false;
            while (!winMaxed)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    ShellViewModel.ShellWindowState = WindowState.Maximized;
                    Application.Current.MainWindow.Activate();

                    winMaxed = (ShellViewModel.ShellWindowState == WindowState.Maximized);
                });
            }

            return winMaxed;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
            if (viewActionToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT", new String[] { }));
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Unsubscribe(viewActionToken);
            }
        }

        //void PerformViewAction(object sender, ViewEventArgs e)
        //{

        //    switch (e.action)
        //    {
        //        //case "EXIT": //Exit the application
        //        //    this.Dispatcher.Invoke((Action)delegate
        //        //    {
        //        //        Application.Current.Shutdown();
        //        //    });
        //        //    break;
        //        //case "CONTROLLER_NOT_FOUND":
        //        //    if (controllerNotFound == null)
        //        //    {
        //        //        OpenControllerNotFound(e);
        //        //    }
        //        //    break;
        //        //case "CONTROLLER_CONNECTED":
        //        //    if (controllerNotFound != null)
        //        //    {
        //        //        CloseControllerNotFound();
        //        //        //this.Dispatcher.Invoke((Action)delegate
        //        //        //{
        //        //        //    controllerNotFound.Close();
        //        //        //});
        //        //        //controllerNotFound = null;
        //        //        //this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONTROLLER_NOTFOUND_CLOSE", new string[] { }));
        //        //        //this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { "LAST" }));
        //        //    }
        //        //    break;
        //        //case "BUTTONDIALOG_OPEN":
        //        //    OpenButtonDialog(e);
        //        //    break;
        //        //case "BUTTONDIALOG_CLOSE":
        //        //    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { "LAST" }));
        //        //    CloseButtonDialog();
        //        //    break;
        //        //case "BUTTONDIALOG_SELECT":
        //        //    CloseButtonDialog();
        //        //    switch (e.addlInfo[0])
        //        //    {
        //        //        case "ITEMLIST_PAUSE":
        //        //            switch(e.addlInfo[1])
        //        //            {
        //        //                case "EXIT": //Exit the application
        //        //                    this.Dispatcher.Invoke((Action)delegate
        //        //                    {
        //        //                        Application.Current.Shutdown();
        //        //                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT"));
        //        //                    });
        //        //                    break;
        //        //                case "GAMEDATA_UPLOAD": //Upload from Giantbomb
        //        //                    OpenGameRetrieverProgress(e);
        //        //                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_UPLOAD_START", e.addlInfo));
        //        //                    break;
        //        //            }
        //        //            break;
        //        //        case "ITEMLIST_CONFIRMATION":
        //        //            //buttonActions = new string[] { "FILE_OPEN", "FILE_SEARCH_DATA", "FILE_DELETE_DATA" };
        //        //            switch (e.addlInfo[1])
        //        //            {
        //        //                case "FILE_OPEN":
        //        //                    this.Dispatcher.Invoke((Action)delegate
        //        //                    {
        //        //                        ShellViewModel.ShellWindowState = WindowState.Minimized;
        //        //                    });
        //        //                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("OPEN_ITEM", e.addlInfo));
        //        //                    break;
        //        //            }
        //        //            break;
        //        //        case "APP_PAUSE":
        //        //            //buttonActions = new string[] { "RETURN_TO_APP", "CLOSE_APP", "EXIT" };
        //        //            switch (e.addlInfo[1])
        //        //            {
        //        //                case "RETURN_TO_APP": //Click "Return to app"
        //        //                    ShellViewModel.ShellWindowState = WindowState.Minimized;
        //        //                    break;
        //        //                case "CLOSE_APP": //Click "Close App"
        //        //                    this.Dispatcher.Invoke((Action)delegate
        //        //                    {
        //        //                        ShellViewModel.ShellWindowState = WindowState.Maximized;
        //        //                    });
        //        //                    break;
        //        //                case "EXIT": // Click "Close App + FilePlayer"
        //        //                    this.Dispatcher.Invoke((Action)delegate
        //        //                    {
        //        //                        Application.Current.Shutdown();
        //        //                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT"));
        //        //                    });
        //        //                    break;
        //        //            }
        //        //            break;
        //        //    }
        //        //    break;
        //        //case "FILTER_ACTION": 
        //        //    switch (e.addlInfo[0])
        //        //    {
        //        //        case "FILTER_FILES": //Selecting file filter
        //        //            Canvas.SetLeft(charGetter, Double.Parse(e.addlInfo[1]));
        //        //            Canvas.SetTop(charGetter, Double.Parse(e.addlInfo[2]));

        //        //            charGetter.Visibility = Visibility.Visible;
        //        //            break;
        //        //        case "FILTER_TYPE": //Selecting filter type
        //        //            OpenVerticalOptionSelecter();
        //        //            break;
        //        //    }
        //        //    break;
        //        //case "CHAR_CLOSE": //Close CharGetter
        //        //    this.Dispatcher.Invoke((Action)delegate
        //        //    {
        //        //        charGetter.Visibility = Visibility.Hidden;
        //        //    });
        //        //    break;
        //        //case "VOS_OPTION": //Select an option from VOS
        //        //    this.Dispatcher.Invoke((Action)delegate
        //        //    {
        //        //        dynamicCanvas.Children.Remove(verticalOptionSelecter);
        //        //        verticalOptionSelecter = null;
        //        //    });
        //        //    break;
        //        //case "GIANTBOMB_UPLOAD_COMPLETE":
        //        //    this.Dispatcher.Invoke((Action)delegate
        //        //    {
        //        //        gameRetrieverProgress.Close();
        //        //    });
        //        //    break;
        //        //case "GAMEDATA_SEARCH":
        //        //    OpenSearchGameData(e);
        //        //    break;
        //        //case "SEARCHGAMEDATA_CLOSE":
        //        //    CloseSearchGameData();
        //        //    break;
        //        //case "GAMEDATA_SEARCH_ADD":
        //        //    CloseSearchGameData();
        //        //    break;
        //    }
        //}

    }




}
