using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private ItemLists itemLists;
        private GameInfo gameInfo;        

        private IEventAggregator iEventAggregator;
        private SubscriptionToken itemListToken = null;
        private SubscriptionToken buttonDialogToken = null;

        private Dictionary<string, Action> buttonDialogEventMap;
        private Dictionary<string, Action> eventMap;
        private Dictionary<string, Action<string[]>> eventMapParams;
        

        private IEnumerable<string> allItemNames;
        private IEnumerable<string> allItemPaths;
        private int selectedItemIndex;
        private string currAppName;
        private string itemImage;
        private string releaseDate;
        private string description;
        private string shortDescription;

        private bool shadeEffect;
        private Stack shadeStack = new Stack();

        private Visibility errorVisibility;
        private Visibility filterVisibility;

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
            get { return errorVisibility; }
            set
            {
                errorVisibility = value;
                OnPropertyChanged("ErrorVisiblility");
            }
        }
        public Visibility FilterVisibility
        {
            get { return filterVisibility; }
            set
            {
                filterVisibility = value;
                OnPropertyChanged("FilterVisibility");
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
            FilterVisibility = Visibility.Hidden;
            ShadeEffect = false;
            FilterText = "";
            FilterTypeText = "";
            this.iEventAggregator = iEventAggregator;
            
            UpdateItemLists();

            InitializeEventMaps(); 

            buttonDialogToken = this.iEventAggregator.GetEvent<PubSubEvent<ButtonDialogEventArgs>>().Subscribe(
                (viewEventArgs) => { ButtonDialogHandler(viewEventArgs); }
            );
            
            itemListToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (controllerEventArgs) => { EventHandler(controllerEventArgs); }
            );
        }

        private void InitializeEventMaps()
        {
            buttonDialogEventMap = new Dictionary<string, Action>()
            {
                { "EXIT", CloseCurrentApplication },
                { "FILE_OPEN", OpenSelectedItemInApp }, //Click "Open File"
                { "FILE_DELETE_DATA", DeleteCurrentGameData }, //Click "Delete Game Data"
                { "FILE_SEARCH_DATA", SendSearchGameDataEvent },
                { "RETURN_TO_APP", MaximizeCurrentApp }, //Click "Return to app"
                { "CLOSE_APP", CloseCurrentApplication }
            };

            eventMap = new Dictionary<string, Action>()
            {
                { "MAXIMIZE_CURR_APP", MaximizeCurrentApp },
                { "ITEMLIST_MOVE_LEFT", SetPreviousLists },
                { "ITEMLIST_MOVE_RIGHT", SetNextLists },
                { "GIANTBOMB_UPLOAD_START", UploadFromGiantbomb },
                { "OPEN_FILTER", OpenFilter },
                { "CLOSE_FILTER", CloseFilter },
                { "ADD_SHADE", () => { SetShade(true); } },
                { "REMOVE_SHADE", () => { SetShade(false); } }
            };

            eventMapParams = new Dictionary<string, Action<string[]>>()
            {
                { "ITEMLIST_MOVE_UP", MoveIndexUp },
                { "ITEMLIST_MOVE_DOWN", MoveIndexDown },
                { "FILTER_LIST", FilterItemlist },
                { "GAMEDATA_ADD_ITEM", AddGameDataItem }
            };

        }

        private void DeleteCurrentGameData()
        {
            string currGame = AllItemNames.ElementAt(SelectedItemIndex);
            GameInfo.DeleteGame(currGame);
            SelectedItemIndex = SelectedItemIndex;
        }

        private void SendSearchGameDataEvent()
        {
            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GAMEDATA_SEARCH", new String[] { AllItemNames.ElementAt(SelectedItemIndex) }));
        }

        private void CloseCurrentApplication()
        {
            if (appProc != null)
            {
                if (!appProc.HasExited)
                {
                    appProc.Kill();
                }
            }
        }

        private void ButtonDialogHandler(ButtonDialogEventArgs e)
        {
            if (buttonDialogEventMap.ContainsKey(e.action))
            {
                buttonDialogEventMap[e.action]();
            }
        }

        public void EventHandler(ViewEventArgs e)
        {
            if (eventMap.ContainsKey(e.action))
            {
                eventMap[e.action]();
            }

            if (eventMapParams.ContainsKey(e.action))
            {
                eventMapParams[e.action](e.addlInfo);
            }
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


        public void AddGameDataItem(string[] gameDataItem)
        {
            GameInfo.AddGame(gameDataItem[0], gameDataItem[1], gameDataItem[2], gameDataItem[3]);
            SelectedItemIndex = SelectedItemIndex;
        }

        public void UploadFromGiantbomb()
        {
            Task.Factory.StartNew(() =>
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { "NONE" }));

                GameRetriever.GetAllPlatformsData(itemLists, iEventAggregator);
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GIANTBOMB_UPLOAD_COMPLETE", new String[] { }));
                
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SET_CONTROLLER_STATE", new string[] { "ITEMLIST_BROWSE" }));
            });
        }

        public void MaximizeCurrentApp()
        {
            WindowActions.PerformWindowAction(this.ItemLists.GetConsoleTitleSubString(ItemLists.CurrConsole), "Maximize");
        }

        public void OpenFilter()
        {
            FilterVisibility = Visibility.Visible;
        }

        public void CloseFilter()
        {
            FilterVisibility = Visibility.Hidden;
        }

        public void FilterItemlist(string[] filterData)
        {

            if (!FilterText.Equals(filterData[0]) || !FilterTypeText.Equals(filterData[1]))
            {
                FilterText = filterData[0];
                FilterTypeText = filterData[1];

                AllItemNames = ItemLists.GetItemNames(ItemLists.CurrConsole, FilterText, FilterTypeText);
                AllItemPaths = ItemLists.GetItemFilePaths(ItemLists.CurrConsole, FilterText, FilterTypeText);

                SelectedItemIndex = 0;
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

        public void SetNextLists()
        {
            SetNextLists(FilterText, FilterTypeText);
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


        public void SetPreviousLists()
        {
            SetPreviousLists(FilterText, FilterTypeText);
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

        public void MoveIndexUp(string[] numMoves)
        {
            int numMove = Int32.Parse(numMoves[0]);

            MoveIndexUp(numMove);
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

        public void MoveIndexDown(string[] numMoves)
        {
            int numMove = Int32.Parse(numMoves[0]);

            MoveIndexDown(numMove);
        }


    }
}
