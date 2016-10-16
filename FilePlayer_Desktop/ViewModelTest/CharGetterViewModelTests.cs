using NUnit.Framework;
using Microsoft.Practices.Prism.PubSubEvents;
using FilePlayer.Model;
using FilePlayer.ViewModels;
using FilePlayer.Constants;

namespace FilePlayer.ViewModelTest
{
    [TestFixture]
    class CharGetterViewModelTests
    {
        IEventAggregator eventAggregator = null;

        [OneTimeSetUp]
        public void TestSetup()
        {
            eventAggregator = Event.EventInstance.EventAggregator;
        }


        [TestCase()]
        public void Test_MoveRight()
        {
            int columnCount = 10;
            int rowCount = 4;
            int numControls = columnCount * rowCount + 1;
            bool hasSpaceBar = true;
            CharGetterViewModel viewModel = new CharGetterViewModel(columnCount, rowCount, numControls, hasSpaceBar);

            for (int i=1; i <= columnCount + 1; i++)
            {
                this.eventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_RIGHT"));

                int expected = i % columnCount;
                int actual = viewModel.SelectedControlIndex;
                Assert.IsTrue(expected == actual, "Move Right failed. Expected: " + expected + " Actual: " + actual);
            }
        }


        [TestCase()]
        public void Test_MoveLeft()
        {
            int columnCount = 10;
            int rowCount = 4;
            int numControls = columnCount * rowCount + 1;
            bool hasSpaceBar = true;
            CharGetterViewModel viewModel = new CharGetterViewModel(columnCount, rowCount, numControls, hasSpaceBar);

            for (int i = 1; i <= columnCount; i++)
            {
                this.eventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_LEFT"));

                int expected = columnCount - i; 
                int actual = viewModel.SelectedControlIndex;
                Assert.IsTrue(expected == actual, "Move Left failed. Expected: " + expected + " Actual: " + actual);
            }
        }


        [TestCase()]
        public void Test_MoveUp()
        {
            int columnCount = 10;
            int rowCount = 4;
            int numControls = columnCount * (rowCount - 1) + 1;
            bool hasSpaceBar = true;
            CharGetterViewModel viewModel = new CharGetterViewModel(columnCount, rowCount, numControls, hasSpaceBar);

            this.eventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_UP"));
            int expected = numControls - 1;
            int actual = viewModel.SelectedControlIndex;
            Assert.IsTrue(expected == actual, "Move Up failed. Expected: " + expected + " Actual: " + actual);

            //moves to 2nd after 2 downs bc of empty strings in CharSets.charsetABC row 3, col 0
            this.eventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_UP"));
            expected = columnCount;
            actual = viewModel.SelectedControlIndex;
            Assert.IsTrue(expected == actual, "Move Up failed. Expected: " + expected + " Actual: " + actual);
        }


        [TestCase()]
        public void Test_MoveDown()
        {
            int columnCount = 10;
            int rowCount = 4;
            int numControls = columnCount * (rowCount - 1) + 1;
            bool hasSpaceBar = true;
            CharGetterViewModel viewModel = new CharGetterViewModel(columnCount, rowCount, numControls, hasSpaceBar);

            this.eventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_DOWN"));
            int expected = columnCount;
            int actual = viewModel.SelectedControlIndex;
            Assert.IsTrue(expected == actual, "Move Down failed. Expected: " + expected + " Actual: " + actual);

            //moves to spacebar after 2 downs bc of empty strings in CharSets.charsetABC row 3, col 0
            this.eventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_MOVE_DOWN"));
            expected = numControls - 1;
            actual = viewModel.SelectedControlIndex;
            Assert.IsTrue(expected == actual, "Move Down failed. Expected: " + expected + " Actual: " + actual);
        }


        [TestCase()]
        public void Test_SwitchCharSetLeft()
        {
            int columnCount = 10;
            int rowCount = 4;
            int numControls = columnCount * (rowCount - 1) + 1;
            bool hasSpaceBar = true;
            CharGetterViewModel viewModel = new CharGetterViewModel(columnCount, rowCount, numControls, hasSpaceBar);

            for (int i = CharSets.charSets.Length - 1; i >= 0; i--)
            {
                this.eventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_SWITCHCHARSET_LEFT"));
                int expected = i % CharSets.charSets.Length;
                int actual = viewModel.CurrCharSetIndex;
                Assert.IsTrue(expected == actual, "Switch CharSet Left failed. Expected: " + expected + " Actual: " + actual);
            }
        }

 
        [TestCase()]
        public void Test_SwitchCharSetRight()
        {
            int columnCount = 10;
            int rowCount = 4;
            int numControls = columnCount * (rowCount - 1) + 1;
            bool hasSpaceBar = true;
            CharGetterViewModel viewModel = new CharGetterViewModel(columnCount, rowCount, numControls, hasSpaceBar);

            for (int i = 1; i <= CharSets.charSets.Length; i++)
            {
                this.eventAggregator.GetEvent<PubSubEvent<CharGetterEventArgs>>().Publish(new CharGetterEventArgs("CHAR_SWITCHCHARSET_RIGHT"));
                int expected = i % CharSets.charSets.Length;
                int actual = viewModel.CurrCharSetIndex;
                Assert.IsTrue(expected == actual, "Switch CharSet Right failed. Expected: " + expected + " Actual: " + actual);
            }
        }


    }
}
