using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FilePlayer.Styles
{
    class Controls
    {
    }

    public class AppPopup : PopupWindowAction
    {

        protected override Window GetWindow(INotification notification)
        {
            Window popupWin = base.GetWindow(notification);
            popupWin.WindowStyle = System.Windows.WindowStyle.None;
            popupWin.AllowsTransparency = true;


            return popupWin;

        }

    }
}
