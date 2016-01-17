using FilePlayer.Model;
using FilePlayer.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Windows.Controls;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for LetterGetter.xaml
    /// </summary>
    public partial class CharGetter : UserControl
    {
        private IEventAggregator iEventAggregator;
        public SubscriptionToken filterActionToken;
        public Label[] controls;

        public string[] buttonActions;
        public int selectedControlIndex;

        public CharGetter()
        {
            InitializeComponent();
            Init();

            filterActionToken = this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Subscribe(
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

            controls = new Label[] {  char00, char01, char02, char03, char04, char05, char06, char07, char08, char09,
                                        char10, char11, char12, char13, char14, char15, char16, char17, char18, char19,
                                        char20, char21, char22, char23, char24, char25, char26, char27, char28, char29,
                                        char30 };

            buttonActions = new string[] { "FILTER_APPS", "FILTER_FILES", "FILTER_RESET" };

            
            for (int i = 0; i < (controls.Length); i++)
            {
                bool isSelected = (i == selectedControlIndex);
                SetControlSelected(controls[i], isSelected);
            }
        }


        void PerformViewAction(object sender, CharGetterEventArgs e)
        {
            switch (e.action)
            {
                case "CHAR_MOVE_LEFT":
                    MoveLeft();
                    break;
                case "CHAR_MOVE_RIGHT":
                    MoveRight();
                    break;
                case "CHAR_MOVE_UP":
                    MoveUp();
                    break;
                case "CHAR_MOVE_DOWN":
                    MoveDown();
                    break;
                case "CHAR_SELECT":
                    SelectControl();
                    break;
                case "CHAR_BACK":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CHAR_BACK", new string[] { "" }));
                    break;
                case "CHAR_CLOSE":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CHAR_CLOSE", new string[] { "" }));
                    break;
            }

        }

        public void SetControlSelected(Control ctrl, bool isSelected)
        {
            if (isSelected)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    ctrl.SetResourceReference(Control.StyleProperty, "SelectedCharStyle");
                });
            }
            else
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    ctrl.SetResourceReference(Control.StyleProperty, "UnselectedCharStyle");
                    
                });
            }
        }


        public void MoveLeft()
        {
            if (selectedControlIndex != (controls.Length - 1)) //if not bottom label
            {
                int newIndex = selectedControlIndex;
                this.Dispatcher.Invoke((Action)delegate
                {                
                    do
                    {
                        bool leftEdgeSelected = ((newIndex % charGrid.ColumnDefinitions.Count) == 0);

                        if (leftEdgeSelected)
                        {
                            newIndex = newIndex + (charGrid.ColumnDefinitions.Count - 1);
                        }
                        else
                        {
                            newIndex--;
                        }
                    } while (controls[newIndex].Content.ToString().Equals(""));

                });
                SetControlSelected(controls[selectedControlIndex], false);
                SetControlSelected(controls[newIndex], true);
                selectedControlIndex = newIndex;
            }
        }

        public void MoveRight()
        {
            if (selectedControlIndex != (controls.Length - 1)) //if not bottom label
            {

                int newIndex = selectedControlIndex;
                this.Dispatcher.Invoke((Action)delegate
                {
                    do
                    {
                        bool rightEdgeSelected = ((newIndex % charGrid.ColumnDefinitions.Count) == (charGrid.ColumnDefinitions.Count - 1));

                        if (rightEdgeSelected)
                        {
                            newIndex = newIndex - (newIndex % charGrid.ColumnDefinitions.Count);
                        }
                        else
                        {
                            newIndex++;
                        }
                    } while (controls[newIndex].Content.ToString().Equals(""));
                });
                SetControlSelected(controls[selectedControlIndex], false);
                SetControlSelected(controls[newIndex], true);
                selectedControlIndex = newIndex;
            }
        }

        public void MoveUp()
        {
            

            int newIndex = selectedControlIndex;
            this.Dispatcher.Invoke((Action)delegate
            {
                do
                {
                    bool topEdgeSelected = (newIndex < charGrid.ColumnDefinitions.Count);

                    if (topEdgeSelected)
                    {
                        newIndex = controls.Length - 1;
                    }
                    else
                    {
                        newIndex = newIndex - charGrid.ColumnDefinitions.Count;
                    }
                } while (controls[newIndex].Content.ToString().Equals(""));
            });
            SetControlSelected(controls[selectedControlIndex], false);
            SetControlSelected(controls[newIndex], true);
            selectedControlIndex = newIndex;

        }

        public void MoveDown()
        {


            int newIndex = selectedControlIndex;
            this.Dispatcher.Invoke((Action)delegate
            {
                do
                {
                    bool bottomEdgeSelected = (newIndex == (controls.Length - 1));

                    if (bottomEdgeSelected)
                    {
                        newIndex = 0;
                    }
                    else
                    {
                        newIndex += charGrid.ColumnDefinitions.Count;
                        if (newIndex >= (controls.Length - 1))
                        {
                            newIndex = controls.Length - 1;
                        }
                    }
                } while (controls[newIndex].Content.ToString().Equals(""));
            });
            SetControlSelected(controls[selectedControlIndex], false);
            SetControlSelected(controls[newIndex], true);
            selectedControlIndex = newIndex;
        }

        public void SelectControl()
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                string response = controls[selectedControlIndex].Content.ToString();
                if (response == "___")
                {
                    response = " ";
                }
                this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CHAR_SELECT", new string[] { response }));

                //Init();
            });
        }
    }
}
