using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.PubSubEvents;
using Prism.Interactivity.InteractionRequest;
using Prism.Commands;

using System.Windows.Input;

namespace FilePlayer.ViewModels
{

    
    public class ItemSelectionNotification : Confirmation
    {
        public ItemSelectionNotification()
        {
            this.Items = new List<string>();
            this.SelectedItem = null;
        }

        public ItemSelectionNotification(IEnumerable<string> items)
            : this()
        {
            foreach (string item in items)
            {
                this.Items.Add(item);
            }
        }

        public IList<string> Items { get; private set; }

        public string SelectedItem { get; set; }
    }


    public class ViewEventArgs : EventArgs
    {
        public string action;
        public string[] addlInfo;

        public ViewEventArgs(string _action)
        {
            action = _action;
            addlInfo = new string[0]{};
        }

        public ViewEventArgs(string _action, string[] _addlInfo)
        {
            action = _action;
            addlInfo = _addlInfo;
        }
    }

    public class ConfirmationCommand : ICommand
    {
        private readonly Action<object> _handler;

        public ConfirmationCommand(Action<object> handler)
        {
            _handler = handler;
        }


        public bool CanExecute(object parameter)
        {
            return (parameter != null);
        }

        
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _handler(parameter);
        }
    }
    
    public class ShellViewModel : ViewModelBase
    {
        
        private bool _confirmationPopupIsOpen;
        private string _confirmationText;
        public InteractionRequest<INotification> ConfirmationRequest { get; private set; }
        public InteractionRequest<ItemSelectionNotification> ItemSelectionRequest { get; private set; }


        public ConfirmationCommand RaiseConfirmationCommand { get; private set; }
        

        public ICommand RaiseItemSelectionCommand { get; private set; }

        private string resultMessage;

        public bool ConfirmationPopupIsOpen
        {
            get { return _confirmationPopupIsOpen; }
            set
            {
                _confirmationPopupIsOpen = value;
                OnPropertyChanged("ConfirmationPopupIsOpen");
            }
        }

        public string ConfirmationText
        {
            get { return _confirmationText; }
            set
            {
                _confirmationText = value;
                OnPropertyChanged("ConfirmationText");
            }
        }

        public string InteractionResultMessage
        {
            get
            {
                return this.resultMessage;
            }
            set
            {
                this.resultMessage = value;
                this.OnPropertyChanged("InteractionResultMessage");
            }
        }

        private void RaiseConfirmation(object viewEventArgs)
        {
            // In this case we are passing a simple notification as a parameter.
            // The custom popup view we are using for this interaction request does not have a DataContext of its own
            // so it will inherit the DataContext of the window, which will be this same notification.
            this.InteractionResultMessage = "";

            ViewEventArgs args = (ViewEventArgs)viewEventArgs;
            String contentText = args.addlInfo[0];



            this.ConfirmationRequest.Raise(
                new Notification { Content = contentText, Title = "Custom Popup" });
        }

        private IEventAggregator iEventAggregator;

        private SubscriptionToken viewActionToken = null;

        public ShellViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;
            ConfirmationPopupIsOpen = false;
            ConfirmationText = "";
            this.ConfirmationRequest = new InteractionRequest<INotification>();
            this.ItemSelectionRequest = new InteractionRequest<ItemSelectionNotification>();

            this.RaiseConfirmationCommand = new ConfirmationCommand(this.RaiseConfirmation);


        }
    
    }
}
