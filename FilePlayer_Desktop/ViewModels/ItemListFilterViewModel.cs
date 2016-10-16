using FilePlayer.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Windows;

namespace FilePlayer.ViewModels
{
    public class ItemListFilterViewModel : ViewModelBase
    {
        private int numControls;
        private int selectedControlIndex;
        private string filter;
        private string filterType;
        private string[] buttonActions;

        private DelegateCommand MoveLeftCommand { get; set; }
        private DelegateCommand MoveRightCommand { get; set; }
        private DelegateCommand RemoveLastCharFromFilterCommand { get; set; }
        private DelegateCommand ResetFiltersCommand { get; set; }

        private IEventAggregator iEventAggregator;
        private SubscriptionToken filterViewToken = null;
        private Visibility filterVisibility = Visibility.Hidden;

        public Dictionary<string, Action> EventMap { get; private set; }
        public Dictionary<string, Action<string>> EventMapParam { get; private set; }

        public Visibility FilterVisibility
        {
            get { return filterVisibility; }
            set
            {
                filterVisibility = value;
                OnPropertyChanged("FilterVisibility");
            }
        }


        public int SelectedControlIndex
        {
            get { return selectedControlIndex; }
            set
            {
                selectedControlIndex = value;
                OnPropertyChanged("SelectedControlIndex");
            }
        }
        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                OnPropertyChanged("Filter");
                FilterList();
            }
        }
        public string FilterType
        {
            get { return filterType; }
            set
            {
                filterType = value;
                OnPropertyChanged("FilterType");

                FilterList();
            }
        }

        public ItemListFilterViewModel(IEventAggregator iEventAggregator, int _numControls)
        {
            this.iEventAggregator = iEventAggregator;
            buttonActions = new string[] { "FILTER_RESET", "FILTER_TYPE", "FILTER_FILES" };
            Filter = "";
            FilterType = "Contains";
            numControls = _numControls;

            MoveRightCommand = new DelegateCommand(MoveRight, CanMoveRight);
            MoveLeftCommand = new DelegateCommand(MoveLeft, CanMoveLeft);
            RemoveLastCharFromFilterCommand = new DelegateCommand(RemoveLastCharFromFilter, CanRemoveLastCharFromFilter);
            ResetFiltersCommand = new DelegateCommand(ResetFilters, CanResetFilters);


            EventMap = new Dictionary<string, Action>()
            {
                {"FILTER_MOVE_LEFT", () =>
                    {
                        if (MoveLeftCommand.CanExecute())
                        {
                            MoveLeftCommand.Execute();
                        }
                    }
                },
                {"FILTER_MOVE_RIGHT", () =>
                    {
                        if (MoveRightCommand.CanExecute())
                        {
                            MoveRightCommand.Execute();
                        }
                    }
                },
                {"CHAR_BACK", () =>
                    {
                        if (RemoveLastCharFromFilterCommand.CanExecute())
                        {
                            RemoveLastCharFromFilterCommand.Execute();
                        }
                    }
                }

            };

            EventMapParam = new Dictionary<string, Action<string>>()
            {
                {"CHAR_SELECT", AppendToFilter},
                {"VOS_OPTION",  (_filterType) => 
                    {
                        FilterType = _filterType;
                    }
                }
            };


            filterViewToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    EventHandler(viewEventArgs);
                }
            );
            
        }

        void EventHandler(ViewEventArgs e)
        {
            if (EventMapParam.ContainsKey(e.action))
            {
                EventMapParam[e.action](e.addlInfo[0]);
            }

            if (EventMap.ContainsKey(e.action))
            {
                EventMap[e.action]();
            }
        }

        private bool CanMoveLeft()
        {
            return (selectedControlIndex != 0);
        }


        private void MoveLeft()
        {
            SelectedControlIndex--;
        }

        private bool CanMoveRight()
        {
            return (selectedControlIndex != (numControls - 1));
        }

        private void MoveRight()
        {
            SelectedControlIndex++;
        }

        public void AppendToFilter(string appendStr)
        {
            Filter = Filter + appendStr;
        }

        public bool CanRemoveLastCharFromFilter()
        {
            return (Filter.Length > 0);
        }


        public void RemoveLastCharFromFilter()
        {
            Filter = Filter.Substring(0, Filter.Length - 1);
        }       

        public bool CanResetFilters()
        {
            return (Filter != "") && (FilterType != "Contains");
        }

        public void ResetFilters()
        {
            Filter = "";
            FilterType = "Contains";
        }

        public void FilterList()
        {
            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_LIST", new string[] { Filter, FilterType }));
        }
        


    }
}
