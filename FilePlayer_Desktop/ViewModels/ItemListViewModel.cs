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
using Microsoft.Practices.Prism.Commands;


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

        private static string DEFAULT_FILTER_TEXT = "";
        private static string DEFAUTL_FILTER_TYPE = "Contains";

        private const string consolesSamplesURL = "https://github.com/yousefm87/FilePlayer/wiki/Consoles.Json";

        private IEventAggregator iEventAggregator;
        private SubscriptionToken itemListToken = null;
        private SubscriptionToken buttonDialogToken = null;

        private Dictionary<string, Action> buttonDialogEventMap;
        private Dictionary<string, Action> eventMap;
        private Dictionary<string, Action<string[]>> eventMapParams;

        private DelegateCommand<string[]> MoveUpCommand { get; set; }
        private DelegateCommand<string[]> MoveDownCommand { get; set; }
        private DelegateCommand MoveLeftCommand { get; set; }
        private DelegateCommand MoveRightCommand { get; set; }
        private DelegateCommand OpenAppCommand { get; set; }
        private DelegateCommand OpenSampleCommand { get; set; }
        private DelegateCommand OpenDataFolderCommand { get; set; }
        private DelegateCommand SelectItemCommand { get; set; }
        private DelegateCommand ToggleFilterCommand { get; set; }
        private DelegateCommand UploadFromGiantBombCommand { get; set; }

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

        private Visibility errorVisibility = Visibility.Hidden;
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

        
        string consolesStr = "C:\\FPData\\consoles.json";
        string sampleDestStr = "C:\\FPData\\sample.json";

        public ItemListViewModel(IEventAggregator iEventAggregator)
        {
            FilterVisibility = Visibility.Hidden;
            ErrorVisiblility = Visibility.Hidden;
            ShadeEffect = false;
            FilterText = "";
            FilterTypeText = "";
            this.iEventAggregator = iEventAggregator;
            
            InitializeList();

            InitializeEventMaps(); 

            buttonDialogToken = this.iEventAggregator.GetEvent<PubSubEvent<ButtonDialogEventArgs>>().Subscribe(
                (viewEventArgs) => { ButtonDialogHandler(viewEventArgs); }
            );
            
            itemListToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (controllerEventArgs) => { EventHandler(controllerEventArgs); }
            );

            MoveUpCommand = new DelegateCommand<string[]>(MoveIndexUp, CanMoveIndex);
            MoveDownCommand = new DelegateCommand<string[]>(MoveIndexDown, CanMoveIndex);
            MoveLeftCommand = new DelegateCommand(SetPreviousList, CanSetPreviousList);
            MoveRightCommand = new DelegateCommand(SetNextList, CanSetNextList);

            OpenSampleCommand = new DelegateCommand(OpenConsoleSamplePage, CanOpenConsoleSamplePage);
            OpenDataFolderCommand = new DelegateCommand(OpenDataFolder, CanOpenDataFolder);
            SelectItemCommand = new DelegateCommand(SelectItem, CanSelectItem);

            ToggleFilterCommand = new DelegateCommand(ToggleFilter, CanToggleFilter);
            UploadFromGiantBombCommand = new DelegateCommand(UploadFromGiantbomb, CanUploadFromGiantBomb);
        }



        private void InitializeEventMaps()
        {
            eventMap = new Dictionary<string, Action>()
            {
                { "GIANTBOMB_UPLOAD_START", () =>
                    {
                        if (UploadFromGiantBombCommand.CanExecute())
                        {
                            UploadFromGiantBombCommand.Execute();
                        }
                    }
                },
                { "TOGGLE_FILTER", () =>
                    {
                        if (ToggleFilterCommand.CanExecute())
                        {
                            ToggleFilterCommand.Execute();
                        }
                    }
                },
                { "ITEMLIST_MOVE_LEFT", () =>
                    {
                        if (MoveLeftCommand.CanExecute())
                        {
                            MoveLeftCommand.Execute();
                        }
                    }
                },
                { "ITEMLIST_MOVE_RIGHT", () =>
                    {
                        if (MoveRightCommand.CanExecute())
                        {
                            MoveRightCommand.Execute();
                        }
                    }
                },
                { "OPEN_CONSOLE_SAMPLE", () =>
                    {
                        if (OpenSampleCommand.CanExecute())
                        {
                            OpenSampleCommand.Execute();
                        }
                    }
                },
                { "OPEN_DATA_FOLDER", () =>
                    {
                        if (OpenDataFolderCommand.CanExecute())
                        {
                            OpenDataFolderCommand.Execute();
                        }
                    }
                },
                { "ITEMLIST_SELECT", () =>
                    {
                        if (SelectItemCommand.CanExecute())
                        {
                            SelectItemCommand.Execute();
                        }
                    }
                }
            };

            eventMapParams = new Dictionary<string, Action<string[]>>()
            {
                { "ITEMLIST_MOVE_UP", (numMoves) =>
                    {
                        if(MoveUpCommand.CanExecute(numMoves))
                        {
                            MoveUpCommand.Execute(numMoves);
                        }
                    }
                },
                { "ITEMLIST_MOVE_DOWN", (numMoves) =>
                    {
                        if (MoveDownCommand.CanExecute(numMoves))
                        {
                            MoveDownCommand.Execute(numMoves);
                        }
                    }
                },
                {"GIANTBOMB_PLATFORM_UPLOAD_FINISH", (details) =>
                    {
                        InitializeList();
                    }
                },

                { "ADD_SHADE", (windowName) => { SetShade(true, windowName[0]); } },
                { "REMOVE_SHADE", (windowName) => { SetShade(false, windowName[0]); } },
                { "FILTER_LIST", FilterItemlist },
                { "GAMEDATA_ADD_ITEM", AddGameDataItem }

            };

            buttonDialogEventMap = new Dictionary<string, Action>()
            {
                { "FILE_OPEN", () => //Click "Open File"
                    {
                        string appPath = ItemLists.GetConsoleAppPath(ItemLists.CurrConsole);
                        string itemPath = AllItemPaths.ToList().ElementAt(SelectedItemIndex);
                        string consoleArgs = ItemLists.GetConsoleArguments(ItemLists.CurrConsole);
                        string appCaption = ItemLists.GetConsoleTitleSubString(ItemLists.CurrConsole);

                        string[] appData = {appPath, itemPath, consoleArgs, appCaption};

                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("OPEN_APP", appData));
                    }
                }, 
                { "FILE_DELETE_DATA", DeleteCurrentGameData }, //Click "Delete Game Data"
                { "FILE_SEARCH_DATA", SendSearchGameDataEvent },
                { "UPDATE_ITEMLISTS", () =>
                    {
                        InitializeList();
                    }
                }
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


        private void ButtonDialogHandler(ButtonDialogEventArgs e)
        {
            if (buttonDialogEventMap.ContainsKey(e.action))
            {
                buttonDialogEventMap[e.action]();
            }
        }

        private void EventHandler(ViewEventArgs e)
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

        private void GenerateSampleJson()
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

        private void InitializeList()
        {
            this.ItemLists = new ItemLists(consolesStr);

            if (ItemLists.GetConsoleCount() > 0)
            {
                RefreshList();
                                                
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


        private void RefreshList()
        {
            string gameInfoStr = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "gameinfo.json";
            string imgFolder = ItemLists.GetConsoleFilePath(ItemLists.CurrConsole) + "Images\\";

            this.GameInfo = new GameInfo(gameInfoStr, imgFolder);

            this.AllItemNames = this.ItemLists.GetItemNames(ItemLists.CurrConsole, FilterText, FilterTypeText);
            this.AllItemPaths = this.ItemLists.GetItemFilePaths(ItemLists.CurrConsole, FilterText, FilterTypeText);

            this.CurrAppName = this.ItemLists.GetConsoleName(ItemLists.CurrConsole);
        }


        private void AddGameDataItem(string[] gameDataItem)
        {
            GameInfo.AddGame(gameDataItem[0], gameDataItem[1], gameDataItem[2], gameDataItem[3]);
            SelectedItemIndex = SelectedItemIndex;
        }

        private void UploadFromGiantbomb()
        {
            Task.Factory.StartNew(() =>
            {
                this.iEventAggregator.GetEvent<PubSubEvent<StateEventArgs>>().Publish(new StateEventArgs(ApplicationState.None, true));

                GameRetriever.GetAllPlatformsData(itemLists, iEventAggregator);

                this.iEventAggregator.GetEvent<PubSubEvent<StateEventArgs>>().Publish(new StateEventArgs(ApplicationState.None, false));
            });
        }


        private bool CanToggleFilter()
        {
            return (ItemLists.GetConsoleCount() > 0);
        }

        private void ToggleFilter()
        {
            if (FilterVisibility == Visibility.Hidden)
            {
                FilterVisibility = Visibility.Visible;
            }
            else
            {
                FilterVisibility = Visibility.Hidden;
            }
        }

        private void FilterItemlist(string[] filterData)
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

        
        private bool CanSetNextList()
        {
            return (ItemLists.GetConsoleCount() > 0) && (ItemLists.CurrConsole < (ItemLists.GetConsoleCount() - 1));
        }


        private void SetNextList()
        {
            ItemLists.SetConsoleNext();
            RefreshList();
            SelectedItemIndex = 0;
        }


        private bool CanSetPreviousList()
        {
            return (ItemLists.GetConsoleCount() > 0) && (ItemLists.CurrConsole > 0);
        }

        private void SetPreviousList()
        {
            ItemLists.SetConsolePrevious();
            RefreshList();
            SelectedItemIndex = 0;
        }

        private bool CanOpenSelectedItem()
        {
            string appPath = ItemLists.GetConsoleAppPath(ItemLists.CurrConsole);
            string itemPath = AllItemPaths.ToList().ElementAt(SelectedItemIndex);

            return File.Exists(appPath) && File.Exists(itemPath);
        }
        

        private bool CanOpenConsoleSamplePage()
        {
            return (ErrorVisiblility == Visibility.Visible);
        }


        private void OpenConsoleSamplePage()
        {
            Process.Start(consolesSamplesURL);
        }

        private bool CanOpenDataFolder()
        {
            return (ErrorVisiblility == Visibility.Visible) && (File.Exists(consolesStr));
        }

        private void OpenDataFolder()
        {
            Process.Start(Path.GetDirectoryName(consolesStr));

        }


        private void SetShade(bool isShaded, string windowName)
        {
            if (isShaded)
            {
                if (shadeStack.Count == 0)
                {
                    ShadeEffect = true;
                }

                shadeStack.Push(windowName);
            }
            else
            {
                Stack winNames = new Stack();

                string currWinName = (string)shadeStack.Pop();

                while ((shadeStack.Count > 0) && (currWinName != windowName)) //in case windows are closed out of order
                {
                    winNames.Push(currWinName);
                    currWinName = (string)shadeStack.Pop();
                }
               
       
                for (int i = 0; i < winNames.Count; i++) //put remaining back in stack
                {
                    shadeStack.Push(winNames.Pop());
                }
                
                if (shadeStack.Count == 0)
                {
                    ShadeEffect = false;
                }
            }
        }

        private bool CanSelectItem()
        {
            return (AllItemNames.Count() > 0); 
        }

        private void SelectItem()
        {
            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_OPEN", new string[] { "ITEM_LIST_CONFIRMATION_OPEN" }));
        }


        private bool CanMoveIndex(string[] numMoves)
        {
            return (AllItemNames.Count() > 0);
        }

        private int MoveIndexUp(int numMove)
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

        private void MoveIndexUp(string[] numMoves)
        {
            int numMove = Int32.Parse(numMoves[0]);

            MoveIndexUp(numMove);
        }

        private int MoveIndexDown(int numMove)
        {
            int selectedIndex = SelectedItemIndex;

            int newSelectedIndex = selectedIndex + numMove;


            int maxIndex = AllItemNames.Count() - 1;

            if (newSelectedIndex > maxIndex)
            {
                newSelectedIndex = maxIndex;
            }

            SelectedItemIndex = newSelectedIndex;

            return SelectedItemIndex;
        }

        private void MoveIndexDown(string[] numMoves)
        {
            int numMove = Int32.Parse(numMoves[0]);

            MoveIndexDown(numMove);
        }

        private bool CanUploadFromGiantBomb()
        {
            return ItemLists.HasItems();
        }


    }
}
