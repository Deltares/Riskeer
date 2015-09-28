namespace DelftTools.Utils.Collections
{
    public interface INotifyCollectionChanging
    {
        event NotifyCollectionChangingEventHandler CollectionChanging;
    }
}