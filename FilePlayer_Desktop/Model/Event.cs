using Microsoft.Practices.Prism.PubSubEvents;
using System;

namespace FilePlayer.Model
{
    public class ViewEventArgs : EventArgs
    {
        public string action;
        public string[] addlInfo;


        public ViewEventArgs(string _action)
        {
            action = _action;
            addlInfo = new string[0] { };
        }

        public ViewEventArgs(string _action, string[] _addlInfo)
        {
            action = _action;
            addlInfo = _addlInfo;
        }
    }

    public sealed class Event
    {
        #region Class Properties

        internal static Event EventInstance
        {
            get
            {
                return eventInstance;
            }
        }

        #endregion

        #region Instance Properties

        internal IEventAggregator EventAggregator
        {
            get
            {
                if (eventAggregator == null)
                {
                    eventAggregator = new EventAggregator();
                }

                return eventAggregator;
            }
        }

        #endregion

        #region Constructors

        private Event()
        {

        }

        #endregion

        #region Class Fields

        private static readonly Event eventInstance = new Event();

        #endregion

        #region Instance Fields

        private IEventAggregator eventAggregator;

        #endregion

    }
}
