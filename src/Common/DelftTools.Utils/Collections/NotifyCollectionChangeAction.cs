namespace DelftTools.Utils.Collections
{
    /// <summary>
    /// Action for changes to a collection such as add, remove.
    /// </summary>
    public enum NotifyCollectionChangeAction
    {
        Add,
        Remove,
        Replace,
        Reset // HACK, TODO: remove this, used only in filtered Functions, move to a separate event in filtered function
    }
}