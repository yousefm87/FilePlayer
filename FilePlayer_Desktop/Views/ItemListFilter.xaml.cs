using FilePlayer.Model;
using FilePlayer.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for ItemListFilter.xaml
    /// </summary>
    public partial class ItemListFilter : UserControl
    {
        private IEventAggregator iEventAggregator;
        public SubscriptionToken filterActionToken;
        public Control[] controls;
        public string[] buttonActions;
        public int selectedControlIndex;


        Brush selectedButtonBackground = Brushes.DodgerBlue;
        Brush selectedButtonForeground = Brushes.White;
        Brush unselectedButtonBackground = Brushes.AliceBlue;
        Brush unselectedButtonForeground = Brushes.Black;


        Inline cursorInline = new Run(" ") { TextDecorations = TextDecorations.Underline };

        public ItemListFilter()
        {
            InitializeComponent();
            Init();

            filterActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    PerformViewAction(this, viewEventArgs);
                }
            );
        }

        public void Init()
        {
            iEventAggregator = Event.EventInstance.EventAggregator;

            selectedControlIndex = 0;
            controls = new Control[] { filterTypeText, fileFilterText, resetBtn };
            buttonActions = new string[] { "FILTER_TYPE", "FILTER_FILES", "FILTER_RESET" };

            fileFilterText.Text = "";

            for (int i = 0; i < controls.Length; i++)
            {
                bool isSelected = (i == selectedControlIndex);
                SetControlSelected(controls[i], isSelected);
            }
        }


        void PerformViewAction(object sender, ViewEventArgs e)
        {
            switch (e.action)
            {
                case "FILTER_MOVE_LEFT":
                    MoveLeft();
                    break;
                case "FILTER_MOVE_RIGHT":
                    MoveRight();
                    break;
                case "FILTER_SELECT_CONTROL":
                    SelectControl();
                    break;
                case "CHAR_SELECT":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        fileFilterText.Text = fileFilterText.Text + e.addlInfo[0];
                    });
                    break;
                case "CHAR_BACK":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        if (fileFilterText.Text.Length > 0)
                        {
                            fileFilterText.Text = fileFilterText.Text.Substring(0, fileFilterText.Text.Length - 1);
                        }
                    });
                    break;
                case "CHAR_CLOSE":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_LIST", new string[] { }));
                    break;
                case "FILTER_ACTION":
                    switch (e.addlInfo[0])
                    {
                        case "FILTER_RESET":
                            this.Dispatcher.Invoke((Action)delegate
                            {
                                fileFilterText.Text = "";
                                filterTypeText.Text = "Contains";

                                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_LIST", new string[] { }));
                            });
                            break;
                    }
                    break;
                case "VOS_OPTION":
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        filterTypeText.Text = e.addlInfo[1];
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_LIST", new string[] { }));
                    });


                    break;
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

        public void MoveLeft()
        {
            if (selectedControlIndex != 0)
            {
                SetControlSelected(controls[selectedControlIndex--], false);
                SetControlSelected(controls[selectedControlIndex], true);
            }
        }

        public void MoveRight()
        {
            if (selectedControlIndex != (controls.Length - 1))
            {
                SetControlSelected(controls[selectedControlIndex++], false);
                SetControlSelected(controls[selectedControlIndex], true);
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
                string[] response = (string[]) responseList.ToArray(typeof(string));
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_ACTION", response));
            });
        }
    }
}
