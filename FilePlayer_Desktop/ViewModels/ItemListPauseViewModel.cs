using Microsoft.Practices.Prism.PubSubEvents;


namespace FilePlayer.ViewModels
{
    public class ItemListPauseViewEventArgs : ViewEventArgs
    {
        public ItemListPauseViewEventArgs(string action) : base(action) { }
        public ItemListPauseViewEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }

    public class ItemListPauseViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;

        public ItemListPauseViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;
        }
    }
}