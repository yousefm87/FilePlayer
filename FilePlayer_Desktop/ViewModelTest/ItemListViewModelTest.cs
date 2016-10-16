using NUnit.Framework;
using Microsoft.Practices.Prism.PubSubEvents;
using FilePlayer.Model;
using FilePlayer.ViewModels;
using System.Linq;
using System.Windows;

namespace FilePlayer.ViewModelTest
{
    [TestFixture]
    class ItemListViewModelTest
    {
        IEventAggregator eventAggregator = null;

        [OneTimeSetUp]
        public void TestSetup()
        {
            eventAggregator = Event.EventInstance.EventAggregator;
        }
        
        [TestCase(1)]
        [TestCase(10)]
        public void TestMoveDown(int numMoves)
        {
            ViewEventArgs moveDownEventArgs = new ViewEventArgs("ITEMLIST_MOVE_DOWN", new string[] { numMoves.ToString() });
            ItemListViewModel viewModel = new ItemListViewModel(eventAggregator);
            for (int i = 1; i <= viewModel.AllItemNames.Count() + 1; i++) 
            {
                int expected = viewModel.SelectedItemIndex + numMoves;
                if (expected > viewModel.AllItemNames.Count() - 1)
                {
                    expected = viewModel.AllItemNames.Count() - 1;
                }
                this.eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(moveDownEventArgs);
                
                int actual = viewModel.SelectedItemIndex;

                Assert.IsTrue(expected == actual, "Move Down failed. Expected: " + expected + " Actual: " + actual);
            }
        }

        [TestCase(1)]
        [TestCase(10)]
        public void TestMoveUp(int numMoves)
        {
            ViewEventArgs moveUpEventArgs = new ViewEventArgs("ITEMLIST_MOVE_UP", new string[] { numMoves.ToString() });
            ItemListViewModel viewModel = new ItemListViewModel(eventAggregator);
            int lastItemIndex = viewModel.AllItemNames.Count() - 1;

            viewModel.SelectedItemIndex = lastItemIndex;

            for (int i = 1; i <= viewModel.AllItemNames.Count() + 1; i++)
            {
                int expected = viewModel.SelectedItemIndex - numMoves;
                if (expected < 0)
                {
                    expected = 0;
                }
                this.eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(moveUpEventArgs);

                int actual = viewModel.SelectedItemIndex;

                Assert.IsTrue(expected == actual, "Move Up failed. Expected: " + expected + " Actual: " + actual);
            }
        }

        [TestCase()]
        public void TestMoveLeft()
        {
            ViewEventArgs moveLeftEventArgs = new ViewEventArgs("ITEMLIST_MOVE_LEFT", new string[] { });
            ItemListViewModel viewModel = new ItemListViewModel(eventAggregator);
            
            viewModel.ItemLists.CurrConsole = viewModel.ItemLists.GetConsoleCount() - 1;

            for (int i = 1; i <= viewModel.ItemLists.GetConsoleCount() + 1; i++)
            {
                int expected = viewModel.SelectedItemIndex - 1;
                if (expected < 0)
                {
                    expected = 0;
                }
                this.eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(moveLeftEventArgs);

                int actual = viewModel.SelectedItemIndex;

                Assert.IsTrue(expected == actual, "Move Left failed. Expected: " + expected + " Actual: " + actual);
            }
        }

        [TestCase()]
        public void TestMoveRight()
        {
            ViewEventArgs moveRightEventArgs = new ViewEventArgs("ITEMLIST_MOVE_RIGHT", new string[] { });
            ItemListViewModel viewModel = new ItemListViewModel(eventAggregator);

            for (int i = 1; i <= viewModel.ItemLists.GetConsoleCount() + 1; i++)
            {
                int expected = viewModel.ItemLists.CurrConsole + 1;
                if (expected > viewModel.ItemLists.GetConsoleCount() - 1)
                {
                    expected = viewModel.ItemLists.GetConsoleCount() - 1;
                }
                this.eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(moveRightEventArgs);

                int actual = viewModel.ItemLists.CurrConsole;

                Assert.IsTrue(expected == actual, "Move Right failed. Expected: " + expected + " Actual: " + actual);
            }
        }
        
        [TestCase()]
        public void Test_ToggleFilter()
        {
            ViewEventArgs filterEventArgs = new ViewEventArgs("TOGGLE_FILTER", new string[] { });
            ItemListViewModel viewModel = new ItemListViewModel(eventAggregator);

            for (int i = 1; i <= 10; i++)
            {
                Visibility expected;
                if (viewModel.FilterVisibility == Visibility.Hidden)
                {
                    expected = Visibility.Visible;
                }
                else
                {
                    expected = Visibility.Hidden;
                }

                this.eventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(filterEventArgs);

                Visibility actual = viewModel.FilterVisibility;

                Assert.IsTrue(expected == actual, "Toggle filter failed. Expected: " + expected + " Actual: " + actual);
            }
        }



    }
}
