

namespace DelftTools.Utils.Collections
{
    public interface INotifyCollectionChange : INotifyCollectionChanging, INotifyCollectionChanged
    {
        /// <summary>
        /// Makes sure that <see cref="INotifyPropertyChange.HasParent"/> property is checked/reset in added/removed child objects.
        /// See also <see cref="DelftTools.Utils.Aop.AggregationAttribute"/>.
        /// TODO: this property must be moved to IEntity. Also it seems to be redundant.
        /// </summary>
        bool HasParentIsCheckedInItems { get; set; }

        // TODO: move to IEventedList! Not relevant for all INotifyCollectionChange
        bool SkipChildItemEventBubbling { get; set; }
    }
}