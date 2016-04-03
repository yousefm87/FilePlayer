using FilePlayer.Model;
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
            }
        }
        public string FilterType
        {
            get { return filterType; }
            set
            {
                filterType = value;
                OnPropertyChanged("FilterType");

                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_LIST", new string[] { Filter, FilterType }));
            }
        }

        public ItemListFilterViewModel(IEventAggregator iEventAggregator, int _numControls)
        {
            this.iEventAggregator = iEventAggregator;
            buttonActions = new string[] { "FILTER_RESET", "FILTER_TYPE", "FILTER_FILES" };
            Filter = "";
            FilterType = "Contains";
            numControls = _numControls;


            EventMap = new Dictionary<string, Action>()
            {
                {"FILTER_MOVE_LEFT", MoveLeft },
                {"FILTER_MOVE_RIGHT", MoveRight },
                {"CHAR_BACK", FilterRemoveLastChar },
                {"CHAR_CLOSE", () => 
                    {
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_LIST", new string[] { Filter, FilterType }));
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


        public void MoveLeft()
        {
            if (selectedControlIndex != 0)
            {
                SelectedControlIndex--;
            }
        }

        public void MoveRight()
        {
            if (selectedControlIndex != (numControls - 1))
            {
                SelectedControlIndex++;
            }
        }

        public void AppendToFilter(string appendStr)
        {
            Filter = Filter + appendStr;
        }

        public void FilterRemoveLastChar()
        {
            if (Filter.Length > 0)
            {
                Filter = Filter.Substring(0, Filter.Length - 1);
            }
        }       

        public void ResetFilters()
        {
            Filter = "";
            FilterType = "Contains";
        }


    }
}
