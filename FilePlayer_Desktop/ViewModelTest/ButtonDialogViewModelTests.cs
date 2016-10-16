using System;
using System.Linq;
using NUnit.Framework;
using FilePlayer.ViewModels;
using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FilePlayer.ViewModelTest
{
    [TestFixture]
    class ButtonDialogViewModelTests
    {
        IEventAggregator eventAggregator = null;

        [OneTimeSetUp]
        public void TestSetup()
        {
            eventAggregator = Event.EventInstance.EventAggregator;
        }

        [TestCase("ITEM_LIST_PAUSE_OPEN")]
        [TestCase("ITEM_LIST_CONFIRMATION_OPEN")]
        [TestCase("APP_PAUSE_OPEN")]
        public void Should_Have_Equal_Button_To_Response_Ratio(String dialogType)
        {
            ButtonDialogViewModel viewModel = new ButtonDialogViewModel(eventAggregator, dialogType);

            bool isNameToResponseRatioEqual = (viewModel.ButtonNames.Count() == viewModel.ButtonResponses.Count());
            Assert.IsTrue(isNameToResponseRatioEqual, "Button Name to Response ratio is not 1:1");
        }

        [TestCase("ITEM_LIST_PAUSE_OPEN")]
        [TestCase("ITEM_LIST_CONFIRMATION_OPEN")]
        [TestCase("APP_PAUSE_OPEN")]
        public void Test_MoveUp(String dialogType)
        {
            ButtonDialogViewModel viewModel = new ButtonDialogViewModel(eventAggregator, dialogType);
            bool isMoveUpSuccess;

            //Move to end of list 
            for (int i=1; i < viewModel.ButtonNames.Count(); i++) 
            {
                eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_MOVE_DOWN", new string[] { }));
            }

            for (int i=1; i < viewModel.ButtonNames.Count(); i++)
            {
                eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_MOVE_UP", new string[] { }));

                isMoveUpSuccess = viewModel.SelectedButtonIndex == viewModel.ButtonNames.Count() - 1 - i;
                Assert.IsTrue(isMoveUpSuccess, "Move Up did not work. If Move Down Test failed, that might be the cause.");
            }

            eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_MOVE_UP", new string[] { }));

            isMoveUpSuccess = viewModel.SelectedButtonIndex == 0;
            Assert.IsTrue(isMoveUpSuccess, "Moved Up past first item. If Move Down Test failed, that might be the cause.");
        }

        [TestCase("ITEM_LIST_PAUSE_OPEN")]
        [TestCase("ITEM_LIST_CONFIRMATION_OPEN")]
        [TestCase("APP_PAUSE_OPEN")]
        public void Test_MoveDown(String dialogType)
        {
            ButtonDialogViewModel viewModel = new ButtonDialogViewModel(eventAggregator, dialogType);
            bool isMoveDownSuccess;

            for (int i = 1; i < viewModel.ButtonNames.Count(); i++)
            {
                eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_MOVE_DOWN", new string[] { }));

                int expected = i;
                int actual = viewModel.SelectedButtonIndex;
                isMoveDownSuccess = expected == actual;

                Assert.IsTrue(isMoveDownSuccess, "Move Down did not work. Expected:" + expected + " Actual:" + actual);
            }

            eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("BUTTONDIALOG_MOVE_DOWN", new string[] { }));

            isMoveDownSuccess = viewModel.SelectedButtonIndex == viewModel.ButtonNames.Count() - 1;
            Assert.IsTrue(isMoveDownSuccess, "Moved Down past last item.");
        }


    }
}
