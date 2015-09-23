using FilePlayer.Model;
using Microsoft.Practices.Prism.Mvvm;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;

namespace FilePlayer.ViewModels
{
    public class ItemListViewEventArgs : EventArgs
    {
        public string action;
    }

    public class ItemListViewModel : BindableBase
    {
        private ItemLists itemLists;
        Thread gamepadThread;
        XboxControllerInputProvider input;
        public int SelectedItemIndex;


        public delegate void ItemListViewEventHandler<ItemListViewEventArgs>(object sender, ItemListViewEventArgs e);
        public event ItemListViewEventHandler<ItemListViewEventArgs> SendAction;



        public ItemLists ItemLists
        {
            get { return this.itemLists; }
            set { SetProperty(ref this.itemLists, value); }
        }

        public IEnumerable<string> AllItems { get; private set; }

        public ItemListViewModel()
        {
            String consolesStr = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\JSON\\consoles.json";
            this.ItemLists = new ItemLists(consolesStr);

            this.AllItems = this.ItemLists.GetNames();
            this.SelectedItemIndex = 1;
            
            input = new XboxControllerInputProvider();
            input.ControllerButtonPressed += ControllerButtonPress;
            gamepadThread = new Thread(new ThreadStart(input.PollGamepad));
            gamepadThread.Start();

        }

        
        
        void ControllerButtonPress(object sender, ControllerEventArgs e)
        {
            
            switch (e.buttonPressed)
            {
                case "A":
                    if (this.SendAction != null)
                        this.SendAction(this, new ItemListViewEventArgs { action = "CONFIRM_OPEN" });
                    break;
                case "B":
                    Console.WriteLine("Case 2");
                    break;
                case "DUP":
                    if(this.SendAction != null)
                        this.SendAction(this, new ItemListViewEventArgs { action = "MOVE_UP" });
                    break;
                case "DDOWN":
                    if(this.SendAction != null)
                        this.SendAction(this, new ItemListViewEventArgs { action = "MOVE_DOWN" });
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
