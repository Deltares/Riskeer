using System;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Shell.Core.Workflow
{
    public interface IActivityRunner
    {
        /// <summary>
        /// Fired when an activity completes
        /// </summary>
        event EventHandler<ActivityEventArgs> ActivityCompleted;

        /// <summary>
        /// Occurs when the queuestate changes
        /// </summary>
        event EventHandler IsRunningChanged;

        event EventHandler<ActivityStatusChangedEventArgs> ActivityStatusChanged;

        /// <summary>
        /// All activities (todo and running)
        /// </summary>
        IEventedList<IActivity> Activities { get; }

        /// <summary>
        /// Determines whether some activity is running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Call to find out if a specific activity is being run
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        bool IsRunningActivity(IActivity activity);

        /// <summary>
        /// Schedules activity to run.
        /// </summary>
        /// <param name="activity">Activity to process</param>
        void Enqueue(IActivity activity);

        /// <summary>
        /// Cancels the specified activity
        /// </summary>
        /// <param name="activity"></param>
        void Cancel(IActivity activity);

        /// <summary>
        /// Stops all activities and empties the TODO-list
        /// </summary>
        void CancelAll();
    }
}