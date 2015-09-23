using System;
using SharpDX.XInput;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FilePlayer
{

    public class ControllerEventArgs : EventArgs
    {
        public string buttonPressed;
    }

    public class XboxControllerInputProvider
   { 
        [DllImport("xinput1_3.dll", EntryPoint = "#100")]
        static extern int secret_get_gamepad(int playerIndex, out XINPUT_GAMEPAD_SECRET struc);

        public delegate void ControllerEventHandler<ControllerEventArgs>(object sender, ControllerEventArgs e);
        public event ControllerEventHandler<ControllerEventArgs> ControllerButtonPressed;

        Controller[] controllers;
        Controller controller;

        public XINPUT_GAMEPAD_SECRET xgs;

        public struct XINPUT_GAMEPAD_SECRET
        {
            public UInt32 eventCount;
            public ushort wButtons;
            public Byte bLeftTrigger;
            public Byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }


        public XboxControllerInputProvider()
        {
            controllers = new[] { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };
            controller = null;
            FindController(5000);
        }

        public int ButtonStrToHex(string btnName)
        {
            switch(btnName.ToLower())
            {
                case "guide":
                    return 0x0400;
                case "up":
                    return 0x00000001;
                case "down":
                    return 0x00000002;
                case "left":
                    return 0x00000004;
                case "right":
                    return 0x00000008;
                case "start":
                    return 0x00000010;
                case "back":
                    return 0x00000020;
                case "lthumb":
                    return 0x00000040;
                case "rthumb":
                    return 0x00000080;
                case "lshoulder":
                    return 0x0100;
                case "rshoulder":
                    return 0x0200;
                case "a":
                    return 0x1000;
                case "b":
                    return 0x2000;
                case "x":
                    return 0x4000;
                case "y":
                    return 0x8000;
            }
            Console.WriteLine("ButtonStrToHex: button '" + btnName + "' not recognized.");
            return -1;
        }

        public bool IsButtonPressed(int playerIndex, string btn)
        {
            int stat;
            bool value;

            stat = secret_get_gamepad(playerIndex, out xgs);

            if (stat == 0)
            {
                value = ((xgs.wButtons & ButtonStrToHex(btn)) != 0);

                if (value)
                {
                    return true;
                }

            }

            return false;
        }


        public void PollGamepad()
        {
            int WaitTimeAfterClick = 150;
            if(controller == null)
            {
                FindController(-1);
            }

            while (true)
            {
                if (IsButtonPressed(0, "guide"))
                {
                    this.ControllerButtonPressed(this, new ControllerEventArgs { buttonPressed = "GUIDE" });
                    Thread.Sleep(WaitTimeAfterClick);
                }
                if (IsButtonPressed(0, "a"))
                {
                    this.ControllerButtonPressed(this, new ControllerEventArgs { buttonPressed = "A" });
                    Thread.Sleep(WaitTimeAfterClick);
                }
                if (IsButtonPressed(0, "b"))
                {
                    this.ControllerButtonPressed(this, new ControllerEventArgs { buttonPressed = "B" });
                    Thread.Sleep(WaitTimeAfterClick);
                }
                if (IsButtonPressed(0, "up"))
                {
                    this.ControllerButtonPressed(this, new ControllerEventArgs { buttonPressed = "DUP" });
                    Thread.Sleep(WaitTimeAfterClick);
                }
                if (IsButtonPressed(0, "down"))
                {
                    this.ControllerButtonPressed(this, new ControllerEventArgs { buttonPressed = "DDOWN" });
                    Thread.Sleep(WaitTimeAfterClick);
                }
                if (IsButtonPressed(0, "rshoulder"))
                {
                    this.ControllerButtonPressed(this, new ControllerEventArgs { buttonPressed = "RSHOULDER" });
                    Thread.Sleep(WaitTimeAfterClick);
                }
            }
        }

        /// <summary>
        /// Polls list of controller slots. 
        /// </summary>
        /// <param name="timeout"></param>
        public bool FindController(int timeout)
        {
            Stopwatch stopwatch = null;
            if (timeout >= 0)
            { 
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }

            bool lookForController = (timeout >= -1);

            while (lookForController)
            {
                // Get 1st controller available
                foreach (var selectControler in controllers)
                {
                    if (selectControler.IsConnected)
                    {
                        controller = selectControler;
                        return true;
                    }
                }
                if (stopwatch != null)
                {
                    if (stopwatch.ElapsedMilliseconds > timeout)
                    {
                        lookForController = false;
                    }

                }
            }

            return false;
        }
        
    }
}


