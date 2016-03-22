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
using System.Windows.Media.Effects;
using System.Collections;
using System.Windows;

namespace FilePlayer.ViewModels
{

    public class ItemListViewEventArgs : ViewEventArgs
    {
        public ItemListViewEventArgs(string action) : base(action) { }
        public ItemListViewEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }


    public class ItemListViewModel : ViewModelBase
    {
        private Stack shadeStack = new Stack();


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
        private bool shadeEffect;
        private Visibility errorVisiblility;
        private Visibility filterVisiblility;
        private string filterText;
        private string filterTypeText;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                OnPropertyChanged("FilterText");
            }
        }

        public string FilterTypeText
        {
            get { return filterTypeText; }
            set
            {
                filterTypeText = value;
                OnPropertyChanged("FilterTypeTextt");
            }
        }

        public Visibility ErrorVisiblility
        {
            get { return errorVisiblility; }
            set
            {
                errorVisiblility = value;
                OnPropertyChanged("ErrorVisiblility");
            }
        }

        public Visibility FilterVisiblility
        {
            get { return filterVisiblility; }
            set
            {
                filterVisiblility = value;
                OnPropertyChanged("FilterVisiblility");
            }
        }

        public bool ShadeEffect
        {
            get { return shadeEffect; }
            set
            {
                shadeEffect = value;
                OnPropertyChanged("ShadeEffect");
            }
        }


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
        string consolesStr = "C:\\FPData\\consoles.json";
        string sampleDestStr = "C:\\FPData\\sample.json";

        public ItemListViewModel(IEventAggregator iEventAggregator)
        {

            FilterVisiblility = Visibility.Hidden;
            ShadeEffect = false;
            FilterText = "";
            FilterTypeText = "";
            this.iEventAggregator = iEventAggregator;
            
            UpdateItemLists();

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

        public void GenerateSampleJson()
        {
            if (!Directory.Exists(Path.GetDirectoryName(sampleDestStr)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(sampleDestStr));
            }

            if (! File.Exists(sampleDestStr))
            {
                string sampleSrcPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\JSON\\sample.json";
                System.IO.File.Copy(sampleSrcPath, sampleDestStr);
            }
        }

        public void UpdateItemLists()
        {
            //GenerateSampleJson();
            this.ItemLists = new ItemLists(consolesStr);

            if (ItemLists.GetConsoleCount() > 0)
            {
                string gameInfoStr = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "gameinfo.json";

                string imgFolder = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "Images\\";

                this.GameInfo = new GameInfo(gameInfoStr, imgFolder);


                this.AllItemNames = ItemLists.GetItemNames(ItemLists.CurrConsole);
                this.AllItemPaths = ItemLists.GetItemFilePaths(ItemLists.CurrConsole);

                this.CurrAppName = ItemLists.GetConsoleName(ItemLists.CurrConsole);
                this.SelectedItemIndex = 0;

                ErrorVisiblility = Visibility.Hidden;
            }
            else
            {
                this.AllItemNames = Enumerable.Empty<string>();
                this.AllItemPaths = Enumerable.Empty<string>();

                this.CurrAppName = "";
                this.SelectedItemIndex = 0;

                ErrorVisiblility = Visibility.Visible;
            }
        }
        public void PerformAction(ViewEventArgs e)
        {
            int numMoves;
            switch (e.action)
            {
                case "ITEMLIST_MOVE_LEFT":
                    SetPreviousLists(FilterText, FilterTypeText);
                    break;
                case "ITEMLIST_MOVE_RIGHT":
                    SetNextLists(FilterText, FilterTypeText);
                    break;

                case "ITEMLIST_MOVE_UP":
                    numMoves = Int32.Parse(e.addlInfo[0]);
                    MoveIndexUp(numMoves);
                    break;
                case "ITEMLIST_MOVE_DOWN":
                    numMoves = Int32.Parse(e.addlInfo[0]);
                    MoveIndexDown(numMoves);
                    break;

                case "SET_CONTROLLER_STATE":
                    controllerHandler.SetControllerState(e.addlInfo[0]);
                    if (e.addlInfo[0].Equals("ITEM_PLAY"))
                    {
                        WindowActions.PerformWindowAction(this.ItemLists.GetConsoleTitleSubString(ItemLists.CurrConsole), "Maximize");
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
                case "GIANTBOMB_UPLOAD_START":
                    SetShade(true);
                    Task.Factory.StartNew(() =>
                    {
                        controllerHandler.SetControllerState("NONE");
                        GameRetriever.GetAllPlatformsData(itemLists, iEventAggregator);
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_UPLOAD_COMPLETE", new String[] { }));
                        controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    });
                    break;
                case "GAMEPAD_ABORT":
                    gamepadThread.Abort();
                    break;
                case "BUTTONDIALOG_SELECT":
                    SetShade(false);

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
                                case "UPDATE_ITEMLIST":
                                    UpdateItemLists();
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
                case "FILTER_LIST":
                    if (!FilterText.Equals(e.addlInfo[0]) || !FilterTypeText.Equals(e.addlInfo[1]))
                    {
                        FilterText = e.addlInfo[0];
                        filterTypeText = e.addlInfo[1];

                        AllItemNames = ItemLists.GetItemNames(ItemLists.CurrConsole, FilterText, FilterTypeText);
                        AllItemPaths = ItemLists.GetItemFilePaths(ItemLists.CurrConsole, FilterText, FilterTypeText);

                        SelectedItemIndex = 0;
                    }
                    break;
                case "OPEN_FILTER":
                    if (ItemLists.GetConsoleCount() > 0)
                    {
                        FilterVisiblility = Visibility.Visible;
                    }
                    break;
                case "CLOSE_FILTER":
                    FilterVisiblility = Visibility.Hidden;
                    break;
                case "SEARCHGAMEDATA_CLOSE":
                    controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    break;
                case "GAMEDATA_SEARCH_ADD":
                    GameInfo.AddGame(e.addlInfo[0], e.addlInfo[1], e.addlInfo[2], e.addlInfo[3]);
                    controllerHandler.SetControllerState("ITEMLIST_BROWSE");
                    SelectedItemIndex = SelectedItemIndex;
                    break;
                case "BUTTONDIALOG_OPEN":
                    SetShade(true);
                    break;
                case "BUTTONDIALOG_CLOSE":
                    SetShade(false);
                    break;
                case "CONTROLLER_NOTFOUND_OPEN":
                    SetShade(true);
                    break;
                case "CONTROLLER_NOTFOUND_CLOSE":
                    SetShade(false);
                    break;
                case "GIANTBOMB_UPLOAD_COMPLETE":
                    SetShade(false);
                    break;
            }
        }


        public void SetNextLists(string searchStr, string filterType)
        {
            if (ItemLists.SetConsoleNext())
            {
                this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole, searchStr, filterType);
                this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole, searchStr, filterType);
                SelectedItemIndex = 0;

                this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);

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

                this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);

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


        public void SetShade(bool isShaded)
        {
            if (isShaded)
            {
                if (shadeStack.Count == 0)
                {
                    ShadeEffect = true;
                }

                shadeStack.Push(0);
            }
            else
            {
                shadeStack.Pop();

                if (shadeStack.Count == 0)
                {
                    ShadeEffect = false;
                }
            }
        }

        public int MoveIndexUp(int numMove)
        {
            int selectedIndex = SelectedItemIndex;
            int newSelectedIndex = selectedIndex - numMove;
            int minIndex = 0;

            if (newSelectedIndex < minIndex)
            {
                newSelectedIndex = minIndex;
            }

            SelectedItemIndex = newSelectedIndex;
            return SelectedItemIndex;
        }

        public int MoveIndexDown(int numMove)
        {
            int selectedIndex = SelectedItemIndex;

            int newSelectedIndex = selectedIndex + numMove;


            int minIndex = 0;

            if (newSelectedIndex < minIndex)
            {
                newSelectedIndex = minIndex;
            }

            SelectedItemIndex = newSelectedIndex;

            return SelectedItemIndex;
        }


    }
}
