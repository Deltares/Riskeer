using System;

namespace Core.Common.Gui
{
    /// <summary>
    /// Used by IGui.SelectionChanged.
    /// </summary>
    public class SelectedItemChangedEventArgs : EventArgs
    {
        public SelectedItemChangedEventArgs(object item)
        {
            Item = item;
        }

        public object Item { get; private set; }
    }
}