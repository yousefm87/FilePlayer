﻿using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using Prism.Mvvm;
using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FilePlayer.ViewModels
{

    public class ItemListViewEventArgs : ViewEventArgs
    {
        public ItemListViewEventArgs(string action) : base(action) { }
        public ItemListViewEventArgs(string action, string[] addlInfo) : base(action, addlInfo) { }
    }


    public class ItemListViewModel : ViewModelBase
    {
        private ItemLists itemLists;
        Thread gamepadThread;
        public XboxControllerInputProvider input;
        public int SelectedItemIndex;

        private IEventAggregator iEventAggregator;

        private SubscriptionToken controllerSubToken = null;
        private SubscriptionToken itemListToken = null;

        public ItemLists ItemLists
        {
            get { return this.itemLists; }
            set
            {
                itemLists = value;
                OnPropertyChanged("itemLists");
            }
        }

        public IEnumerable<string> AllItemNames { get; private set; }
        public IEnumerable<string> AllItemPaths { get; private set; }

        Process autProc = null;
        Process appProc = null;

        [DllImport("User32")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);


        public ItemListViewModel(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;

            String consolesStr = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\JSON\\consoles.json";
            this.ItemLists = new ItemLists(consolesStr);

            this.AllItemNames = this.ItemLists.GetNames();
            this.AllItemPaths = this.itemLists.GetItemFilePaths();

            this.SelectedItemIndex = 1;
            
            input = new XboxControllerInputProvider(Event.EventInstance.EventAggregator);
            
            gamepadThread = new Thread(new ThreadStart(input.PollGamepad));
            gamepadThread.Start();

            SetControllerState("ITEMLIST_BROWSE");
            itemListToken = this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Subscribe(
                (controllerEventArgs) =>
                {
                    PerformAction(controllerEventArgs);
                }
            );

        }

        public void PerformAction(ItemListViewEventArgs e)
        {
            switch (e.action)
            {
                case "PAUSE_OPEN":
                    MinimizeProcess(appProc);
                    break;
                case "PAUSE_CLOSE":
                    switch (e.addlInfo[0])
                    {
                        case "RETURN_TO_APP":
                            MaximizeProcess(appProc);
                            SetControllerState("ITEM_PLAY");
                            break;
                        case "CLOSE_APP":
                            appProc.Kill();
                            SetControllerState("ITEMLIST_BROWSE");
                            break;
                        case "CLOSE_ALL":

                            break;
                    }
                    break;
            }
        }

        public void SetControllerState(string state)
        {
            if (controllerSubToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Unsubscribe(controllerSubToken);
            }
            switch (state)
            {
                case "ITEMLIST_BROWSE":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionItemListView(controllerEventArgs);
                        }
                    );
                    break;
                case "ITEMLIST_CONFIRM":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionConfirmationDialog(controllerEventArgs);
                        }
                    );
                    break;
                case "ITEM_PLAY":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionItemPlaying(controllerEventArgs);
                        }
                    );
                    break;
                case "ITEM_PAUSE":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionPauseDialog(controllerEventArgs);
                        }
                    );
                    break;
            }
            
        }

        void ControllerButtonPressToActionItemListView(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    string itemName = AllItemNames.ToList().ElementAt(SelectedItemIndex);
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("CONFIRM_OPEN", new string[]{ itemName }));
                    SetControllerState("ITEMLIST_CONFIRM");
                    break;
                case "B":
                    Console.WriteLine("Case 2");
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_UP"));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("ITEMLIST_MOVE_DOWN"));
                    break;
                case "RSHOULDER":
                    Console.WriteLine("");
                    break;
                case "GUIDE":
                    break;
            }
        }

        void ControllerButtonPressToActionConfirmationDialog(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    break;
                case "B":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    break;
                case "X":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    break;
                case "Y":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_SELECT_BUTTON"));
                    break;
                case "DUP":
                    break;
                case "DDOWN":
                    break;
                case "DLEFT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_MOVE_LEFT"));
                    break;
                case "DRIGHT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_MOVE_RIGHT"));
                    break;
                case "LSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_MOVE_LEFT"));
                    break;
                case "RSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<ConfirmationViewEventArgs>>().Publish(new ConfirmationViewEventArgs("CONFIRM_MOVE_RIGHT"));
                    break;
                case "GUIDE":
                    break;
            }
        }


        void ControllerButtonPressToActionPauseDialog(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_SELECT_BUTTON"));
                    break;
                case "B":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_SELECT_BUTTON"));
                    break;
                case "X":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_SELECT_BUTTON"));
                    break;
                case "Y":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_SELECT_BUTTON"));
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_MOVE_UP"));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<PauseViewEventArgs>>().Publish(new PauseViewEventArgs("PAUSE_MOVE_DOWN"));
                    break;
                case "DLEFT":
                    break;
                case "DRIGHT":
                    break;
                case "LSHOULDER":
                    break;
                case "RSHOULDER":
                    break;
                case "GUIDE":

                    break;
            }
        }

        void ControllerButtonPressToActionItemPlaying(ControllerEventArgs e)
        { 
            switch (e.buttonPressed)
            {
                case "GUIDE":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListViewEventArgs>>().Publish(new ItemListViewEventArgs("PAUSE_OPEN", new string[] { "" }));
                    SetControllerState("ITEM_PAUSE");
                    break;
            }
        }


        public void OpenSelectedItemInApp()
        {
            string autPath = "C:\\Program Files (x86)\\AutoIt3\\AutoIt3.exe";
            string maximizeActionPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Automation\\maximize.au3"; ;
            string appPath = ItemLists.GetCurrentConsoleAppPath();
            string itemPath = AllItemPaths.ToList().ElementAt(SelectedItemIndex);



            appProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    Arguments = "\"" + itemPath + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
                
            };

            appProc.Start();

            appProc.WaitForInputIdle();

            autProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = autPath,
                    Arguments = "\"" + maximizeActionPath + "\" \"" + appProc.MainWindowTitle + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            autProc.Start();

            autProc.WaitForExit();
        }

        public void MinimizeProcess(Process proc)
        {
            ShowWindow(proc.MainWindowHandle, 6);
        }

        public void MaximizeProcess(Process proc)
        {
            ShowWindow(proc.MainWindowHandle, 3);
        }
    }
}
