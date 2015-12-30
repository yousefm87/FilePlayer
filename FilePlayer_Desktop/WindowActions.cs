using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FilePlayer
{
    class WindowActions
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, ref SearchData data);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref SearchData data);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);


        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();


        private class SearchData
        {
            // You can put any dicks or Doms in here...
            public string Wndclass;
            public string Title;
            public IntPtr hWnd;
        }

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        private static bool EnumProc(IntPtr hWnd, ref SearchData data)
        {
            // Check classname and title 
            // This is different from FindWindow() in that the code below allows partial matches
            StringBuilder sb = new StringBuilder(1024);
            //GetClassName(hWnd, sb, sb.Capacity);
            //if (sb.ToString().StartsWith(data.Wndclass))
            //{
            sb = new StringBuilder(1024);
            GetWindowText(hWnd, sb, sb.Capacity);
            if (sb.ToString().Contains(data.Title))
            {
                data.hWnd = hWnd;
                return false;    // Found the wnd, halt enumeration
            }
            //}
            return true;
        }

        private static IntPtr SearchForWindow(string wndclass, string title)
        {
            SearchData sd = new SearchData { Wndclass = wndclass, Title = title };
            EnumWindows(new EnumWindowsProc(EnumProc), ref sd);
            return sd.hWnd;
        }

        // public static bool PerformWindowAction(string title, string action)
        //{

        //}

        public static bool PerformWindowAction(string title, string action)
        {


            IntPtr hWnd = SearchForWindow("", title);
            bool foundWin = (hWnd.ToInt32() != 0);

            WindowShowStyle winStyle = WindowShowStyle.ShowMinNoActivate;

            switch (action.ToLower())
            {
                case "maximize":
                    winStyle = WindowShowStyle.ShowMaximized;
                    break;
                case "minimize":
                    winStyle = WindowShowStyle.Hide;
                    break;
                default:
                    action = "";
                    break;
            }


            StringBuilder sb = new StringBuilder(1024);
            GetWindowText(hWnd, sb, sb.Capacity);

            if (foundWin && (action != ""))
            {
                Console.WriteLine("PerformWindowAction: Attempting to '" + action + "' window '" + sb.ToString() + "'.");

                Stopwatch stopwatch;
                long currMilliseconds;
                switch (winStyle)
                {
                    case WindowShowStyle.Maximize:
                        
                        //ShowWindow(hWnd, WindowShowStyle.ShowMaximized);
                        SwitchToThisWindow(hWnd, true);
                        SetForegroundWindow(hWnd);

                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                        currMilliseconds = stopwatch.ElapsedMilliseconds;
                        while (currMilliseconds < 5000 && GetForegroundWindow() != hWnd)
                        {
                            SetForegroundWindow(hWnd);
                            currMilliseconds = stopwatch.ElapsedMilliseconds;
                        }
                        stopwatch.Stop();

                        break;
                    case WindowShowStyle.Minimize:
                        ShowWindow(hWnd, WindowShowStyle.Hide);
                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                        currMilliseconds = stopwatch.ElapsedMilliseconds;
                        while (currMilliseconds < 5000 && GetForegroundWindow() == hWnd)
                        {
                            ShowWindow(hWnd, WindowShowStyle.Hide);
                            currMilliseconds = stopwatch.ElapsedMilliseconds;
                        }
                        stopwatch.Stop();
                        break;
                    default:
                        action = "";
                        break;
                }
            }
            else
            {
                Console.WriteLine("PerformWindowAction: Failed to find to window '" + sb.ToString() + "'.");
            }

            return foundWin;
        }


        /// <summary>Enumeration of the different ways of showing a window using 
        /// ShowWindow</summary>
        private enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window is minimized 
            /// or maximized, the system restores it to its original size and 
            /// position. An application should specify this flag when displaying 
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position. 
            /// This value is similar to "ShowNormal", except the window is not 
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size 
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next 
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is 
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This 
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is 
            /// minimized or maximized, the system restores it to its original size 
            /// and position. An application should specify this flag when restoring 
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
            /// that owns the window is hung. This flag should only be used when 
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }


    }
}
