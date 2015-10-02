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

    public class ItemListViewEventArgs : ViewEventArgs
    {
        public ItemListViewEventArgs(string action) : base(action) { }
        public ItemListViewEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }


    public class ItemListViewModel : ViewModelBase
    {
        private ItemLists itemLists;
        Thread gamepadThread;
        public XboxControllerInputProvider input;
        public int SelectedItemIndex;

        private IEventAggregator iEventAggregator;

        private SubscriptionToken controllerSubToken = null;

        public ItemLists ItemLists
        {
            get { return this.itemLists; }
            set
            {
                itemLists = value;
                OnPropertyChanged("itemLists");
            }
        }

        public IEnumerable<string> AllItems { get; private set; }

        public ItemListViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;

            String consolesStr = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\JSON\\consoles.json";
            this.ItemLists = new ItemLists(consolesStr);

            this.AllItems = this.ItemLists.GetNames();
            this.SelectedItemIndex = 1;
            
            input = new XboxControllerInputProvider(Event.EventInstance.EventAggregator);
            
            gamepadThread = new Thread(new ThreadStart(input.PollGamepad));
            gamepadThread.Start();

            SetControllerState("ITEMLIST_BROWSE");


        }

        public void SetControllerState(string state)
        {
            if (controllerSubToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Unsubscribe(controllerSubToken);
            }
            switch (state)
            {
                case "ITEMLIST_BROWSE":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionItemListView(controllerEventArgs);
                        }
                    );
                    break;
                case "ITEMLIST_CONFIRM":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionConfirmationDialog(controllerEventArgs);
                        }
                    );
                    break;
                case "ITEM_PLAY":
                    break;
                case "ITEM_PAUSE":
                    break;
            }
            
        }
        
        void ControllerButtonPressToActionItemListView(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    string itemName = AllItems.ToList().ElementAt(SelectedItemIndex);
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("CONFIRM_OPEN", new string[]{ itemName }));
                    SetControllerState("ITEMLIST_CONFIRM");
                    break;
                case "B":
                    Console.WriteLine("Case 2");
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_UP"));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_DOWN"));
                    break;
                case "RSHOULDER":
                    Console.WriteLine("Case 2");
                    break;
                case "GUIDE":
                    break;
            }
        }

        void ControllerButtonPressToActionConfirmationDialog(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    SetControllerState("ITEMLIST_BROWSE");
                    break;
                case "B":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    SetControllerState("ITEMLIST_BROWSE");
                    break;
                case "X":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    SetControllerState("ITEMLIST_BROWSE");
                    break;
                case "Y":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    SetControllerState("ITEMLIST_BROWSE");
                    break;
                case "DUP":
                    break;
                case "DDOWN":
                    break;
                case "DLEFT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_MOVE_LEFT"));
                    break;
                case "DRIGHT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_MOVE_RIGHT"));
                    break;
                case "LSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_MOVE_LEFT"));
                    break;
                case "RSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_MOVE_RIGHT"));
                    break;
                case "GUIDE":
                    break;
            }
        }


    }
}
