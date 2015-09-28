using System.Collections.Generic;
using System.Linq;
using DelftTools.Shell.Core.Workflow;

namespace DelftTools.Shell.Core.Extensions
{
    public static class CompositeActivityExtensions
    {
        /// <summary>
        /// Get activities in this composition, that according to the currently selected workflow, run simultaneous with the given target activity.
        /// Note: Simultaneous = parallel
        /// </summary>
        /// <param name="compositeModel"></param>
        /// <param name="targetActivity"></param>
        /// <returns></returns>
        public static IEnumerable<IActivity> GetActivitiesRunningSimultaneous(this ICompositeActivity compositeModel, IActivity targetActivity)
        {
            var workflow = compositeModel.CurrentWorkflow ?? compositeModel; //if compositeModel is already a workflow, this also works fine

            var parentActivity = GetParentActivity(workflow, targetActivity);

            //we make the assumption here that all other composite activities run parallel
            if (parentActivity != null && !(parentActivity is SequentialActivity)) 
            {
                return parentActivity.Activities.Select(UnwrapActivity).Where(a => a != targetActivity);
            }
            return new IActivity[0];
        }

        public static IEnumerable<T> GetAllActivitiesRecursive<T>(this ICompositeActivity compositeModel) where T : IActivity
        {
            if (compositeModel is T)
            {
                yield return (T) compositeModel;
            }

            foreach (var subActivity in compositeModel.Activities)
            {
                var compActivity = subActivity as ICompositeActivity;
                if (compActivity != null)
                {
                    foreach (var activity in GetAllActivitiesRecursive<T>(compActivity))
                    {
                        yield return activity;
                    }
                }
                else
                {
                    if (subActivity is T)
                    {
                        yield return (T) subActivity;
                    }
                }
            }
        }

        private static ICompositeActivity GetParentActivity(IActivity workflow, IActivity targetActivity)
        {
            var compositeWorkflow = workflow as ICompositeActivity;
            if (compositeWorkflow == null)
                return null;

            foreach (var a in compositeWorkflow.Activities)
            {
                var activity = UnwrapActivity(a);

                if (activity == targetActivity)
                    return compositeWorkflow;

                var parent = GetParentActivity(activity, targetActivity);

                if (parent != null)
                    return parent;
            }
            return null;
        }

        private static IActivity UnwrapActivity(IActivity activity)
        {
            return activity is ActivityWrapper ? ((ActivityWrapper) activity).Activity : activity;
        }
    }
}