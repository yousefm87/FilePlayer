using FilePlayer.Model;
using FilePlayer.ViewModels;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Linq;
using System.Windows.Controls;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ButtonDialog.xaml
    /// </summary>
    public partial class ButtonDialog : MetroWindow
    {
        private IEventAggregator iEventAggregator;

        public ButtonDialogViewModel ButtonDialogViewModel { get; set; }

        public ButtonDialog(string dialogType)
        {
            iEventAggregator = Event.EventInstance.EventAggregator;

            ButtonDialogViewModel = new ButtonDialogViewModel(Event.EventInstance.EventAggregator, dialogType);
            this.DataContext = ButtonDialogViewModel;

            InitializeComponent();

            this.Topmost = true;
            this.Topmost = false;
        }

        public void OnWindowClosed(object sender, EventArgs e)
        {
            ButtonDialogViewModel.OnWindowClosed(sender, e);
        }
        
    }


}
