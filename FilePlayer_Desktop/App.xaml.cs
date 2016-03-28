using FilePlayer.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Windows;

namespace FilePlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // hook on error before app really starts
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            base.OnStartup(e);
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // put your tracing or logging code here (I put a message box as an example)
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        SubscriptionToken viewActionToken;
        private IEventAggregator iEventAggregator;

        App()
        {
            iEventAggregator = Event.EventInstance.EventAggregator;
            viewActionToken = this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Subscribe(
                (viewEventArgs) => { EventHandler(viewEventArgs); }
            );
        }

        private void EventHandler(ViewEventArgs e)
        {
            if (e.action.Equals("EXIT"))
            {
                Dispatcher.Invoke((Action)delegate
                {
                    this.iEventAggregator.GetEvent<PubSubEvent<ViewEventArgs>>().Publish(new ViewEventArgs("GAMEPAD_ABORT", new String[] { }));
                    Application.Current.Shutdown();
                });
            }
        }
    }
}
