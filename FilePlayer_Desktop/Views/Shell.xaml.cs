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
        PauseDialog pauseDialog = null;
        ButtonDialog buttonDialog = null;

        ItemListPauseView itemlistPauseView = null;
        VerticalOptionSelecter verticalOptionSelecter = null;
        GameRetrieverProgress gameRetrieverProgress = null;
        SearchGameData searchGameData = null;
        ControllerNotFound controllerNotFound = null;

        public Shell()
        {
            ShellViewModel = new ShellViewModel(Event.EventInstance.EventAggregator);
            this.DataContext = ShellViewModel;
            this.iEventAggregator = Event.EventInstance.EventAggregator;

            
            viewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );

            InitializeComponent();
            this.Topmost = true;
            this.Topmost = false;
        }


        void PerformViewAction(object sender, ViewEventArgs e)
        {
            
            switch (e.action)
            {
                case "CONTROLLER_NOT_FOUND":

                    if (controllerNotFound == null)
                    {
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { "NONE" }));

                        this.Dispatcher.Invoke((Action)delegate
                        {
                            this.Activate();
                        });
                        OpenControllerNotFound(e);

                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONTROLLER_NOTFOUND_OPEN", new string[] {}));
                    }
                    break;
                case "CONTROLLER_CONNECTED":
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
                    
                    break;
                case "BUTTONDIALOG_OPEN":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        this.Activate();
                    });

                    OpenButtonDialog(e);
                    break;
                case "BUTTONDIALOG_CLOSE":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        buttonDialog.ButtonDialogViewModel.ReturnController();
                        buttonDialog.Close();
                    });
                    break;
                case "BUTTONDIALOG_SELECT":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        buttonDialog.Close();
                    });

                    switch (e.addlInfo[0])
                    {
                        case "ITEMLIST_PAUSE":
                            switch(e.addlInfo[1])
                            {
                                case "EXIT": //Exit the application
                                    this.Dispatcher.Invoke((Action)delegate
                                    {
                                        Application.Current.Shutdown();
                                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT"));
                                    });
                                    break;
                                case "ITEMLISTPAUSE_CLOSE": //Close Itemlist pause
                                    break;
                                case "GAMEDATA_UPLOAD": //Upload from Giantbomb
                                    OpenGameRetrieverProgress(e);
                                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_UPLOAD_START", e.addlInfo));
                                    break;
                            }
                           
                            break;
                        case "ITEMLIST_CONFIRMATION":
                            //buttonActions = new string[] { "FILE_OPEN", "FILE_SEARCH_DATA", "FILE_DELETE_DATA" };
                            switch (e.addlInfo[1])
                            {
                                case "FILE_OPEN":
                                    this.Dispatcher.Invoke((Action)delegate
                                    {
                                        ShellViewModel.ShellWindowState = WindowState.Minimized;
                                    });
                                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("OPEN_ITEM", e.addlInfo));
                                    break;
                            }
                            break;
                        case "APP_PAUSE":
                            //buttonActions = new string[] { "RETURN_TO_APP", "CLOSE_APP", "EXIT" };
                            switch (e.addlInfo[1])
                            {
                                case "RETURN_TO_APP": //Click "Return to app"
                                    ShellViewModel.ShellWindowState = WindowState.Minimized;
                                    break;
                                case "CLOSE_APP": //Click "Close App"
                                    this.Dispatcher.Invoke((Action)delegate
                                    {
                                        ShellViewModel.ShellWindowState = WindowState.Maximized;
                                    });
                                    break;
                                case "EXIT": // Click "Close App + FilePlayer"
                                    this.Dispatcher.Invoke((Action)delegate
                                    {
                                        Application.Current.Shutdown();
                                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT"));
                                    });
                                    break;
                            }
                            break;
                    }
                    break;
                case "FILTER_ACTION": 
                    switch (e.addlInfo[0])
                    {
                        case "FILTER_FILES": //Selecting file filter
                            Canvas.SetLeft(charGetter, Double.Parse(e.addlInfo[1]));
                            Canvas.SetTop(charGetter, Double.Parse(e.addlInfo[2]));

                            charGetter.Visibility = Visibility.Visible;
                            break;
                        case "FILTER_TYPE": //Selecting filter type
                            OpenVerticalOptionSelecter(e);
                            break;
                    }
                    break;
                case "CHAR_CLOSE": //Close CharGetter
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        charGetter.Visibility = Visibility.Hidden;
                    });
                    break;
                case "VOS_OPTION": //Select an option from VOS
                    verticalOptionSelecter.Visibility = Visibility.Hidden;
                    dynamicCanvas.Children.Remove(verticalOptionSelecter);
                    verticalOptionSelecter = null;
                    break;
                case "GIANTBOMB_UPLOAD_COMPLETE":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        gameRetrieverProgress.Close();
                    });
                    break;
                case "GAMEDATA_SEARCH":
                    OpenSearchGameData(e);
                    break;
                case "SEARCHGAMEDATA_CLOSE":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        searchGameData.Close();
                    });
                    break;
                case "GAMEDATA_SEARCH_ADD":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        searchGameData.Close();
                    });
                    break;
            }
        }
        

        private void OpenPauseDialog(ViewEventArgs e)
        {
            MaximizeShell();

            this.Dispatcher.Invoke((Action)delegate
            {
                pauseDialog = new PauseDialog();
                pauseDialog.ShowInTaskbar = false;
                pauseDialog.Owner = Application.Current.MainWindow;
                    
                while (!pauseDialog.IsVisible)
                {
                    pauseDialog.Show();
                    pauseDialog.Left = (Application.Current.MainWindow.ActualWidth - pauseDialog.Width) / 2;
                    pauseDialog.Top = (Application.Current.MainWindow.ActualHeight - pauseDialog.Height) / 2;
                }
            });
        }

        private void OpenSearchGameData(ViewEventArgs e)
        {
            MaximizeShell();

            this.Dispatcher.Invoke((Action)delegate
            {
                searchGameData = new SearchGameData(e.addlInfo[0]);
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


        private void OpenControllerNotFound(ViewEventArgs e)
        {
            MaximizeShell();

            this.Dispatcher.Invoke((Action)delegate
            {
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


        private void OpenButtonDialog(ViewEventArgs e)
        {
            MaximizeShell();

            this.Dispatcher.Invoke((Action)delegate
            {
                string dialogName = "";
                string closeEvent = "";
                string[] buttonNames = new string[] { };
                string[] buttonActions = new string[] { };

                switch (e.addlInfo[0]) //action will be the type of dialog to open, maybe addlInfo[0]?
                {
                    case "ITEM_LIST_PAUSE_OPEN":
                        dialogName = "ITEMLIST_PAUSE";
                        buttonNames = new string[] { "Exit", "Reload Consoles", "Upload Game Data" };
                        buttonActions = new string[] { "EXIT", "UPDATE_ITEMLISTS", "GAMEDATA_UPLOAD" };
                        closeEvent = "ITEMLIST_BROWSE";
                        break;
                    case "ITEM_LIST_CONFIRMATION_OPEN":
                        dialogName = "ITEMLIST_CONFIRMATION";
                        buttonNames = new string[] { "Open", "Search For Data", "Delete Data" };
                        buttonActions = new string[] { "FILE_OPEN", "FILE_SEARCH_DATA", "FILE_DELETE_DATA" };
                        closeEvent = "ITEMLIST_BROWSE";
                        break;
                    case "APP_PAUSE_OPEN":
                        dialogName = "APP_PAUSE";
                        buttonNames = new string[] { "Return to App", "Close App", "Exit" };
                        buttonActions = new string[] { "RETURN_TO_APP", "CLOSE_APP", "EXIT" };
                        closeEvent = "ITEM_PLAY";
                        break;

                }

                buttonDialog = new ButtonDialog(dialogName, buttonNames, buttonActions, closeEvent);

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



        private void OpenVerticalOptionSelecter(ViewEventArgs e)
        {
            MaximizeShell();

            int arrLength = (e.addlInfo.Length - 3) / 2;
            string[] options = new string[arrLength];
            string[] actions = new string[arrLength];

            Array.Copy(e.addlInfo, 3, options, 0, arrLength);
            Array.Copy(e.addlInfo, 3 + arrLength, actions, 0, arrLength);
            dynamicCanvas.Dispatcher.Invoke((Action)delegate
            {
                verticalOptionSelecter = new VerticalOptionSelecter(options, actions);
                dynamicCanvas.Children.Add(verticalOptionSelecter);
            
                verticalOptionSelecter.Visibility = Visibility.Visible;
                Canvas.SetLeft(verticalOptionSelecter, Double.Parse(e.addlInfo[1]));
                Canvas.SetTop(verticalOptionSelecter, Double.Parse(e.addlInfo[2]));

            });
        }


        private void OpenItemlistPauseView(ViewEventArgs e)
        {
            MaximizeShell();

            this.Dispatcher.Invoke((Action)delegate
            {
                itemlistPauseView = new ItemListPauseView();
                itemlistPauseView.ShowInTaskbar = false;
                itemlistPauseView.Owner = Application.Current.MainWindow;


                while (!itemlistPauseView.IsVisible)
                {
                    itemlistPauseView.Show();
                    itemlistPauseView.Left = (Application.Current.MainWindow.ActualWidth - itemlistPauseView.Width) / 2;
                    itemlistPauseView.Top = (Application.Current.MainWindow.ActualHeight - itemlistPauseView.Height) / 2;
                }
            });
            
        }

        private void OpenGameRetrieverProgress(ViewEventArgs e)
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
                Application.Current.Shutdown();
            }
        }

    }



    
}
