using System.Collections.Generic;
using DelftTools.Shell.Core.Workflow;

namespace DelftTools.Shell.Core.Extensions
{
    public static class CompositeActivityExtensions
    {
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
            {
                return null;
            }

            foreach (var a in compositeWorkflow.Activities)
            {
                if (a == targetActivity)
                {
                    return compositeWorkflow;
                }

                var parent = GetParentActivity(a, targetActivity);

                if (parent != null)
                {
                    return parent;
                }
            }

            return null;
        }
    }
}