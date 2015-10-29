namespace Core.Common.Utils.Collections
{
    public interface INotifyCollectionChanging
    {
        event NotifyCollectionChangingEventHandler CollectionChanging;
    }
}