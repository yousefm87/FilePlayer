using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Linq;
using System.Diagnostics;
using GiantBomb.Api;
using GiantBomb.Api.Model;
using System.Net;
using System.Text;
using Newtonsoft.Json;

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
        private GameInfo gameInfo;
        Thread gamepadThread;
        public XboxControllerInputProvider input;
        

        private IEventAggregator iEventAggregator;
        private ControllerHandler controllerHandler;
        
        private SubscriptionToken itemListToken = null;
        private IEnumerable<string> allItemNames;
        private IEnumerable<string> allItemPaths;
        private string currAppName;
        private string itemImage;
        private int selectedItemIndex;
        private string releaseDate;
        private string description;
        private string shortDescription;
        



        public int SelectedItemIndex
        {
            get { return this.selectedItemIndex; }
            set
            {
                selectedItemIndex = value;

                ItemImage = GameInfo.GetGameImageLocation(AllItemNames.ElementAt(SelectedItemIndex));
                ReleaseDate = GameInfo.GetGameReleaseDate(AllItemNames.ElementAt(SelectedItemIndex));
                Description = GameInfo.GetGameDescription(AllItemNames.ElementAt(SelectedItemIndex));
                ShortDescription = GameInfo.GetGameShortDescription(AllItemNames.ElementAt(SelectedItemIndex));

                OnPropertyChanged("SelectedItemIndex");
            }
        }

        public ItemLists ItemLists
        {
            get { return this.itemLists; }
            set
            {
                itemLists = value;
                OnPropertyChanged("itemLists");
            }
        }

        public GameInfo GameInfo
        {
            get { return this.gameInfo; }
            set
            {
                gameInfo = value;
                OnPropertyChanged("GameInfo");
            }
        }

        public string ReleaseDate
        {
            get { return this.releaseDate; }
            set
            {
                if(value != "")
                {
                    releaseDate = Convert.ToDateTime(value).ToString("MMMM d, yyyy");
                }
                else
                {
                    releaseDate = value;
                }
                OnPropertyChanged("ReleaseDate");
            }
        }

        public string Description
        {
            get { return this.description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        public string ShortDescription
        {
            get { return this.shortDescription; }
            set
            {
                shortDescription = value;
                OnPropertyChanged("ShortDescription");
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
                OnPropertyChanged("AllItemNames");
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

        public string ItemImage
        {
            get { return this.itemImage; }
            set
            {
                itemImage = value;
                OnPropertyChanged("ItemImage");
            }
        }
        Process autProc = null;
        Process appProc = null;

        public ItemListViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;

            String consolesStr = System.AppDomain.CurrentDomain.BaseDirectory + "\\JSON\\consoles.json";
                //Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\JSON\\consoles.json";
            this.ItemLists = new ItemLists(consolesStr);

            string gameInfoStr = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "gameinfo.json";

            this.GameInfo = new GameInfo(gameInfoStr);

            
            this.AllItemNames = ItemLists.GetItemNames(ItemLists.CurrConsole);
            this.AllItemPaths = ItemLists.GetItemFilePaths(ItemLists.CurrConsole);

            this.CurrAppName = ItemLists.GetConsoleName(ItemLists.CurrConsole);
            this.SelectedItemIndex = 0;

            //GameRetriever.GetConsoleData(AllItemNames, CurrAppName, ItemLists.GetConsoleFilePath(ItemLists.CurrConsole), true);

            input = new XboxControllerInputProvider(Event.EventInstance.EventAggregator);
            
            gamepadThread = new Thread(new ThreadStart(input.PollGamepad));
            gamepadThread.Start();

            controllerHandler = new ControllerHandler(Event.EventInstance.EventAggregator);

            itemListToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (controllerEventArgs) =>
                {
                    PerformAction(controllerEventArgs);
                }
            );

        }


        public void PerformAction(ViewEventArgs e)
        {
            switch (e.action)
            {
                case "CONFIRM_OPEN":
                    if (allItemNames.Count() > 0)
                    {
                        string currItem = allItemNames.ElementAt(SelectedItemIndex);
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CONFIRM_OPEN_DIALOG", new String[] { currItem }));
                        controllerHandler.SetControllerState("ITEMLIST_CONFIRM");
                    }
                    break;
                case "PAUSE_OPEN":
                    WindowActions.PerformWindowAction(this.ItemLists.GetConsoleTitleSubString(ItemLists.CurrConsole), "Minimize");
                    break;
                case "PAUSE_CLOSE":
                    switch (e.addlInfo[0])
                    {
                        case "RETURN_TO_APP":
                            WindowActions.PerformWindowAction(this.ItemLists.GetConsoleTitleSubString(ItemLists.CurrConsole), "Maximize");
                            controllerHandler.SetControllerState("ITEM_PLAY");
                            break;
                        case "CLOSE_APP":
                            if(!appProc.HasExited)
                                appProc.Kill();
                            controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                            break;
                        case "CLOSE_ALL":
                            if (!appProc.HasExited)
                            {
                                appProc.Kill();
                                gamepadThread.Abort();
                            }
                            break;
                    }
                    break;
                case "ITEMLIST_PAUSE_CLOSE":
                    switch (e.addlInfo[0])
                    {
                        case "EXIT":
                            gamepadThread.Abort();
                            break;
                        case "ITEMLIST_PAUSE_CLOSE":
                            controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                            break;
                    }
                    break;
                case "ITEMLIST_MOVE_LEFT":
                    this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);

                    break;
                case "ITEMLIST_MOVE_RIGHT":
                    this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);

                    break;
                case "OPEN_ITEM":
                    if (e.addlInfo[0] == "YES")
                    {
                        OpenSelectedItemInApp();
                        controllerHandler.SetControllerState("ITEM_PLAY");
                    }
                    else
                    {
                        controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    }
                    break;
                case "FILTER_ACTION":
                    switch (e.addlInfo[0])
                    {
                        case "FILTER_FILES":
                            controllerHandler.SetControllerState("CHAR_GETTER");
                            break;
                        case "FILTER_APPLY":
                            break;
                        case "FILTER_TYPE":
                            controllerHandler.SetControllerState("VERTICAL_OPTION_SELECTER");
                            break;
                    }
                    break;
                case "CHAR_CLOSE":
                    controllerHandler.SetControllerState("FILTER_MAIN");
                    
                    break;
                case "VOS_OPTION":
                    controllerHandler.SetControllerState("FILTER_MAIN");
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

        public void SetNextLists(string searchStr)
        {
            if (ItemLists.SetConsoleNext())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole, searchStr);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole, searchStr);
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

        public void SetPreviousLists(string searchStr)
        {
            if (ItemLists.SetConsolePrevious())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole, searchStr);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole, searchStr);
                SelectedItemIndex = 0;
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


        public void SetItemImage()
        {
            string itemPath = AllItemPaths.ToList().ElementAt(SelectedItemIndex);

        }




    }
}
