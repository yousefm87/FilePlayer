using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using Prism.Mvvm;
using FilePlayer.Model;


namespace FilePlayer.ViewModels
{

    public class ItemListViewEventArgs : ViewEventArgs {}

    public class ItemListViewModel : BindableBase
    {
        private ItemLists itemLists;
        Thread gamepadThread;
        public XboxControllerInputProvider input;
        public int SelectedItemIndex;


        public delegate void ItemListViewEventHandler<ViewEventArgs>(object sender, ViewEventArgs e);
        public event ItemListViewEventHandler<ViewEventArgs> SendAction;



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
                        this.SendAction(this, new ViewEventArgs { action = "CONFIRM_OPEN" });
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
