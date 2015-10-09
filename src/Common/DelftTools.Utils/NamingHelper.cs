using System;
using System.Collections.Generic;
using System.Linq;

namespace DelftTools.Utils
{
    ///<summary>
    ///</summary>
    [Obsolete("This class is messy (taking IEnumerable as arguments, Type t?!)")]
    public static class NamingHelper
    {
        /// <summary>
        /// Extracts an unique name from the item collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="items"></param>
        /// <param name="t">TODO: confusing, why specify it if we have filter?!</param>
        /// <returns></returns>
        public static string GetUniqueName<T>(string filter, IEnumerable<T> items, Type t = null) where T : INameable
        {
            return GetUniqueName<T>(filter, items, t, true);
        }

        /// <summary>
        /// Extracts an unique name from the item collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">specifies name template, can be in a form: "item name {0}"</param>
        /// <param name="items"></param>
        /// <param name="t">TODO: confusing, why specify it if we have filter?!</param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static string GetUniqueName<T>(string filter, IEnumerable<T> items, Type t, bool ignoreCase) where T : INameable
        {
            if (null != filter)
            {
                if (filter.Length == 0)
                {
                    // to do test if filter has format code
                    throw new ArgumentException("Can not create an unique name when filter is empty.");
                }

                if (!filter.Contains("{0")) // format supported with {0:d2}
                {
                    throw new ArgumentException("Invalid filter");
                }
            }
            else
            {
                filter = t.Name + "{0}";
            }

            var names = items.Select(item => item.Name);
            return GenerateUniqueNameFromList(filter, ignoreCase, names);
        }

        public static string GenerateUniqueNameFromList(string filter, bool ignoreCase, IEnumerable<string> names)
        {
            var namesList = names.Distinct().ToList();

            String unique;
            int id = 1;

            do
            {
                unique = String.Format(filter, id++);
            } while (namesList.Contains(unique, new NameComparer(ignoreCase)));

            return unique;
        }

        /// <summary>
        /// Makes all <param name="nameables" /> have a unique name
        /// </summary>
        /// <param name="nameables">List of INameble objects to make unique</param>
        public static void MakeNamesUnique(IEnumerable<INameable> nameables)
        {
            var uniqueNames = new HashSet<string>();
            var previousNonUniqueValue = string.Empty;
            var previousCounter = 0;

            foreach (var nameable in nameables)
            {
                var name = nameable.Name;
                if (uniqueNames.Contains(name))
                {
                    string uniqueName;
                    if (previousNonUniqueValue == name)
                    {
                        uniqueName = string.Format(name + "{0}", previousCounter);
                        previousCounter++;
                    }
                    else
                    {
                        var uniqueNameForListTuple = GenerateUniqueNameForList(name + "{0}", true, uniqueNames);
                        uniqueName = uniqueNameForListTuple.First;

                        previousCounter = uniqueNameForListTuple.Second;
                        previousNonUniqueValue = name;
                    }

                    nameable.Name = uniqueName;
                }

                uniqueNames.Add(name);
            }
        }

        private static Tuple<string, int> GenerateUniqueNameForList(string filter, bool ignoreCase, HashSet<string> uniqueNames)
        {
            String uniqueName;
            int id = 1;

            do
            {
                uniqueName = String.Format(filter, id++);
            } while (uniqueNames.Contains(uniqueName, new NameComparer(ignoreCase)));

            return new Tuple<string, int>(uniqueName, id);
        }

        private class NameComparer : IEqualityComparer<string>
        {
            private readonly bool ignoreCase;

            public NameComparer(bool ignoreCase)
            {
                this.ignoreCase = ignoreCase;
            }

            public bool Equals(string x, string y)
            {
                if (x == null && y != null)
                {
                    return false;
                }
                return ignoreCase ? x.Equals(y, StringComparison.CurrentCultureIgnoreCase) : x.Equals(y);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}