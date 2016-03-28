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
        private IEventAggregator iEventAggregator;
        public Dictionary<string, Action> dialogStateMap;

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

            dialogStateMap = new Dictionary<string, Action>()
            {
                { "CharGetterState", SetCharGetterState },
                { "ControllerNotFoundState", SetControllerNotFoundState },
                { "ButtonDialogState", SetButtonDialogState },
                { "GameRetrieverProgressState", SetGameRetrieverProgressState },
                { "SearchGameDataState", SetSearchGameDataState },
                { "VerticalOptionSelecterState", SetVerticalOptionSelecterState }
            };

            ShellViewModel.PropertyChanged += DialogStateChangedHandler;

        }


        private void DialogStateChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (dialogStateMap.ContainsKey(e.PropertyName))
            {
                dialogStateMap[e.PropertyName]();
            }
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


        private void SetCharGetterState()
        {
            if (ShellViewModel.CharGetterState)
                OpenCharGetter();
            else
                CloseCharGetter();
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


        private void SetSearchGameDataState()
        {
            if (ShellViewModel.SearchGameDataState)
                OpenSearchGameData();
            else
                CloseSearchGameData();
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


        private void SetControllerNotFoundState()
        {
            if (ShellViewModel.ControllerNotFoundState)
                OpenControllerNotFound();
            else
                CloseControllerNotFound();
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


        private void SetButtonDialogState()
        {
            if (ShellViewModel.ButtonDialogState)
                OpenButtonDialog();
            else
                CloseButtonDialog();
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


        private void SetVerticalOptionSelecterState()
        {
            if (ShellViewModel.VerticalOptionSelecterState)
                OpenVerticalOptionSelecter();
            else
                CloseVerticalOptionSelecter();
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
        

        private void SetGameRetrieverProgressState()
        {
            if (ShellViewModel.GameRetrieverProgressState)
                OpenGameRetrieverProgress();
            else
                CloseGameRetrieverProgress();
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
            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("EXIT", new String[] { }));
        }
        
    }




}
