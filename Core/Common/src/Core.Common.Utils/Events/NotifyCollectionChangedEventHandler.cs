namespace Core.Common.Utils.Events
{
    /// <summary>
    /// Defines the event delegate for <see cref="INotifyCollectionChanged"/>.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangeEventArgs"/> instance containing the event data.</param>
    public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangeEventArgs e);
}