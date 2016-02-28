using System;
using SharpDX.XInput;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FilePlayer
{

    public class ControllerEventArgs : EventArgs
    {
        public string action;

    }

    public class XboxControllerInputProvider
   { 
        [DllImport("xinput1_3.dll", EntryPoint = "#100")]
        static extern int secret_get_gamepad(int playerIndex, out XINPUT_GAMEPAD_SECRET struc);

        public delegate void ControllerEventHandler<ControllerEventArgs>(object sender, ControllerEventArgs e);
        public event ControllerEventHandler<ControllerEventArgs> ControllerButtonPressed;

        private IEventAggregator iEventAggregator;

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


        public XboxControllerInputProvider(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;
            controllers = new[] { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };
            controller = null;
            FindController(1000);
        }

        public int ButtonStrToHex(string btnName)
        {
            switch(btnName.ToLower())
            {
                case "guide": //Guide button
                    return 0x0400;
                case "dup": //D-Pad Up
                    return 0x00000001;
                case "ddown": //D-Pad Down
                    return 0x00000002;
                case "dleft": //D-Pad Left
                    return 0x00000004;
                case "dright": //D-Pad Right
                    return 0x00000008;
                case "start": //Start
                    return 0x00000010;
                case "back": //Back
                    return 0x00000020;
                case "lthumb": //Left Thumbstick
                    return 0x00000040;
                case "rthumb": //Right Thumbstick
                    return 0x00000080;
                case "lshoulder": //Left Shoulder 
                    return 0x0100;
                case "rshoulder": //Right Shoulder
                    return 0x0200;
                case "a": //A
                    return 0x1000;
                case "b": //B
                    return 0x2000;
                case "x": //X
                    return 0x4000;
                case "y": //Y
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


            while (true)
            {
                if (controller == null)
                {
                    FindController(1000);
                }
                else
                {
                    if (controller.IsConnected)
                    {

                        if (IsButtonPressed(0, "GUIDE"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "GUIDE" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "DUP"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "DUP" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "DDOWN"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "DDOWN" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }

                        if (IsButtonPressed(0, "DLEFT"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "DLEFT" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "DRIGHT"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "DRIGHT" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "START"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "START" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "BACK"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "BACK" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "LTHUMB"))
                        {

                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "LTHUMB" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "RTHUMB"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "RTHUMB" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "LSHOULDER"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "LSHOULDER" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "RSHOULDER"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "RSHOULDER" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "A"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "A" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "B"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "B" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "X"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "X" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                        if (IsButtonPressed(0, "Y"))
                        {
                            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "Y" });
                            Thread.Sleep(WaitTimeAfterClick);
                        }
                    }
                    else
                    {
                        controller = null;
                    }
                }
            }
        }

        /// <summary>
        /// Polls list of controller slots. 
        /// </summary>
        /// <param name="timeout">timeout in milliseconds.</param>
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
                        this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "CONTROLLER_CONNECTED" });
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

            this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Publish(new ControllerEventArgs { action = "CONTROLLER_NOT_FOUND" });
            return false;
        }
        
    }
}


