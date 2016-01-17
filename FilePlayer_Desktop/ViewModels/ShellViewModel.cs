using System;
using Microsoft.Practices.Prism.PubSubEvents;
using Prism.Interactivity.InteractionRequest;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;

namespace FilePlayer.ViewModels
{
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


    public class PauseCommand : ICommand
    {
        private readonly Action<object> _handler;

        public PauseCommand(Action<object> handler)
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

        private WindowState _shellWindowState = WindowState.Maximized;
        private bool _confirmationPopupIsOpen;
        private string _confirmationText;
        private string resultMessage;
        private IEventAggregator iEventAggregator;

        public InteractionRequest<INotification> ConfirmationRequest { get; private set; }

        public ConfirmationCommand RaiseConfirmationCommand { get; private set; }



        public WindowState ShellWindowState
        {
            get { return _shellWindowState; }
            set
            {
                _shellWindowState = value;
                OnPropertyChanged("ShellWindowState");
            }
        }

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


        public ShellViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;

            ConfirmationPopupIsOpen = false;
            ConfirmationText = "";

            this.ConfirmationRequest = new InteractionRequest<INotification>();
            this.RaiseConfirmationCommand = new ConfirmationCommand(this.RaiseConfirmation);

        }

        
        private void RaiseConfirmation(object viewEventArgs)
        {
            this.InteractionResultMessage = "";

            ViewEventArgs args = (ViewEventArgs)viewEventArgs;
            String contentText = args.addlInfo[0];

            this.ConfirmationRequest.Raise(
                new Notification { Content = contentText, Title = "Confirm" });
        }

    }
}
