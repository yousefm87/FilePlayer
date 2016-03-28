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


        public string[] buttonActions;
        public Button[] buttons;
        public int selectedButtonIndex;

        public ButtonDialogViewModel ButtonDialogViewModel { get; set; }

        public ControllerNotFound()
        {
            iEventAggregator = Event.EventInstance.EventAggregator;

            InitializeComponent();

            this.Topmost = true;
            this.Topmost = false;

            this.AllowsTransparency = true;
        }
        

        private void Window_Closed(object sender, EventArgs e)
        {
        }
    }


}
