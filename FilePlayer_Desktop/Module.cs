using Prism.Modularity;
using Prism.Regions;
using FilePlayer.Views;

namespace FilePlayer
{
    public class Module : IModule
    {

        #region Constructors

        public Module(IRegionManager iRegionManager)
        {
            this.iRegionManager = iRegionManager;
        }

        #endregion

        #region IModule Implementation

        public void Initialize()
        {
            
            this.iRegionManager.RegisterViewWithRegion("ItemListView", typeof(Views.ItemListView));

        }

        #endregion

        #region Instance Fields

        private IRegionManager iRegionManager;

        #endregion

    }
}
