using System.Collections.Generic;
using Core.Common.Base.Workflow;

namespace Core.Common.Base.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ICompositeActivity"/>.
    /// </summary>
    public static class CompositeActivityExtensions
    {
        /// <summary>
        /// Gets all activities recursively matching a given type.
        /// </summary>
        /// <typeparam name="T">The type of activity to be matched for.</typeparam>
        /// <param name="compositeActivity">The composite activity.</param>
        /// <returns>An iterator over all activities implementing/inheriting <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> GetAllActivitiesRecursive<T>(this ICompositeActivity compositeActivity) where T : IActivity
        {
            if (compositeActivity is T)
            {
                yield return (T) compositeActivity;
            }

            foreach (var subActivity in compositeActivity.Activities)
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
    }
}