using System;
using System.Collections.Generic;

namespace DelftTools.Controls
{
    public interface IMenuItem : IMenuItemCollection<IMenuItem>
    {
        /// <summary>
        /// Name is the unique identifier for a menuitem
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// Text is the visible text of a menuitem
        /// </summary>
        string Text { get; set; }

        string Tooltip { get; set; }

        string Category { get; set; }

        string Shortcut { get; set; }

        /// <summary>
        /// Get or set the enabled status of the menu. Disabled items are grayed
        /// </summary>
        bool Enabled { get; set; }
        
        /// <summary>
        /// Defines view type which will be used to show this menuitem when view is active. 
        /// Menuitem is always active if this property is null.
        /// </summary>
        IList<Type> ActiveForViews { get; }

        ICommand Command { get; set; }
    }
}