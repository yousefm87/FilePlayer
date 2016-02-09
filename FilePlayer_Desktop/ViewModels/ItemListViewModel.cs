using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using Prism.Mvvm;
using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        private SubscriptionToken itemListToken = null;
        private IEnumerable<string> allItemNames;
        private IEnumerable<string> allItemPaths;
        private string currAppName;

        public ItemLists ItemLists
        {
            get { return this.itemLists; }
            set
            {
                itemLists = value;
                OnPropertyChanged("itemLists");
            }
        }

        public string CurrAppName
        {
            get { return this.currAppName; }
            set
            {
                currAppName = value;
                OnPropertyChanged("currAppName");
            }
        }

        public IEnumerable<string> AllItemNames
        {
            get { return this.allItemNames; }
            set
            {
                allItemNames = value;
                OnPropertyChanged("allItemNames");
            }
        }


        public IEnumerable<string> AllItemPaths
        {
            get { return this.allItemPaths; }
            set
            {
                allItemPaths = value;
                OnPropertyChanged("allItemPaths");
            }
        }

        Process autProc = null;
        Process appProc = null;

        public ItemListViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;

            String consolesStr = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\JSON\\consoles.json";
            //String consolesStr = "C:\\FPJSON\\consoles.json";

            this.ItemLists = new ItemLists(consolesStr);

            this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole);
            this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole);

            this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);
            this.SelectedItemIndex = 0;
            
            input = new XboxControllerInputProvider(Event.EventInstance.EventAggregator);
            
            gamepadThread = new Thread(new ThreadStart(input.PollGamepad));
            gamepadThread.Start();

            SetControllerState("ITEMLIST_BROWSE");
            itemListToken = this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Subscribe(
                (controllerEventArgs) =>
                {
                    PerformAction(controllerEventArgs);
                }
            );

        }

        public void PerformAction(ItemListViewEventArgs e)
        {
            switch (e.action)
            {
                case "PAUSE_OPEN":
                    //MinimizeProcess(appProc);
                    WindowActions.PerformWindowAction(this.ItemLists.GetConsoleTitleSubString(ItemLists.CurrConsole), "Minimize");
                    break;
                case "PAUSE_CLOSE":
                    switch (e.addlInfo[0])
                    {
                        case "RETURN_TO_APP":
                            //MaximizeProcess(appProc);
                            
                            WindowActions.PerformWindowAction(this.ItemLists.GetConsoleTitleSubString(ItemLists.CurrConsole), "Maximize");
//                            ShowProcess(appProc);
                            SetControllerState("ITEM_PLAY");
                            break;
                        case "CLOSE_APP":
                            if(!appProc.HasExited)
                                appProc.Kill();
                            SetControllerState("ITEMLIST_BROWSE");
                            break;
                        case "CLOSE_ALL":
                            if(!appProc.HasExited)
                                appProc.Kill();
                            
                            break;
                    }
                    break;
                case "ITEMLIST_MOVE_LEFT":
                    this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);

                    break;
                case "ITEMLIST_MOVE_RIGHT":
                    this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);

                    break;

            }
        }

        public void SetNextLists()
        {
            if (ItemLists.SetConsoleNext())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole);
                SelectedItemIndex = 0;
            }
       }

        public void SetPreviousLists()
        {
            if (ItemLists.SetConsolePrevious())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole);
                SelectedItemIndex = 0;
            }
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
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionItemPlaying(controllerEventArgs);
                        }
                    );
                    break;
                case "ITEM_PAUSE":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionPauseDialog(controllerEventArgs);
                        }
                    );
                    break;
            }
            
        }

        void ControllerButtonPressToActionItemListView(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    string itemName = ItemLists.GetItemNames(ItemLists.CurrConsole).ToList().ElementAt(SelectedItemIndex);
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("CONFIRM_OPEN", new string[]{ itemName }));
                    SetControllerState("ITEMLIST_CONFIRM");
                    break;
                case "B":
                    Console.WriteLine("Case 2");
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_UP", new string[] { 1.ToString() }));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_DOWN", new string[] { 1.ToString() }));
                    break;
                case "DLEFT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_LEFT", new string[] { 1.ToString() }));
                    break;
                case "DRIGHT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_RIGHT", new string[] { 1.ToString() }));
                    break;
                case "LSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_UP", new string[] { 10.ToString() }));
                    break;
                case "RSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_DOWN", new string[] { 10.ToString() }));
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
                    break;
                case "B":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    break;
                case "X":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    break;
                case "Y":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
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


        void ControllerButtonPressToActionPauseDialog(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_SELECT_BUTTON"));
                    break;
                case "B":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_SELECT_BUTTON"));
                    break;
                case "X":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_SELECT_BUTTON"));
                    break;
                case "Y":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_SELECT_BUTTON"));
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_MOVE_UP"));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_MOVE_DOWN"));
                    break;
                case "DLEFT":
                    break;
                case "DRIGHT":
                    break;
                case "LSHOULDER":
                    break;
                case "RSHOULDER":
                    break;
                case "GUIDE":

                    break;
            }
        }

        void ControllerButtonPressToActionItemPlaying(ControllerEventArgs e)
        { 
            switch (e.buttonPressed)
            {
                case "GUIDE":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("PAUSE_OPEN", new string[] { "" }));
                    SetControllerState("ITEM_PAUSE");
                    break;
            }
        }


        public void OpenSelectedItemInApp()
        {
            string autPath = "C:\\Program Files (x86)\\AutoIt3\\AutoIt3.exe";
            string maximizeActionPath_old = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Automation\\maximize.au3";
            string maximizeActionPath = ItemLists.GetConsoleMaxAndFocus(ItemLists.CurrConsole);
            string appPath = ItemLists.GetConsoleAppPath(ItemLists.CurrConsole);
            string itemPath = AllItemPaths.ToList().ElementAt(SelectedItemIndex);
            string consoleArgs = ItemLists.GetConsoleArguments(ItemLists.CurrConsole);


            appProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    Arguments = consoleArgs + " \"" + itemPath + "\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                }
                
            };

            appProc.Start();
            string windowTitle = appProc.MainWindowTitle;
            appProc.WaitForInputIdle();


            if (maximizeActionPath != "")
            {
                autProc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = autPath,
                        Arguments = "\"" + maximizeActionPath + "\" \"" + windowTitle + "\"",
                        UseShellExecute = true,
                        CreateNoWindow = false
                    }
                };

                autProc.Start();
            }

            
            
        }


    }
}
