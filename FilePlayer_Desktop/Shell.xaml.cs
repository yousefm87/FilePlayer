using System.Windows;
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

            viewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );

            InitializeComponent();

            
        }

        void PerformViewAction(object sender, ViewEventArgs e)
        {
            
            switch (e.action)
            {
                case "CONFIRM_OPEN":
                    OpenConfirmationDialog(e);
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

    }

    public class AppPopup : PopupWindowAction
    {

        protected override Window GetWindow(INotification notification)
        {
            Window popupWin = base.GetWindow(notification); 
            popupWin.WindowStyle = System.Windows.WindowStyle.None;
            popupWin.AllowsTransparency = true;
            return popupWin;

        }

    }
}
