namespace DelftTools.Utils.Collections.Generic
{
    /// <summary>
    /// TODO: move to IEnumerableList! No need for a separate interface. Cache?!?
    /// </summary>
    public interface IEnumerableListCache
    {
        INotifyCollectionChange CollectionChangeSource { get; set; }
        INotifyPropertyChange PropertyChangeSource { get; set; }
    }
}