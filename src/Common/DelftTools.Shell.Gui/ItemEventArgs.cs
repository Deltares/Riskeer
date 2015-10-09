using System;

namespace DelftTools.Shell.Gui
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

    public class SelectedItemChangedEventArgs<T> : EventArgs
    {
        public SelectedItemChangedEventArgs(T item)
        {
            Item = item;
        }

        public T Item { get; private set; }
    }
}