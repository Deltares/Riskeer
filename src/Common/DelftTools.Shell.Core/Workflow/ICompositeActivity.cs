using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Shell.Core.Workflow
{
    /// <summary>
    /// Special type of activity which supports workflows to run it's child activities.
    /// 
    /// Note that when activities contained in <see cref="Activities"/> are added to a specific workflow they must be wrapped by <see cref="CompositeActivity"/>.
    /// </summary>
    public interface ICompositeActivity : IActivity
    {
        /// <summary>
        /// Child activities.
        /// </summary>
        IEventedList<IActivity> Activities { get; }

        /// <summary>
        /// Indicates that the 
        /// </summary>
        bool ReadOnly { get; set; }

        /// <summary>
        /// The workflow to be executed when calling <see cref="IActivity.Execute"/>.
        /// </summary>
        ICompositeActivity CurrentWorkflow { get; }
    }
}