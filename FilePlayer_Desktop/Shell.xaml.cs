﻿using System.Windows;
using FilePlayer.ViewModels;
using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;

using Prism.Interactivity;
using System;
using Prism.Interactivity.InteractionRequest;
using System.Drawing;

namespace FilePlayer
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public ShellViewModel ShellViewModel { get; set; }
        SubscriptionToken viewActionToken;
        private IEventAggregator iEventAggregator;

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
                    
                    
                    OpenPauseDialog(e);
                    break;
                case "PAUSE_CLOSE":
                    switch (e.addlInfo[0])
                    {
                        case "RETURN_TO_APP":
                            ShellViewModel.ShellWindowState = WindowState.Minimized;
                            break;
                        case "CLOSE_APP":
                            ShellViewModel.ShellWindowState = WindowState.Maximized;
                            break;
                        case "CLOSE_ALL":
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
                this.Dispatcher.Invoke((Action)delegate
                {
                    ShellViewModel.ShellWindowState = WindowState.Maximized;
                    ShellViewModel.RaisePauseCommand.Execute(e);
                });
            }
        }

    }



    
}
