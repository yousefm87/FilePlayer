using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePlayer.ViewModels
{
    
    public class CharGetterEventArgs : ViewEventArgs
    {
        public CharGetterEventArgs(string action) : base(action) { }
        public CharGetterEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }

    public class CharGetterViewModel
    {
        private static string[] charSetABC = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                                                    "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                                                    "", "", "U", "V", "W", "X", "Y", "Z", "", "" };
        private static string[] charSetNonABC = new string[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
                                                      "", ".", "?", "!", ":", "-", "#","&", "+", "",
                                                      "", "", "(", ")", "\\", "/", "\"", "'", "", "" };

        private string[][] charSets = new string[][] { charSetABC, charSetNonABC };

        public int CurrCharSetIndex = 0;

        public CharGetterViewModel()
        {

        }

        public string[] GetCurrCharSet()
        {
            return charSets[CurrCharSetIndex];
        }

        public void SwitchCharSetLeft()
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

        public void SwitchCharSetRight()
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


    }
}
