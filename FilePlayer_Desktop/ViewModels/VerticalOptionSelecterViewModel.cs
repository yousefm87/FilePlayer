using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;
using System.Linq;

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
        private SubscriptionToken optionToken;

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


        public VerticalOptionSelecterViewModel(IEventAggregator iEventAggregator, string[] _options, string[] _responses)
        {
            this.iEventAggregator = iEventAggregator;
            this.VertOptions = _options;
            this.Responses = _responses;
            this.SelectedOptionIndex = 0;

            optionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }






        void PerformViewAction(object sender, ViewEventArgs e)
        {
            switch (e.action)
            {
                case "VOS_MOVE_UP":
                    MoveUp();
                    break;
                case "VOS_MOVE_DOWN":
                    MoveDown();
                    break;
                case "VOS_SELECT":
                    SelectControl();
                    break;

            }

        }


        public void MoveUp()
        {
            if (SelectedOptionIndex > 0)
            {
                SelectedOptionIndex--;
            }
        }

        public void MoveDown()
        {
            bool bottomEdgeSelected = SelectedOptionIndex == (VertOptions.Count() - 1);

            if (!bottomEdgeSelected)
            {
                SelectedOptionIndex++;
            }
        }

        public void SelectControl()
        {

                string response = Responses.ElementAt(SelectedOptionIndex);
                string optionVal = VertOptions.ElementAt(SelectedOptionIndex);

                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("VOS_OPTION", new string[] { optionVal, response }));

                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Unsubscribe(optionToken);
        }

    }
}