namespace FilePlayer.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            this.ItemListViewModel = new ItemListViewModel();
        }

        public ItemListViewModel ItemListViewModel { get; set; }
    }
}
