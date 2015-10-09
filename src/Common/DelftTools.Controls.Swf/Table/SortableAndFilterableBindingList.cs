using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DelftTools.Utils.Collections.Extensions;
using DelftTools.Utils.Reflection;

namespace DelftTools.Controls.Swf.Table
{
    public class SortableAndFilterableBindingList<T> : BindingList<T>, IBindingListView where T : class
    {
        private enum FilterOption
        {
            Equal,
            NotEqual,
            GreaterThen,
            SmallerThen,
            Contains,
            StartsWith,
            EndsWith
        }

        private readonly IList<T> dataList;
        private bool isSorted;
        private ListSortDirection sortDirection;
        private PropertyDescriptor sortProperty;
        private string filter;
        private List<FilterOperation> filterLookup;

        public SortableAndFilterableBindingList(IList<T> dataList)
        {
            this.dataList = dataList;
            Items.AddRange(dataList);
        }

        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
                filterLookup = CreateFilterLookUp();

                UpdateItems();
            }
        }

        public ListSortDescriptionCollection SortDescriptions { get; private set; }

        public bool SupportsAdvancedSorting
        {
            get
            {
                return true;
            }
        }

        public bool SupportsFiltering
        {
            get
            {
                return true;
            }
        }

        public void ApplySort(ListSortDescriptionCollection sorts) {}

        public void RemoveFilter()
        {
            Filter = null;
        }

        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return sortDirection;
            }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return sortProperty;
            }
        }

        protected override bool IsSortedCore
        {
            get
            {
                return isSorted;
            }
        }

        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

        protected override void RemoveSortCore()
        {
            if (!isSorted)
            {
                return;
            }

            UpdateItems();
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            if (prop.PropertyType.GetInterface("IComparable") == null)
            {
                return;
            }

            sortDirection = direction;
            sortProperty = prop;
            isSorted = true;

            UpdateItems();
        }

        private List<FilterOperation> CreateFilterLookUp()
        {
            var lookUp = new List<FilterOperation>();

            if (string.IsNullOrEmpty(filter))
            {
                return lookUp;
            }

            var propertyFilters = filter.Split(new[]
            {
                " AND "
            }, StringSplitOptions.RemoveEmptyEntries);
            var operators = new[]
            {
                " = ",
                " < ",
                " > ",
                " <> ",
                " Like "
            };

            foreach (var propertyFilter in propertyFilters)
            {
                var filterArguments = propertyFilter.Split(operators, StringSplitOptions.None);

                var operation = operators.First(o => propertyFilter.Contains(o)).Trim(' ');
                lookUp.Add(new FilterOperation(filterArguments[0], operation, filterArguments[1], typeof(T).GetProperty(filterArguments[0]).PropertyType));
            }

            return lookUp;
        }

        private void UpdateItems()
        {
            RaiseListChangedEvents = false;

            var displayItems = string.IsNullOrEmpty(filter)
                                   ? dataList
                                   : dataList.Where(item => filterLookup.All(fo => fo.MatchesFilterCondition(GetPropertyValue(item, fo))));

            displayItems = isSorted
                               ? displayItems.OrderBy(i => i, new PropertyComparer(sortProperty.Name, sortDirection))
                               : displayItems;

            ClearItems();

            Items.AddRange(displayItems);

            RaiseListChangedEvents = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private static IComparable GetPropertyValue(T item, FilterOperation filterOperation)
        {
            var propertyValue = TypeUtils.GetPropertyValue(item, filterOperation.PropertyName);

            var comparable = propertyValue as IComparable;
            if (comparable != null)
            {
                return comparable;
            }

            return propertyValue.ToString();
        }

        private class FilterOperation
        {
            private readonly Type propertyType;
            private readonly FilterOption filterOperation;
            private readonly IComparable filterValue;

            public FilterOperation(string propertyName, string operation, string value, Type propertyType)
            {
                PropertyName = propertyName;
                this.propertyType = propertyType;

                filterValue = GetValue(value);
                filterOperation = GetFilterOption(operation);
            }

            public string PropertyName { get; private set; }

            public bool MatchesFilterCondition(IComparable valueToCheck)
            {
                if (valueToCheck == null)
                {
                    return false;
                }

                switch (filterOperation)
                {
                    case FilterOption.Equal:
                        return valueToCheck.Equals(filterValue);
                    case FilterOption.NotEqual:
                        return !valueToCheck.Equals(filterValue);
                    case FilterOption.GreaterThen:
                        return valueToCheck.CompareTo(filterValue) > 0;
                    case FilterOption.SmallerThen:
                        return valueToCheck.CompareTo(filterValue) < 0;
                    case FilterOption.Contains:
                        if (propertyType != typeof(string))
                        {
                            return false;
                        }
                        var containsValue = ((string) filterValue).Trim('%');
                        return ((string) valueToCheck).Contains(containsValue);
                    case FilterOption.StartsWith:
                        if (propertyType != typeof(string))
                        {
                            return false;
                        }
                        var startsWithValue = ((string) filterValue).TrimEnd('%');
                        return ((string) valueToCheck).StartsWith(startsWithValue);
                    case FilterOption.EndsWith:
                        if (propertyType != typeof(string))
                        {
                            return false;
                        }
                        var endsWithValue = ((string) filterValue).TrimStart('%');
                        return ((string) valueToCheck).EndsWith(endsWithValue);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private FilterOption GetFilterOption(string filterOperation)
            {
                if (filterOperation == "=")
                {
                    return FilterOption.Equal;
                }
                if (filterOperation == "<>")
                {
                    return FilterOption.NotEqual;
                }
                if (filterOperation == "<")
                {
                    return FilterOption.SmallerThen;
                }
                if (filterOperation == ">")
                {
                    return FilterOption.GreaterThen;
                }
                if (filterOperation == "Like")
                {
                    var value = filterValue.ToString();
                    if (value.StartsWith("%") && value.EndsWith("%"))
                    {
                        return FilterOption.Contains;
                    }
                    if (value.StartsWith("%"))
                    {
                        return FilterOption.EndsWith;
                    }
                    if (value.EndsWith("%"))
                    {
                        return FilterOption.StartsWith;
                    }
                }

                throw new ArgumentException("Can't determine operator from " + filterOperation);
            }

            private IComparable GetValue(string value)
            {
                if (propertyType == typeof(string))
                {
                    value = value.Trim('\'');
                }

                if (propertyType == typeof(DateTime))
                {
                    value = value.Trim('#');
                }

                if (propertyType.Implements(typeof(IConvertible)))
                {
                    var convertedValue = Convert.ChangeType(value, propertyType);

                    if (convertedValue is IComparable)
                    {
                        return (IComparable) convertedValue;
                    }

                    return value.Trim('\'');
                }

                return value.Trim('\'');
            }
        }

        private class PropertyComparer : IComparer<T>
        {
            public PropertyComparer(string propName, ListSortDirection direction)
            {
                PropInfo = typeof(T).GetProperty(propName);
                Direction = direction;
            }

            public int Compare(T firstValue, T secondValue)
            {
                return Direction == ListSortDirection.Ascending
                           ? Comparer.Default.Compare(PropInfo.GetValue(firstValue, null), PropInfo.GetValue(secondValue, null))
                           : Comparer.Default.Compare(PropInfo.GetValue(secondValue, null), PropInfo.GetValue(firstValue, null));
            }

            private PropertyInfo PropInfo { get; set; }
            private ListSortDirection Direction { get; set; }
        }
    }
}