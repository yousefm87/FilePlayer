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

    public class ConfirmationCommand 
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
        private string resultMessage;
        private IEventAggregator iEventAggregator;


        public WindowState ShellWindowState
        {
            get { return _shellWindowState; }
            set
            {
                _shellWindowState = value;
                OnPropertyChanged("ShellWindowState");
            }
        }


        public ShellViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;
        }

        
 

    }
}
