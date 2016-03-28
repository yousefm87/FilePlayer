using FilePlayer.Model;
using FilePlayer.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Windows.Controls;

namespace FilePlayer.Views
{
    public partial class VerticalOptionSelecter : UserControl
    {
        private IEventAggregator iEventAggregator;
        public VerticalOptionSelecterViewModel VerticalOptionSelecterViewModel { get; set; }
        public SubscriptionToken optionToken;
        public Label[] controls;
       

        public string[] buttonActions;
        public int selectedControlIndex;


        public VerticalOptionSelecter(string[] _labels, string[] _actions)
        {
            InitializeComponent();
            iEventAggregator = Event.EventInstance.EventAggregator;

            VerticalOptionSelecterViewModel = new VerticalOptionSelecterViewModel(Event.EventInstance.EventAggregator, _labels, _actions);
            this.DataContext = VerticalOptionSelecterViewModel;
        }
    }
}
