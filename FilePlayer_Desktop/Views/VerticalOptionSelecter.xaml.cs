using FilePlayer.Model;
using FilePlayer.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Linq;
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

        public VerticalOptionSelecter()
        {
            InitializeComponent();
            VerticalOptionSelecterViewModel = new VerticalOptionSelecterViewModel(Event.EventInstance.EventAggregator);


            this.DataContext = VerticalOptionSelecterViewModel;
            this.iEventAggregator = Event.EventInstance.EventAggregator;

            Init(new String[] { }, new String[] { });

            optionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }

        public VerticalOptionSelecter(string[] _labels, string[] _actions)
        {
            InitializeComponent();
            iEventAggregator = Event.EventInstance.EventAggregator;
            VerticalOptionSelecterViewModel = new VerticalOptionSelecterViewModel(Event.EventInstance.EventAggregator);

            
            this.DataContext = VerticalOptionSelecterViewModel;
            this.iEventAggregator = Event.EventInstance.EventAggregator;

            Init(_labels, _actions);

            optionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }

        public void Init(string[] _labels, string[] _actions)
        {
            
            VerticalOptionSelecterViewModel.VertOptions = _labels;
            VerticalOptionSelecterViewModel.Responses = _actions;

            

            if(_labels.Length == _actions.Length)
            {

                /*
                for (int i = 0; i < (controls.Length); i++)
                {
                    bool isSelected = (i == selectedControlIndex);
                    SetControlSelected(controls[i], isSelected);
                }*/
            }


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
            if(VerticalOptionSelecterViewModel.SelectedOptionIndex > 0)
            {
                VerticalOptionSelecterViewModel.SelectedOptionIndex--;
            }
        }

        public void MoveDown()
        {
            bool bottomEdgeSelected = VerticalOptionSelecterViewModel.SelectedOptionIndex == (VerticalOptionSelecterViewModel.VertOptions.Count() - 1);

            if(!bottomEdgeSelected)
            {
                VerticalOptionSelecterViewModel.SelectedOptionIndex++;
            }
        }

        public void SelectControl()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                string response = VerticalOptionSelecterViewModel.Responses.ElementAt(VerticalOptionSelecterViewModel.SelectedOptionIndex);
                string optionVal = VerticalOptionSelecterViewModel.VertOptions.ElementAt(VerticalOptionSelecterViewModel.SelectedOptionIndex);

                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("VOS_OPTION", new string[] { response, optionVal}));

                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Unsubscribe(optionToken);
            });
        }
    }
}
