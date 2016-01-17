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
        ItemListPauseView itemlistPauseView = null;
        VerticalOptionSelecter verticalOptionSelecter = null;

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
                case "CONFIRM_OPEN_DIALOG":
                    OpenConfirmationDialog(e);
                    break;
                case "CONFIRM_CLOSE":
                    if (e.addlInfo[0] == "YES")
                    {
                        this.Dispatcher.Invoke((Action)delegate
                        {
                            ShellViewModel.ShellWindowState = WindowState.Minimized;
                        });

                    }
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("OPEN_ITEM", e.addlInfo));
                    
                    break;
                case "PAUSE_OPEN":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        this.Activate();
                    });
                    
                    OpenPauseDialog(e);
                    break;
                case "PAUSE_CLOSE":
                    switch (e.addlInfo[0])
                    {
                        case "RETURN_TO_APP":
                            pauseDialog.Close();
                            ShellViewModel.ShellWindowState = WindowState.Minimized;
                            break;
                        case "CLOSE_APP":
                            this.Dispatcher.Invoke((Action)delegate
                            {
                                pauseDialog.Close();
                                ShellViewModel.ShellWindowState = WindowState.Maximized;
                            });
                            break;
                        case "CLOSE_ALL":
                            this.Dispatcher.Invoke((Action)delegate
                            {
                                pauseDialog.Close(); 
                                Application.Current.Shutdown();
                            });

                            break;
                    }

                    break;
                case "ITEMLIST_PAUSE_OPEN":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        this.Activate();
                    });

                    OpenItemlistPauseView(e);
                    break;

                case "ITEMLIST_PAUSE_CLOSE":
                    switch (e.addlInfo[0])
                    {
                        case "EXIT":
                            this.Dispatcher.Invoke((Action)delegate
                            {
                                itemlistPauseView.Close();
                                Application.Current.Shutdown();
                            });
                            break;
                        case "ITEMLIST_PAUSE_CLOSE":
                            this.Dispatcher.Invoke((Action)delegate
                            {
                                itemlistPauseView.Close();
                            });
                            break;
                        case "GET_DATA_FROM_GIANTBOMB":
                            break;
                    }

                    break;
                case "FILTER_ACTION":
                    switch (e.addlInfo[0])
                    {
                        case "FILTER_FILES":
                            Canvas.SetLeft(charGetter, Double.Parse(e.addlInfo[1]));
                            Canvas.SetTop(charGetter, Double.Parse(e.addlInfo[2]));

                            charGetter.Visibility = Visibility.Visible;
                            break;
                        case "FILTER_TYPE":
                            OpenVerticalOptionSelecter(e);

                            break;
                    }
                    break;
                case "CHAR_CLOSE":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        charGetter.Visibility = Visibility.Hidden;
                    });
                    break;
                case "VOS_OPTION":
                    verticalOptionSelecter.Visibility = Visibility.Hidden;
                    dynamicCanvas.Children.Remove(verticalOptionSelecter);
                    break;



            }
        }


        private void OpenConfirmationDialog(ViewEventArgs e)
        {
            if (ShellViewModel.RaiseConfirmationCommand.CanExecute(e))
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    ShellViewModel.RaiseConfirmationCommand.Execute(e);
                });
            }
        }

        private void OpenPauseDialog(ViewEventArgs e)
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

        private void OpenVerticalOptionSelecter(ViewEventArgs e)
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

    }



    
}
