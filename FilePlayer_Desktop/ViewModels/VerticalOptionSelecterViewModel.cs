using FilePlayer.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
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

        private DelegateCommand MoveUpCommand { get; set; }
        private DelegateCommand MoveDownCommand { get; set; }
        private DelegateCommand SelectControlCommand { get; set; }

        private Dictionary<string, Action> eventMap;

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

            MoveUpCommand = new DelegateCommand(MoveUp, CanMoveUp);
            MoveDownCommand = new DelegateCommand(MoveDown, CanMoveDown);
            SelectControlCommand = new DelegateCommand(SelectControl, CanSelectControl);

            InitEventMaps();
            optionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    EventHandler(viewEventArgs);
                }
            );
        }

        private void EventHandler(ViewEventArgs e)
        {
            if (eventMap.ContainsKey(e.action))
            {
                eventMap[e.action]();
            }
        }




        void InitEventMaps()
        {
            eventMap = new Dictionary<string, Action>()
            {
                { "VOS_MOVE_UP", () =>
                    {
                        if (MoveUpCommand.CanExecute())
                        {
                            MoveUp();
                        }
                    }
                },
                { "VOS_MOVE_DOWN", () =>
                    {
                        if (MoveDownCommand.CanExecute())
                        {
                            MoveDownCommand.Execute();
                        }
                    }
                },
                { "VOS_SELECT", () =>
                    {
                        if (SelectControlCommand.CanExecute())
                        {
                            SelectControlCommand.Execute();
                        }
                    }
                }
            };

        }


        public void MoveUp()
        {
            SelectedOptionIndex--;
        }

        public bool CanMoveUp()
        {
            return (SelectedOptionIndex > 0);
        }

        public void MoveDown()
        {
            SelectedOptionIndex++;
        }

        public bool CanMoveDown()
        {
            return (SelectedOptionIndex != (VertOptions.Count() - 1));
        }


        public void SelectControl()
        {
            string response = Responses.ElementAt(SelectedOptionIndex);
            string optionVal = VertOptions.ElementAt(SelectedOptionIndex);

            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("VOS_OPTION", new string[] { optionVal, response }));

            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Unsubscribe(optionToken);
        }

        public bool CanSelectControl()
        {
            return VertOptions.Count() > 0;
        }

    }
}