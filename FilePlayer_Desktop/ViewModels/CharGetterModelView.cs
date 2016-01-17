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

    class CharGetterModelView
    {

    }
}
