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
using System.Threading.Tasks;

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

                if (AllItemNames.Count() > 0)
                {
                    ItemImage = GameInfo.GetGameImageLocation(AllItemNames.ElementAt(SelectedItemIndex));
                    ReleaseDate = GameInfo.GetGameReleaseDate(AllItemNames.ElementAt(SelectedItemIndex));
                    Description = GameInfo.GetGameDescription(AllItemNames.ElementAt(SelectedItemIndex));
                    ShortDescription = GameInfo.GetGameShortDescription(AllItemNames.ElementAt(SelectedItemIndex));
                }

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

                if (AllItemNames.Count() == 0)
                {
                    ItemImage = "";
                    ReleaseDate = "";
                    Description = "";
                    ShortDescription = "";
                }
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

            string imgFolder = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "Images\\";

            this.GameInfo = new GameInfo(gameInfoStr, imgFolder);

                        
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
                case "SET_CONTROLLER_STATE":
                    controllerHandler.SetControllerState(e.addlInfo[0]);
                    if (e.addlInfo[0].Equals("ITEM_PLAY"))
                    {
                        WindowActions.PerformWindowAction(this.ItemLists.GetConsoleTitleSubString(ItemLists.CurrConsole), "Maximize");
                    }
                    break;
                case "ITEMLIST_MOVE_LEFT":
                    this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);
                    break;
                case "ITEMLIST_MOVE_RIGHT":
                    this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);
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
                case "GIANTBOMB_UPLOAD_START":
                    Task.Factory.StartNew(() =>
                    {
                        controllerHandler.SetControllerState("NONE");
                        GameRetriever.GetAllPlatformsData(itemLists, iEventAggregator);
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_UPLOAD_COMPLETE", new String[] { }));
                        controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    });
                    break;
                case "EXIT":
                    gamepadThread.Abort();
                    break;
                case "BUTTONDIALOG_SELECT":
                    switch (e.addlInfo[0])
                    {
                        case "ITEMLIST_PAUSE":
                            switch (e.addlInfo[1])
                            {
                                case "EXIT": //Exit the application
                                    //gamepadThread.Abort();
                                    break;
                                case "ITEMLISTPAUSE_CLOSE":
                                    controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                                    break;
                            }

                            break;
                        case "ITEMLIST_CONFIRMATION":
                            //buttonActions = new string[] { "FILE_OPEN", "FILE_SEARCH_DATA", "FILE_DELETE_DATA" };
                            switch (e.addlInfo[1])
                            {
                                case "FILE_OPEN":
                                    OpenSelectedItemInApp();
                                    controllerHandler.SetControllerState("ITEM_PLAY");
                                    break;
                                case "FILE_DELETE_DATA":
                                    string currGame = AllItemNames.ElementAt(SelectedItemIndex);
                                    GameInfo.DeleteGame(currGame);
                                    controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                                    SelectedItemIndex = SelectedItemIndex;
                                    break;
                                case "FILE_SEARCH_DATA":
                                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GAMEDATA_SEARCH", new String[] { AllItemNames.ElementAt(SelectedItemIndex) }));
                                    controllerHandler.SetControllerState("SEARCH_GAME_DATA");
                                    break;
                            }
                            break;
                        case "APP_PAUSE":
                            //buttonActions = new string[] { "RETURN_TO_APP", "CLOSE_APP", "EXIT" };
                            switch (e.addlInfo[1])
                            {
                                case "RETURN_TO_APP":
                                    WindowActions.PerformWindowAction(this.ItemLists.GetConsoleTitleSubString(ItemLists.CurrConsole), "Maximize");
                                    controllerHandler.SetControllerState("ITEM_PLAY");
                                    break;
                                case "CLOSE_APP":
                                    if (!appProc.HasExited)
                                        appProc.Kill();
                                    controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                                    break;
                                case "EXIT":
                                    if (!appProc.HasExited)
                                    {
                                        appProc.Kill();
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case "SEARCHGAMEDATA_CLOSE":
                    controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    break;
                case "GAMEDATA_SEARCH_ADD":
                    GameInfo.AddGame(e.addlInfo[0], e.addlInfo[1], e.addlInfo[2], e.addlInfo[3]);
                    controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    SelectedItemIndex = SelectedItemIndex;
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

                string gameInfoStr = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "gameinfo.json";

                string imgFolder = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "Images\\";

                this.GameInfo = new GameInfo(gameInfoStr, imgFolder);
            }
        }

        public void SetNextLists(string searchStr)
        {
            if (ItemLists.SetConsoleNext())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole, searchStr);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole, searchStr);
                SelectedItemIndex = 0;

                string gameInfoStr = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "gameinfo.json";
                string imgFolder = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "Images\\";

                this.GameInfo = new GameInfo(gameInfoStr, imgFolder);
            }
       }

        public void SetNextLists(string searchStr, string filterType)
        {
            if (ItemLists.SetConsoleNext())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole, searchStr, filterType);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole, searchStr, filterType);
                SelectedItemIndex = 0;

                string gameInfoStr = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "gameinfo.json";

                string imgFolder = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "Images\\";

                this.GameInfo = new GameInfo(gameInfoStr, imgFolder);
            }
        }

        public void SetPreviousLists()
        {
            if (ItemLists.SetConsolePrevious())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole);
                SelectedItemIndex = 0;

                string gameInfoStr = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "gameinfo.json";

                string imgFolder = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "Images\\";

                this.GameInfo = new GameInfo(gameInfoStr, imgFolder);
            }
        }

        public void SetPreviousLists(string searchStr)
        {
            if (ItemLists.SetConsolePrevious())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole, searchStr);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole, searchStr);
                SelectedItemIndex = 0;

                string gameInfoStr = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "gameinfo.json";

                string imgFolder = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "Images\\";

                this.GameInfo = new GameInfo(gameInfoStr, imgFolder);
            }
        }

        public void SetPreviousLists(string searchStr, string filterType)
        {
            if (ItemLists.SetConsolePrevious())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole, searchStr, filterType);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole, searchStr, filterType);
                SelectedItemIndex = 0;

                string gameInfoStr = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "gameinfo.json";

                string imgFolder = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "Images\\";

                this.GameInfo = new GameInfo(gameInfoStr, imgFolder);
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
