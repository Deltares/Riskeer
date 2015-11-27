using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewManager;

namespace Core.Common.Test.TestObjects
{
    /// <summary>
    /// Docking manager for test...does nothing
    /// </summary>
    public class TestDockingManager : IDockingManager
    {
// Required by interface, but not used (yet)
#pragma warning disable 67
        public event EventHandler<DockTabClosingEventArgs> ViewBarClosing;
        public event EventHandler<ActiveViewChangeEventArgs> ViewActivated;

        public event Action<object, MouseEventArgs, IView> ViewSelectionMouseDown;
#pragma warning restore 67

        public TestDockingManager()
        {
            Views = new List<IView>();
        }

        public IEnumerable<IView> Views { get; private set; }

        public void Add(IView view, ViewLocation location) {}

        public void Remove(IView view, bool removeTabFromDockingbar) {}

        public void SetToolTip(IView view, string tooltip) {}

        public void ActivateView(IView view) {}

        public void Dispose() {}
    }
}