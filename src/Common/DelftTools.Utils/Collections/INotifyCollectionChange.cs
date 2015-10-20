namespace DelftTools.Utils.Collections
{
    public interface INotifyCollectionChange : INotifyCollectionChanging, INotifyCollectionChanged
    {
        // TODO: move to IEventedList! Not relevant for all INotifyCollectionChange
        bool SkipChildItemEventBubbling { get; set; }
    }
}