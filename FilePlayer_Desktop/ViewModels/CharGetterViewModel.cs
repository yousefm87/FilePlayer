using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;

namespace FilePlayer.ViewModels
{
    
    public class CharGetterEventArgs : ViewEventArgs
    {
        public CharGetterEventArgs(string action) : base(action) { }
        public CharGetterEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }

    public class CharGetterViewModel : ViewModelBase
    {
        private IEventAggregator iEventAggregator;
        private SubscriptionToken charGetterActionToken;
        private Dictionary<string, Action> eventMap;

        private string spaceText;
        private static string[] charSetABC = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                                                            "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                                                            "", "", "U", "V", "W", "X", "Y", "Z", "", "" };
        private static string[] charSetNonABC = new string[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
                                                              "", ".", "?", "!", ":", "-", "#","&", "+", "",
                                                              "", "", "(", ")", "\\", "/", "\"", "'", "", "" };
        private string[][] charSets = new string[][] { charSetABC, charSetNonABC };

        private int currCharSetIndex = 0;
        private int selectedControlIndex = 0;

        private int columnCount;
        private int rowCount;
        private int numControls;

        

        public int CurrCharSetIndex
        {
            get { return currCharSetIndex; }
            set
            {
                currCharSetIndex = value;
                OnPropertyChanged("CurrCharSetIndex");
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

        public string SpaceText
        {
            get { return spaceText; }
            set
            {
                spaceText = value;
            }
        }


        public CharGetterViewModel(int _columnCount, int _rowCount, int _numControls, bool _hasSpaceBar)
        {
            CurrCharSetIndex = 0;
            columnCount = _columnCount;
            rowCount = _rowCount;
            numControls = _numControls;
            spaceText = "___";
            iEventAggregator = Event.EventInstance.EventAggregator;


            eventMap = new Dictionary<string, Action>()
            {
                { "CHAR_MOVE_LEFT", MoveLeft },
                { "CHAR_MOVE_RIGHT", MoveRight },
                { "CHAR_MOVE_UP", MoveUp },
                { "CHAR_MOVE_DOWN", MoveDown },
                { "CHAR_SWITCHCHARSET_LEFT", SwitchCharSetPrev },
                { "CHAR_SWITCHCHARSET_RIGHT", SwitchCharSetNext },
                { "CHAR_SELECT", SelectControl },
                { "CHAR_BACK", () =>
                    {
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CHAR_BACK"));
                    }
                },
                { "CHAR_CLOSE", () =>
                    {
                        this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CHAR_CLOSE"));
                    }
                }
            };

            charGetterActionToken = this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Subscribe(
                (viewEventArgs) =>
                {
                    EventHandler(this, viewEventArgs);
                }
            );
        }

        void EventHandler(object sender, CharGetterEventArgs e)
        {
            if (eventMap.ContainsKey(e.action))
            {
                eventMap[e.action]();
            }
        }


        public string[] GetCurrCharSet()
        {
            return charSets[CurrCharSetIndex];
        }

        public void SwitchCharSetPrev()
        {
            if (CurrCharSetIndex > 0)
            {
                CurrCharSetIndex--;
            }
            else
            {
                CurrCharSetIndex = charSets.Length - 1;
            }
        }

        public void SwitchCharSetNext()
        {
            if (CurrCharSetIndex < (charSets.Length - 1))
            {
                CurrCharSetIndex++;
            }
            else
            {
                CurrCharSetIndex = 0;
            }
        }

        public void MoveLeft()
        {
            if (SelectedControlIndex != (numControls - 1)) //if not bottom label
            {
                int newIndex = SelectedControlIndex;

                    do
                    {
                        bool leftEdgeSelected = ((newIndex % columnCount) == 0);

                        if (leftEdgeSelected)
                        {
                            newIndex = newIndex + (columnCount - 1);
                        }
                        else
                        {
                            newIndex--;
                        }
                    } while (charSets[CurrCharSetIndex][newIndex].Equals(""));

                SelectedControlIndex = newIndex;
            }
        }

        public void MoveRight()
        {
            if (SelectedControlIndex != (numControls - 1)) //if not bottom label
            {
                int newIndex = SelectedControlIndex;

                do
                {
                    bool rightEdgeSelected = ((newIndex % columnCount) == (columnCount - 1));

                    if (rightEdgeSelected)
                    {
                        newIndex = newIndex - (newIndex % columnCount);
                    }
                    else
                    {
                        newIndex++;
                    }
                } while (charSets[CurrCharSetIndex][newIndex].Equals(""));

                SelectedControlIndex = newIndex;
            }
        }

        public void MoveUp()
        {
            int newIndex = SelectedControlIndex;
            bool findNextChar = false;

            do
            {
                bool topEdgeSelected = (newIndex < columnCount);

                if (topEdgeSelected)
                {
                    newIndex = numControls - 1;
                    findNextChar = false;
                }
                else
                {
                    newIndex = newIndex - columnCount;
                    findNextChar = charSets[CurrCharSetIndex][newIndex].Equals("");
                }

            } while (findNextChar);
            SelectedControlIndex = newIndex;
        }

        public void MoveDown()
        {
            int newIndex = SelectedControlIndex;
            bool findNextChar = false;

            do
            {
                bool bottomEdgeSelected = (newIndex == (numControls - 1));

                if (bottomEdgeSelected)
                {
                    newIndex = 0;
                }
                else
                {
                    newIndex += columnCount;
                }

                if (newIndex >= (numControls - 1))
                {
                    newIndex = numControls - 1;
                    findNextChar = false;
                }
                else
                {
                    findNextChar = charSets[CurrCharSetIndex][newIndex].Equals("");
                }
            } while (findNextChar);
            SelectedControlIndex = newIndex;
        }

        public void SelectControl()
        {
            string response;

            if (SelectedControlIndex == (numControls - 1))
            {
                response = " ";
            }
            else
            {
                response = charSets[CurrCharSetIndex][SelectedControlIndex];
            }

            this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CHAR_SELECT", new string[] { response }));
        }

    }
}
