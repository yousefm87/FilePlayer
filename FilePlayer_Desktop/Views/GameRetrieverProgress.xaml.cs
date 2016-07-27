using System.Windows.Controls;
using Microsoft.Practices.Prism.PubSubEvents;
using FilePlayer.Model;
using FilePlayer.ViewModels;
using MahApps.Metro.Controls;

namespace FilePlayer.Views
{
    /// <summary>
    /// Interaction logic for GameRetrieverProgress.xaml
    /// </summary>
    public partial class GameRetrieverProgress : MetroWindow
    {
        public SubscriptionToken progressActionToken;

        public string[] buttonActions;
        public Button[] buttons;
        
        public GameRetrieverProgressViewModel GameRetrieverProgressViewModel { get; set; }

        public GameRetrieverProgress()
        {
            InitializeComponent();

            GameRetrieverProgressViewModel = new GameRetrieverProgressViewModel(Event.EventInstance.EventAggregator);
            
            this.DataContext = GameRetrieverProgressViewModel;

            this.Topmost = true;
            this.Topmost = false;
            Activate();

            
        }

    }
}