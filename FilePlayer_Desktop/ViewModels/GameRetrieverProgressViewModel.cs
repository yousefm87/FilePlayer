using Microsoft.Practices.Prism.PubSubEvents;


namespace FilePlayer.ViewModels
{
    public class GameRetrieverProgressEventArgs : ViewEventArgs
    {
        public GameRetrieverProgressEventArgs(string action) : base(action) { }
        public GameRetrieverProgressEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }

    public class GameRetrieverProgressViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;

        public GameRetrieverProgressViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;
        }
    }
}