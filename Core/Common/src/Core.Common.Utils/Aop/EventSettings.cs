using System.ComponentModel;

namespace Core.Common.Utils.Aop
{
    /// <summary>
    /// Settings for EntityAttribute behavior
    /// </summary>
    /// <remarks>Seperate class from EntityAttribute to remove need to reference PostSharp if you just want to check/modify these fields</remarks>
    public static class EventSettings
    {
        public static bool BubblingEnabled = true;
        public static bool EnableLogging = false;
        public static object LastEventBubbler;

        #region Global Helpers

        public static void FirePropertyChanging(object lastSender, object originalSender,
                                                PropertyChangingEventArgs args,
                                                PropertyChangingEventHandler fireAction)
        {
            if (fireAction == null)
            {
                return;
            }

            var previousSender = LastEventBubbler;
            LastEventBubbler = lastSender;

            try
            {
                fireAction(originalSender, args);
            }
            finally
            {
                LastEventBubbler = previousSender;
            }
        }

        public static void FirePropertyChanged(object lastSender, object originalSender,
                                               PropertyChangedEventArgs args,
                                               PropertyChangedEventHandler fireAction)
        {
            if (fireAction == null)
            {
                return;
            }

            var previousSender = LastEventBubbler;
            LastEventBubbler = lastSender;

            try
            {
                fireAction(originalSender, args);
            }
            finally
            {
                LastEventBubbler = previousSender;
            }
        }

        #endregion
    }
}