
using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;

namespace FilePlayer.ViewModels
{
    public class VerticalOptionSelecterViewModelEventArgs : ViewEventArgs
    {
        public VerticalOptionSelecterViewModelEventArgs(string action) : base(action) { }
        public VerticalOptionSelecterViewModelEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }
    public class VerticalOptionSelecterViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        private IEnumerable<string> vertOptions;
        private IEnumerable<string> responses;
        private int selectedOptionIndex;

        public int SelectedOptionIndex
        {
            get { return this.selectedOptionIndex; }
            set
            {
                selectedOptionIndex = value;
                OnPropertyChanged("SelectedOptionIndex");
            }
        }
        public IEnumerable<string> VertOptions
        {
            get { return this.vertOptions; }
            set
            {
                vertOptions = value;
                OnPropertyChanged("VertOptions");
            }
        }

        public IEnumerable<string> Responses
        {
            get { return this.responses; }
            set
            {
                responses = value;
                OnPropertyChanged("Responses");
            }
        }


        public VerticalOptionSelecterViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;
            this.VertOptions = new string[] { };
            this.Responses = new string[] { };
            this.SelectedOptionIndex = 0;
        }

    }
}