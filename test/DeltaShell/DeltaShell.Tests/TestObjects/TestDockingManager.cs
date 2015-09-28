using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DeltaShell.Gui.Forms.ViewManager;

namespace DeltaShell.Tests.TestObjects
{
    /// <summary>
    /// Docking manager for test...does nothing
    /// </summary>
    public class TestDockingManager :IDockingManager
    {
        public event EventHandler<DockTabClosingEventArgs> ViewBarClosing;
        public event EventHandler<ActiveViewChangeEventArgs> ViewActivated;
        public IEnumerable<IView> Views { get; private set; }

        public TestDockingManager()
        {
            Views = new List<IView>();
        }

        public void Add(IView view, ViewLocation location)
        {
        }

        public void Add(IView view, ViewLocation location, ContextMenuStrip containerContextMenuStrip)
        {
        }

        public void Remove(IView view, bool removeTabFromDockingbar)
        {
        }

        public void SetToolTip(IView view, string tooltip)
        {
        }

        public void ActivateView(IView view)
        {
        }

        public event Action<object, MouseEventArgs, IView> ViewSelectionMouseDown;

        public void Dispose()
        {
        }
    }
}