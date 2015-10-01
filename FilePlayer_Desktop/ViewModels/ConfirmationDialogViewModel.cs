using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using Prism.Mvvm;
using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;
using System.Linq;

namespace FilePlayer.ViewModels
{
    class ConfirmationDialogViewModel : ViewModelBase
    {
        private ItemLists itemLists;
        Thread gamepadThread;
        public XboxControllerInputProvider input;
        public int SelectedItemIndex;

        private IEventAggregator iEventAggregator;

        private SubscriptionToken controllerSubToken = null;

        private bool _isYesSelected;

        public Boolean YesSelected
        {
            get { return _isYesSelected; }
            set { _isYesSelected = value; }
        }

        public ConfirmationDialogViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;

            YesSelected = false;


        }
    }
}