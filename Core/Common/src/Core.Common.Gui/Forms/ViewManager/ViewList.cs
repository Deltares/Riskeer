using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Events;
using log4net;

namespace Core.Common.Gui.Forms.ViewManager
{
    /// <summary>
    /// ViewList manages all view in a given dock sites.
    /// </summary>
    public class ViewList : IViewList
    {
        public event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanging;

        public event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private static readonly ILog Log = LogManager.GetLogger(typeof(ViewList));
        private readonly ViewLocation? defaultLocation;
        private readonly IDockingManager dockingManager;
        private readonly IList<IView> views;

        private IView activeView;
        private bool clearing; // used to skip view activation when it is not necessary

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewList"/> class. 
        /// Instantiates a view manager
        /// </summary>
        /// <param name="dockingManager">Docking manager to provide adding removing windows</param>
        /// <param name="defaultLocation">Location used if a view is added without a location parameter</param>
        public ViewList(IDockingManager dockingManager, ViewLocation? defaultLocation)
        {
            this.dockingManager = dockingManager;
            this.defaultLocation = defaultLocation;
            views = new List<IView>();

            this.dockingManager.ViewBarClosing += DockingManagerViewBarClosing;
            this.dockingManager.ViewActivated += DockingManagerViewActivated;
        }

