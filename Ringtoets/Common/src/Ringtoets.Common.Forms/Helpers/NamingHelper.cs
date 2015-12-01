using System;
using System.Collections.Generic;
using System.Linq;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for generating unique names.
    /// </summary>
    public static class NamingHelper
    {
        /// <summary>
        /// Generate an unique name given a collection of existing named objects.
        /// </summary>
        /// <typeparam name="T">Type of objects in the collection.</typeparam>
        /// <param name="existingObjects">All existing named objects.</param>
        /// <param name="nameBase">The base naming scheme to use.</param>
        /// <param name="nameGetter">Getter method to determine the name of each object in <paramref cref="existingObjects"/>.</param>
        /// <returns>A unique name based on <paramref name="nameBase"/> that is not used
        /// in <paramref name="existingObjects"/>.</returns>
        public static string GetUniqueName<T>(IEnumerable<T> existingObjects, string nameBase, Func<T, string> nameGetter)
        {
            int i = 1;
            string result = nameBase;
            var existingNames = existingObjects.Select(nameGetter).ToArray();
            while (existingNames.Any(name => name.Equals(result)))
            {
                result = string.Format("{0} ({1})", nameBase, i++);
            }
            return result;
        }
    }
}