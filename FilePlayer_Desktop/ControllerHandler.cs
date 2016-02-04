using FilePlayer.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using System;

namespace FilePlayer
{
    class ControllerHandler
    {
        private IEventAggregator iEventAggregator;
        private SubscriptionToken controllerSubToken = null;

        public ControllerHandler(IEventAggregator iEventAggregator)
        {
            this.iEventAggregator = iEventAggregator;




            SetControllerState("ITEMLIST_BROWSE");


        }

        public void SetControllerState(string state)
        {

            if (controllerSubToken != null)
            {
                this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Unsubscribe(controllerSubToken);
            }
            switch (state)
            {
                case "NONE":
                    break;
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
                case "ITEMLIST_PAUSE":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionItemlistPause(controllerEventArgs);
                        }
                    );
                    break;
                case "FILTER_MAIN":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionFilter(controllerEventArgs);
                        }
                    );
                    break;
                case "CHAR_GETTER":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionCharGetter(controllerEventArgs);
                        }
                    );
                    break;
                case "VERTICAL_OPTION_SELECTER":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionVerticalOptionSelecter(controllerEventArgs);
                        }
                    );
                    break;
                case "BUTTON_DIALOG":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionButtonDialog(controllerEventArgs);
                        }
                    );
                    break;
                case "SEARCH_GAME_DATA":
                    controllerSubToken = this.iEventAggregator.GetEvent<PubSubEvent<ControllerEventArgs>>().Subscribe(
                        (controllerEventArgs) =>
                        {
                            ControllerButtonPressToActionSearchGameData(controllerEventArgs);
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
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_OPEN", new string[] { "ITEM_LIST_CONFIRMATION_OPEN" }));
                    SetControllerState("BUTTON_DIALOG");
                    break;
                case "B":
                    break;
                case "X":
                    break;
                case "Y":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("OPEN_FILTER", new string[] { "" }));
                    SetControllerState("FILTER_MAIN");
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ITEMLIST_MOVE_UP", new string[] { 1.ToString() }));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ITEMLIST_MOVE_DOWN", new string[] { 1.ToString() }));
                    break;
                case "DLEFT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ITEMLIST_MOVE_LEFT", new string[] { 1.ToString() }));
                    break;
                case "DRIGHT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ITEMLIST_MOVE_RIGHT", new string[] { 1.ToString() }));
                    break;
                case "LSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ITEMLIST_MOVE_UP", new string[] { 10.ToString() }));
                    break;
                case "RSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ITEMLIST_MOVE_DOWN", new string[] { 10.ToString() }));
                    break;
                case "GUIDE":
                    //this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ITEMLIST_PAUSE_OPEN", new string[] { 10.ToString() }));
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_OPEN", new string[] { "ITEM_LIST_PAUSE_OPEN", "ITEM_LIST_BROWSE" }));
                    
                    SetControllerState("BUTTON_DIALOG");
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

        void ControllerButtonPressToActionButtonDialog(ControllerEventArgs e)
        {
            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_SELECT_BUTTON"));
                    break;
                case "B":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_CLOSE"));
                    break;
                case "X":
                    break;
                case "Y":
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_MOVE_UP"));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_MOVE_DOWN"));
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
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_CLOSE"));
                    break;
            }
        }

        void ControllerButtonPressToActionItemlistPause(ControllerEventArgs e)
        {
            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListPauseViewEventArgs>>().Publish(new ItemListPauseViewEventArgs("ITEMLIST_PAUSE_SELECT_BUTTON"));
                    break;
                case "B":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListPauseViewEventArgs>>().Publish(new ItemListPauseViewEventArgs("ITEMLIST_PAUSE_SELECT_BUTTON"));
                    break;
                case "X":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListPauseViewEventArgs>>().Publish(new ItemListPauseViewEventArgs("ITEMLIST_PAUSE_SELECT_BUTTON"));
                    break;
                case "Y":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListPauseViewEventArgs>>().Publish(new ItemListPauseViewEventArgs("ITEMLIST_PAUSE_SELECT_BUTTON"));
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListPauseViewEventArgs>>().Publish(new ItemListPauseViewEventArgs("ITEMLIST_PAUSE_MOVE_UP"));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<ItemListPauseViewEventArgs>>().Publish(new ItemListPauseViewEventArgs("ITEMLIST_PAUSE_MOVE_DOWN"));
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
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("ITEMLIST_PAUSE_CLOSE", new string[] { "ITEMLIST_PAUSE_CLOSE" }));

                    break;
            }
        }

        void ControllerButtonPressToActionItemPlaying(ControllerEventArgs e)
        {
            switch (e.buttonPressed)
            {
                case "GUIDE":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_OPEN", new string[] { "APP_PAUSE_OPEN" }));

                    SetControllerState("BUTTON_DIALOG");
                    break;
            }
        }

        void ControllerButtonPressToActionFilter(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_SELECT_CONTROL", new string[] { "" }));
                    break;
                case "B":
                    break;
                case "X":
                    break;
                case "Y":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CLOSE_FILTER", new string[] { "" }));
                    SetControllerState("ITEMLIST_BROWSE");
                    break;
                case "DUP":
                    break;
                case "DDOWN":
                    break;
                case "DLEFT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_MOVE_LEFT"));
                    break;
                case "DRIGHT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("FILTER_MOVE_RIGHT"));
                    break;
                case "LSHOULDER":
                    break;
                case "RSHOULDER":
                    break;
                case "GUIDE":
                    break;
            }
        }

        void ControllerButtonPressToActionCharGetter(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_SELECT", new string[] { "" }));
                    break;
                case "B":
                    this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_CLOSE", new string[] { "" }));
                    break;
                case "X":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("CHAR_BACK", new string[] { "" }));
                    break;
                case "Y":
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_UP"));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_DOWN"));
                    break;
                case "DLEFT":
                    this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_LEFT"));
                    break;
                case "DRIGHT":
                    this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_RIGHT"));
                    break;
                case "LSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_SWITCHCHARSET_LEFT"));
                    break;
                case "RSHOULDER":
                    this.iEventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_SWITCHCHARSET_RIGHT"));
                    break;
                case "GUIDE":
                    break;
            }
        }

        void ControllerButtonPressToActionVerticalOptionSelecter(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("VOS_SELECT", new string[] { "" }));
                    break;
                case "B":
                    break;
                case "X":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("VOS_BACK", new string[] { "" }));
                    break;
                case "Y":
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("VOS_MOVE_UP"));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("VOS_MOVE_DOWN"));
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


        void ControllerButtonPressToActionSearchGameData(ControllerEventArgs e)
        {

            switch (e.buttonPressed)
            {
                case "A":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SEARCHGAMEDATA_SELECT", new string[] { "" }));
                    break;
                case "B":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SEARCHGAMEDATA_CLOSE", new string[] { "" }));
                    break;
                case "X":
                    break;
                case "Y":
                    break;
                case "DUP":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SEARCHGAMEDATA_MOVE_UP"));
                    break;
                case "DDOWN":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SEARCHGAMEDATA_MOVE_DOWN"));
                    break;
                case "DLEFT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SEARCHGAMEDATA_MOVE_LEFT"));
                    break;
                case "DRIGHT":
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("SEARCHGAMEDATA_MOVE_RIGHT"));
                    break;
                case "LSHOULDER":
                    break;
                case "RSHOULDER":
                    break;
                case "GUIDE":
                    break;
            }
        }
    }
}
