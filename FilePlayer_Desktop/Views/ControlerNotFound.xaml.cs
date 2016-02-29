using FilePlayer.Model;
using FilePlayer.ViewModels;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Windows.Controls;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ControllerNotFound.xaml
    /// </summary>
    public partial class ControllerNotFound : MetroWindow
    {
        private IEventAggregator iEventAggregator;
        public SubscriptionToken dialogActionToken;

        public string[] buttonActions;
        public Button[] buttons;
        public int selectedButtonIndex;

        public ButtonDialogViewModel ButtonDialogViewModel { get; set; }

        public ControllerNotFound()
        {
            iEventAggregator = Event.EventInstance.EventAggregator;

            InitializeComponent();

            this.Topmost = true;

            dialogActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
            this.AllowsTransparency = true;
        }

        void PerformViewAction(object sender, ViewEventArgs e)
        {
        }
        

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Topmost = false;
        }
    }


}
