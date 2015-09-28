using System;

namespace DelftTools.Utils.Threading
{
    // HACK: this should not be needed, if we need to globally disable/enable events - add it in DelayedEventHandler
    public static class DelayedEventHandlerController
    {
        //default to true
        private static bool fireEvents = true;

        public static bool FireEvents
        {
            get { return fireEvents; }
            set
            {
                //don't sent unnecessary changes
                if (fireEvents == value)
                {
                    return;
                }
                fireEvents = value;
                OnFireEventsChanged();
            }
        }

        private static void OnFireEventsChanged()
        {
            var localEventCopy = FireEventsChanged; //thread safety
            if (localEventCopy != null)
            {
                //no sender since we are in a static context here
                localEventCopy(null, EventArgs.Empty);
            }
        }

        public static event EventHandler FireEventsChanged;
    }
}