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
                            ControllerButtonPress(controllerEventArgs);
                        }
                    );
                    break;
                case "ITEMLIST_CONFIRM":
                    break;
                case "ITEM_PLAY":
                    break;
                case "ITEM_PAUSE":
                    break;
            }
            
        }
        
        void ControllerButtonPress(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    //if (this.SendAction != null)
                    //this.SendAction(this, new ViewEventArgs { action = "CONFIRM_OPEN" });
                    string itemName = AllItems.ToList().ElementAt(SelectedItemIndex);
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONFIRM_OPEN", new string[]{ itemName }));
                    break;
                case "B":
                    Console.WriteLine("Case 2");
                    break;
                case "DUP":
                    //if(this.SendAction != null)
                        //this.SendAction(this, new ItemListViewEventArgs { action = "MOVE_UP" });
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("MOVE_UP"));
                    break;
                case "DDOWN":
                    //if(this.SendAction != null)
                    //this.SendAction(this, new ItemListViewEventArgs { action = "MOVE_DOWN" });
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("MOVE_DOWN"));
                    break;
                case "RSHOULDER":
                    
                    Console.WriteLine("Case 2");
                    break;
                case "GUIDE":
                    //if(this.SendAction != null)
                        //this.SendAction(this, new ItemListViewEventArgs { action = "MOVEUP" });
                    break;
                //default:
                //    Console.WriteLine("Default case");
                //    break;
            }
        }

        
 

    }
}
