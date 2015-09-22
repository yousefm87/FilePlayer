using FilePlayer.ViewModels;


namespace FilePlayer
{
    class ItemListViewDesignViewModel
    {
        public ItemListViewDesignViewModel()
        {
            this.ItemListViewModel = new ItemListViewModel();
        }

        public ItemListViewModel ItemListViewModel { get; set; }
    }
}

