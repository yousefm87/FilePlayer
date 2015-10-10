using Microsoft.Practices.Prism.PubSubEvents;


namespace FilePlayer.ViewModels
{
    public class PauseViewEventArgs : ViewEventArgs
    {
        public PauseViewEventArgs(string action) : base(action) { }
        public PauseViewEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }

    public class PauseDialogViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;

        public PauseDialogViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;
        }
    }
}