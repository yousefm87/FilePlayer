using System.Windows;
using FilePlayer.ViewModels;
using FilePlayer.Model;
using FilePlayer.Views;
using Microsoft.Practices.Prism.PubSubEvents;

using Prism.Interactivity;
using System;
using Prism.Interactivity.InteractionRequest;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace FilePlayer
{


    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {

        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(int hWnd);



        public ShellViewModel ShellViewModel { get; set; }
        SubscriptionToken viewActionToken;
        private IEventAggregator iEventAggregator;
        PauseDialog pauseDialog = null;

        public Shell()
        {
            ShellViewModel = new ShellViewModel(Event.EventInstance.EventAggregator);
            this.DataContext = ShellViewModel;
            this.iEventAggregator = Event.EventInstance.EventAggregator;

            viewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );

            InitializeComponent();
            this.Topmost = true;
            this.Topmost = false;
        }

        void PerformViewAction(object sender, ItemListViewEventArgs e)
        {
            
            switch (e.action)
            {
                case "CONFIRM_OPEN":
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
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("OPEN_ITEM", e.addlInfo));
                    
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
                            pauseDialog.Close();
                            Application.Current.Shutdown();
                            break;
                    }

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
            if (ShellViewModel.RaisePauseCommand.CanExecute(e))
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
                    // ShellViewModel.RaisePauseCommand.Execute(e);

                    pauseDialog = new PauseDialog();
                    pauseDialog.ShowInTaskbar = false;
                    pauseDialog.Owner = Application.Current.MainWindow;




                    while (!pauseDialog.IsVisible)
                    {
                        pauseDialog.Show();
                        pauseDialog.Left = (Application.Current.MainWindow.ActualWidth - pauseDialog.Width) / 2;
                        pauseDialog.Top = (Application.Current.MainWindow.ActualHeight - pauseDialog.Height) / 2;
                        //pauseDialog.WindowState = WindowState.Normal;
                    }
                });
            }
        }
        
    }



    
}
