using FilePlayer.Model;
using FilePlayer.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ItemListFilter.xaml
    /// </summary>
    public partial class ItemListFilter : UserControl
    {
        private IEventAggregator iEventAggregator;
        public SubscriptionToken filterActionToken;
        public Dictionary<string, Action> propertyChangedMap;
        public Dictionary<string, Action> eventMap;
        
        public Control[] controls;
        public string[] buttonActions;
        private int selectedControlIndex = 0;

        public ItemListFilterViewModel ItemListFilterViewModel { get; set; }

        public ItemListFilter()
        {
            InitializeComponent();
            Init();

            ItemListFilterViewModel = new ItemListFilterViewModel(iEventAggregator, controls.Length);
            this.DataContext = ItemListFilterViewModel;

            propertyChangedMap = new Dictionary<string, Action>()
            {
                { "SelectedControlIndex", () => {
                    SetControlSelected(controls[selectedControlIndex], false);
                    selectedControlIndex = ItemListFilterViewModel.SelectedControlIndex;
                    SetControlSelected(controls[selectedControlIndex], true);
                }},

            };

            ItemListFilterViewModel.PropertyChanged += PropertyChangedHandler;

            eventMap = new Dictionary<string, Action>()
            {
                {"FILTER_SELECT_CONTROL", SelectControl }
            };

            filterActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    EventHandler(viewEventArgs);
                }
            );
        }

        void EventHandler(ViewEventArgs e)
        {
            if(eventMap.ContainsKey(e.action))
            {
                eventMap[e.action]();
            }
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (propertyChangedMap.ContainsKey(e.PropertyName))
            {
                propertyChangedMap[e.PropertyName]();
            }
        }

        public void Init()
        {
            iEventAggregator = Event.EventInstance.EventAggregator;

            selectedControlIndex = 0;
            controls = new Control[] { resetBtn, filterTypeText, fileFilterText };
            buttonActions = new string[] { "FILTER_RESET", "FILTER_TYPE", "FILTER_FILES" };

            fileFilterText.Text = "";

            for (int i = 0; i < controls.Length; i++)
            {
                bool isSelected = (i == selectedControlIndex);
                SetControlSelected(controls[i], isSelected);
            }
        }


        public void SetControlSelected(Control ctrl, bool isSelected)
        {
            if (isSelected)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    if (ctrl.Name.Equals("resetBtn"))
                    {
                        ctrl.SetResourceReference(Control.StyleProperty, "SelectedFilterResetBtnStyle");
                    }
                    else
                    {
                        ctrl.SetResourceReference(Control.StyleProperty, "SelectedFilterInputStyle");
                    }
                });
            }
            else
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    if (ctrl.Name.Equals("resetBtn"))
                    {
                        ctrl.SetResourceReference(Control.StyleProperty, "UnselectedFilterResetBtnStyle");
                    }
                    else
                    {
                        ctrl.SetResourceReference(Control.StyleProperty, "UnselectedFilterInputStyle");
                    }
                });
            }
        }
        
        public string GetFilterType()
        {
            string typeText = "";
            this.Dispatcher.Invoke((Action)delegate
            {
                typeText = filterTypeText.Text;
            });
            return typeText;
        }

        public string GetFilterFile()
        {
            string fileText = "";
            this.Dispatcher.Invoke((Action)delegate
            {
                fileText = fileFilterText.Text;
            });
            return fileText;
        }


        public void SelectControl()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                if (buttonActions[selectedControlIndex].Equals("FILTER_RESET"))
                {
                    ItemListFilterViewModel.ResetFilters();
                }
                else
                {
                    ArrayList responseList = new ArrayList();
                    responseList.Add(buttonActions[selectedControlIndex]);
                    double startPointX = -1;
                    double startPointY = -1;

                    if (responseList[0].Equals("FILTER_FILES"))
                    {
                        Point startPoint = new Point(0, fileFilterText.ActualHeight);
                        startPoint = fileFilterText.PointToScreen(startPoint);
                        startPointX = startPoint.X + 10;
                        startPointY = startPoint.Y + 10;

                        responseList.Add(startPointX.ToString());
                        responseList.Add(startPointY.ToString());
                    }
                    if (responseList[0].Equals("FILTER_TYPE"))
                    {
                        Point startPoint = new Point(0, filterTypeText.ActualHeight);
                        startPoint = filterTypeText.PointToScreen(startPoint);
                        startPointX = startPoint.X + 10;
                        startPointY = startPoint.Y + 10;

                        responseList.Add(startPointX.ToString());
                        responseList.Add(startPointY.ToString());
                        responseList.AddRange(new string[] { "Contains", "Starts With", "Ends With" });
                        responseList.AddRange(new string[] { "CONTAINS", "STARTS_WITH", "ENDS_WITH" });
                    }


                    string[] response = (string[])responseList.ToArray(typeof(string));
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListFilterEventArgs>>().Publish(new ItemListFilterEventArgs(response[0], response));
                }
            });
        }
    }
}
