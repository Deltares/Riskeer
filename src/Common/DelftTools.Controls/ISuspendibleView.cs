using System;

namespace DelftTools.Controls
{
    /// <summary>
    /// TODO: remove this interface, not a domain logic! If we have buggy event handling - fix it in event handlers (we already have BubblingEnabled in aspect?!?).
    /// </summary>
    [Obsolete]
    public interface ISuspendibleView
    {
        /// <summary>
        /// Suspends any binding / update logic in the view
        /// </summary>
        void SuspendUpdates();

        /// <summary>
        /// Resume databinding / update functionality
        /// </summary>
        void ResumeUpdates();
    }
}