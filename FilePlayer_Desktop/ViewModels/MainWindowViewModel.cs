using System;

namespace FilePlayer.ViewModels
{
    public class ViewEventArgs : EventArgs
    {
        public string action;
    }

    public class MainWindowViewEventArgs : ViewEventArgs {}

    public class MainWindowViewModel
    {

        public delegate void MainWindowViewEventHandler<MainWindowViewEventArgs>(object sender, MainWindowViewEventArgs e);
        public event MainWindowViewEventHandler<MainWindowViewEventArgs> SendAction;

        public MainWindowViewModel()
        {
            this.ItemListViewModel = new ItemListViewModel();
        }

        public ItemListViewModel ItemListViewModel { get; set; }

 
        
    }
}
