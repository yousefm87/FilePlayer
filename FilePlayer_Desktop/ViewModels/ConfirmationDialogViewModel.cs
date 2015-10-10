using Microsoft.Practices.Prism.PubSubEvents;

namespace FilePlayer.ViewModels
{
    public class ConfirmationViewEventArgs : ViewEventArgs
    {
        public ConfirmationViewEventArgs(string action) : base(action) { }
        public ConfirmationViewEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }

    public class ConfirmationDialogViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        
        public ConfirmationDialogViewModel(IEventAggregator iEventAggregator)
        {

            this.iEventAggregator = iEventAggregator;
        }
    }
}