        public IView this[int index]
        {
            get
            {
                return views[index];
            }

            set
            {
                // don't just replace the view..we want contexts to kick in etc..so we do a remove / insert at index
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public Action<IView> UpdateViewNameAction { get; set; }

        public IView ActiveView
        {
            get
            {
                return activeView;
            }
            set
            {
                ActivateView(value);
            }
        }

        public IEnumerable<IView> AllViews
        {
            get
            {
                return views;
            }
        }

        public int Count
        {
            get
            {
                return views.Count;
            }
        }

        public bool IgnoreActivation { get; set; }

        public bool IsReadOnly
        {
            get
            {
                return views.IsReadOnly;
            }
        }

        public void EnableTabContextMenus()
        {
            new ViewSelectionMouseController(dockingManager, this);
        }

        // bug in Fluent ribbon (views removed during load layout are not cleared - no events), synchronize them manually
        public void SynchronizeViews(IView[] openedViews)
        {
            foreach (var view in views.ToArray())
            {
                if (!openedViews.Contains(view))
                {
                    views.Remove(view);
                }
            }
        }

        public void AddRange(IEnumerable<IView> enumerable)
        {
            foreach (var v in enumerable)
            {
                Add(v);
            }
        }

        public void SetImage(IView view, Image image)
        {
            dockingManager.SetImage(view, image);
        }

        public void Dispose()
        {
            dockingManager.ViewBarClosing -= DockingManagerViewBarClosing;
            dockingManager.ViewActivated -= DockingManagerViewActivated;

            dockingManager.Dispose();
        }

        public void Add(IView view, ViewLocation viewLocation)
        {
            Insert(Count, view, viewLocation);
        }

        public void Add(IView item)
        {
            Insert(Count, item);
        }

        public void Clear()
        {
            clearing = true;
            while (Count > 0)
            {
                Close(views[0], true, false);
            }

            clearing = false;
            ActiveView = null;

            FireCollectionChangedEvent(NotifyCollectionChangeEventArgs.CreateCollectionResetArgs());
        }

        public void Clear(IView viewToKeep)
        {
            clearing = true;
            var viewToKeepIndex = views.IndexOf(viewToKeep);

            for (int i = views.Count - 1; i >= 0; i--)
            {
                if (i != viewToKeepIndex)
                {
                    Close(views[i], true, false);
                }
            }

            clearing = false;

            ActiveView = viewToKeep;
        }

        public bool Contains(IView item)
        {
            return views.Contains(item);
        }

        public void CopyTo(IView[] array, int arrayIndex)
        {
            views.CopyTo(array, arrayIndex);
        }

        public void UpdateViewName(IView view)
        {
            if (UpdateViewNameAction != null)
            {
                UpdateViewNameAction(view);
            }
        }

        public IEnumerator<IView> GetEnumerator()
        {
            return views.GetEnumerator();
        }

        public int IndexOf(IView item)
        {
            return views.IndexOf(item);
        }

        public void Insert(int index, IView view)
        {
            if (defaultLocation == null)
            {
                throw new InvalidOperationException(
                    Resources.ViewList_Insert_No_default_location_specified_Cannot_add_a_view_without_location_parameter_);
            }

            Insert(index, view, (ViewLocation) defaultLocation);
        }

        public bool Remove(IView view)
        {
            Close(view, true, true);
            return true;
        }

        public void RemoveAt(int index)
        {
            Remove(views[index]);
        }

        public void SetTooltip(IView view, string tooltip)
        {
            dockingManager.SetToolTip(view, tooltip);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return views.GetEnumerator();
        }

        private void ActivateView(IView view)
        {
            if (clearing)
            {
                return;
            }

            var disposableView = view as Control;
            if ((disposableView != null) && disposableView.IsDisposed)
            {
                return; // skip view activation when it is disposed (happens e.g. when closing app)
            }

            if (IgnoreActivation)
            {
                return;
            }

            if (activeView == view && (activeView != null && ((Control) activeView).Visible))
            {
                if (activeView is Control)
                {
                    ((Control) activeView).Focus();
                }

                return;
            }

            var oldView = activeView;

            FireActiveViewChangingEvent(oldView, view);

            activeView = view;

            dockingManager.ActivateView(view);

            if (view != null)
            {
                if (!Contains(view))
                {
                    Log.Debug(Resources.ViewList_ActivateView_Item_not_found_in_list_of_views);
                    return;
                }
            }

            var activeControl = activeView as Control;
            if (activeControl != null)
            {
                activeControl.Focus();
            }

            FireActiveViewChangedEvent(oldView);
        }

        /// <summary>
        /// Set the active view to previous view in the list (if any). Or sets activeview to null if no other view available
        /// </summary>
        private void ChangeActiveView()
        {
            var activeViewIndex = views.IndexOf(activeView);

            // set active view to next view
            if (Count > 1)
            {
                activeViewIndex = activeViewIndex > 0
                                      ? Math.Max(0, activeViewIndex - 1)
                                      : 1;

                ActiveView = views[activeViewIndex];
            }
            else
            {
                //Cannot reproduce, but sometimes it doesn't clear ActiveView (and then crashes) because count==0 (iso 1). 
                //So adjusted the check for 'stability'. Couldn't find the underlying issue.
                ActiveView = null;
            }
        }

        private void Close(IView view, bool removeTabFromDockingManager, bool activateNextView)
        {
            if (ViewResolver.IsViewOpening)
            {
                throw new InvalidOperationException(Resources.ViewList_Close_View_is_being_closed_while_it_is_being_opened);
            }

            if (!Contains(view))
            {
                return; // small optimization
            }

            if (activateNextView)
            {
                if (ActiveView == view)
                {
                    ChangeActiveView();
                }
            }

            int oldIndex = views.IndexOf(view);

            views.Remove(view);

            FireCollectionChangedEvent(NotifyCollectionChangeEventArgs.CreateCollectionRemoveArgs(view, oldIndex));

            // remove from docking manager
            dockingManager.Remove(view, removeTabFromDockingManager);

            ForceViewCleanup(view);
        }

        private static void ForceViewCleanup(IView view)
        {
            view.Data = null; // reset data for view

            view.Dispose(); // get rid of view.
        }

        private void DockingManagerViewActivated(object sender, ActiveViewChangeEventArgs e)
        {
            if (Count == 0)
            {
                return;
            }

            ActiveView = e.View;
        }

        private void DockingManagerViewBarClosing(object sender, DockTabClosingEventArgs e)
        {
            Log.DebugFormat(Resources.ViewList_DockingManagerViewBarClosing_Closing_view_0_, e.View);
            Close(e.View, false, true);
        }

        private void FireActiveViewChangingEvent(IView oldView, IView newView)
        {
            if (ActiveViewChanging != null)
            {
                ActiveViewChanging(this, new ActiveViewChangeEventArgs
                {
                    View = newView, OldView = oldView
                });
            }
        }

        private void FireActiveViewChangedEvent(IView oldView)
        {
            if (ActiveViewChanged != null)
            {
                ActiveViewChanged(this, new ActiveViewChangeEventArgs
                {
                    View = ActiveView, OldView = oldView
                });
            }
        }

        private void FireCollectionChangedEvent(NotifyCollectionChangeEventArgs eventArgs)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, eventArgs);
            }
        }

        private void Insert(int index, IView view, ViewLocation viewLocation)
        {
            // activate view only if it is already added
            if (Contains(view))
            {
                ActiveView = view;
                return;
            }

            if (UpdateViewNameAction != null)
            {
                UpdateViewNameAction(view);
            }

            views.Insert(index, view);

            dockingManager.Add(view, viewLocation);

            FireCollectionChangedEvent(NotifyCollectionChangeEventArgs.CreateCollectionAddArgs(view, index));

            ActiveView = view;
        }
    }
}