using System.Windows;
using Prism.Unity;
using Prism.Modularity;

namespace FilePlayer
{
    public class Bootstrapper : UnityBootstrapper
    {

        #region UnityBootstrapper Implementation

        protected override DependencyObject CreateShell()
        {
            return new Shell();
        }

        protected override void InitializeShell()
        {
            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

        }


        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(Module));
        }

        #endregion

    }
}
