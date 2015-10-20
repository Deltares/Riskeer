using System.Collections.Generic;

namespace DelftTools.Controls
{
    /// <summary>
    /// Extents the IList interface with some extra methods. It adds support for name to search
    /// the collection. This simplifies menu- and toolbar building
    /// see also dotnetbar SubItemsCollection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// TODO: remove it!
    public interface IMenuItemCollection<T> : IList<IMenuItem>
    {

    }
}