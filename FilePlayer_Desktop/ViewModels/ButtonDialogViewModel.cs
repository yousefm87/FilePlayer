using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePlayer.ViewModels
{
    public class ButtonDialogViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        private IEnumerable<string> buttonNames;
        private IEnumerable<string> buttonResponses;
        private int selectedButtonIndex;
        private string dialogName;
        private string closeEvent;
        
        public string CloseEvent
        {
            get { return this.closeEvent; }
            set
            {
                closeEvent = value;
                OnPropertyChanged("CloseEvent");
            }
        }

        public int SelectedButtonIndex
        {
            get { return this.selectedButtonIndex; }
            set
            {
                selectedButtonIndex = value;
                OnPropertyChanged("SelectedButtonIndex");
            }
        }
        public IEnumerable<string> ButtonNames
        {
            get { return this.buttonNames; }
            set
            {
                buttonNames = value;
                OnPropertyChanged("ButtonNames");
            }
        }

        public IEnumerable<string> ButtonResponses
        {
            get { return this.buttonResponses; }
            set
            {
                buttonResponses = value;
                OnPropertyChanged("ButtonResponses");
            }
        }

        public string DialogName
        {
            get { return this.dialogName; }
            set
            {
                dialogName = value;
                OnPropertyChanged("DialogName");
            }
        }

        public ButtonDialogViewModel(IEventAggregator iEventAggregator, string _dialogName, string[] _buttonNames, string[] _buttonResponses, string _closeEvent)
        {
            this.iEventAggregator = iEventAggregator;
            this.DialogName = _dialogName;
            this.ButtonNames = _buttonNames;
            this.ButtonResponses = _buttonResponses;
            this.SelectedButtonIndex = 0;
            this.CloseEvent = _closeEvent;
        }


        public void ReturnController()
        {
            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { CloseEvent }));
        }

    }
}
