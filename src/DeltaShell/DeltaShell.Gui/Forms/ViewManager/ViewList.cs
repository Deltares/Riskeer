using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.Utils.Collections;
using DeltaShell.Gui.Properties;
using log4net;

namespace DeltaShell.Gui.Forms.ViewManager
{
    /// <summary>
    /// ViewList manages all view in a given dock sites.
    /// </summary>
    public class ViewList : IViewList
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ViewList));
        private readonly ViewLocation? defaultLocation;
        private readonly IDockingManager dockingManager;
        private readonly IList<IView> views;
        private ViewSelectionMouseController viewSelectionMouseController;
        private IView activeView;
        private bool clearing; // used to skip view activation when it is not necessary

        private readonly IDictionary<IView, NotifyCollectionChangedEventHandler> childViewSubscribtionLookup =
            new Dictionary<IView, NotifyCollectionChangedEventHandler>();

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

        public void Dispose()
        {
            dockingManager.ViewBarClosing -= DockingManagerViewBarClosing;
            dockingManager.ViewActivated -= DockingManagerViewActivated;

            dockingManager.Dispose();

            childViewSubscribtionLookup.Clear();

            Gui = null;
        }

        public event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanging;

        public event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event NotifyCollectionChangingEventHandler CollectionChanging;

        public event NotifyCollectionChangedEventHandler ChildViewChanged;

        bool INotifyCollectionChange.HasParentIsCheckedInItems { get; set; }

        public IGui Gui { get; set; }

        public bool SkipChildItemEventBubbling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DoNotDisposeViewsOnRemove.
        /// </summary>
        public static bool DoNotDisposeViewsOnRemove { get; set; }

        public IView ActiveView
        {
            get { return activeView; }
            set { ActivateView(value); }
        }

        public IEnumerable<IView> AllViews
        {
            get { return FindViewsRecursive<IView>(views); }
        }

        public int Count
        {
            get { return views.Count; }
        }

        public bool IgnoreActivation { get; set; }

        public bool IsReadOnly
        {
            get { return views.IsReadOnly; }
        }

        public Action<IView> UpdateViewNameAction { get; set; }

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

        public void Add(IView view, ViewLocation viewLocation)
        {
            Insert(Count, view, viewLocation);
        }

        public void Add(IView item)
        {
            Insert(Count, item);
        }

        public void AddRange(IEnumerable<IView> enumerable)
        {
            foreach (var v in enumerable)
            {
                Add(v);
            }
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

            FireCollectionChangedEvent(NotifyCollectionChangeAction.Reset, -1, null);
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

        public IEnumerable<T> GetActiveViews<T>() where T : class, IView
        {
            return FindViewsRecursive<T>(new[] { ActiveView });
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
                    Resources.ViewList_Insert_No_default_location_specified__Cannot_add_a_view_without_location_parameter_);
            }

            Insert(index, view, (ViewLocation)defaultLocation);
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

        /// <summary>
        /// The resume all view updates.
        /// </summary>
        public void ResumeAllViewUpdates()
        {
            foreach (var view in AllViews.OfType<ISuspendibleView>())
            {
                view.ResumeUpdates();
            }
        }

        public void SetTooltip(IView view, string tooltip)
        {
            dockingManager.SetToolTip(view, tooltip);
        }

        /// <summary>
        /// The suspend all view updates.
        /// </summary>
        public void SuspendAllViewUpdates()
        {
            foreach (var view in AllViews.OfType<ISuspendibleView>())
            {
                view.SuspendUpdates();
            }
        }

        public void EnableTabContextMenus()
        {
            viewSelectionMouseController = new ViewSelectionMouseController(dockingManager, this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return views.GetEnumerator();
        }

        public IEnumerable<T> FindViewsRecursive<T>(IEnumerable<IView> views) where T : class, IView
        {
            foreach (var view in views)
            {
                if (view is T)
                {
                    yield return (T)view;
                }

                var compositeView = view as ICompositeView;
                if (compositeView == null) continue;

                foreach (var childView in FindViewsRecursive<T>(compositeView.ChildViews))
                {
                    yield return childView;
                }
            }
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

            if (activeView == view && (activeView != null && ((Control)activeView).Visible))
            {
                if (activeView is Control)
                {
                    ((Control)activeView).Focus();
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

            if (Gui != null && Gui.Plugins != null)
            {
                Gui.Plugins.ForEach(p => p.OnActiveViewChanged(view));
            }
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
                throw new InvalidOperationException(Resources.ViewList_Close_View_is_being_closed_while_it_is_being_opened_);
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

            FireCollectionChangingEvent(NotifyCollectionChangeAction.Remove, oldIndex, view);

            views.Remove(view);

            FireCollectionChangedEvent(NotifyCollectionChangeAction.Remove, oldIndex, view);
            
            var compositeView = view as ICompositeView;
            if (compositeView != null)
            {
                compositeView.ChildViews.CollectionChanged -= childViewSubscribtionLookup[view];
                childViewSubscribtionLookup.Remove(view);
            }

            if (Gui != null && Gui.Plugins != null)
            {
                Gui.Plugins.ForEach(p => p.OnViewRemoved(view));
            }

            // remove from docking manager
            dockingManager.Remove(view, removeTabFromDockingManager);

            ForceViewCleanup(view);
        }

        private static void ForceViewCleanup(IView view)
        {
            // allow pluginguis to clean up their shit.
            view.Data = null; // reset data for view

            // deep clean-up
            var compositeView = view as ICompositeView;
            if (compositeView != null)
            {
                foreach (var childView in compositeView.ChildViews)
                {
                    if (childView == null)
                    {
                        Log.WarnFormat(Resources.ViewList_ForceViewCleanup_Unexpected_behaviour_from_composite_view__child_view_is_null__parent_view___0_, view);
                    }
                    else
                    {
                        ForceViewCleanup(childView);
                    }
                }
            }

            if (!DoNotDisposeViewsOnRemove)
            {
                // performance improvement.
                view.Dispose(); // get rid of view.
            }
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
            Log.DebugFormat(Resources.ViewList_DockingManagerViewBarClosing_Closing_view___0_, e.View);
            Close(e.View, false, true);
        }

        private void FireActiveViewChangingEvent(IView oldView, IView newView)
        {
            if (ActiveViewChanging != null)
            {
                ActiveViewChanging(this, new ActiveViewChangeEventArgs { View = newView, OldView = oldView });
            }
        }

        private void FireActiveViewChangedEvent(IView oldView)
        {
            if (ActiveViewChanged != null)
            {
                ActiveViewChanged(this, new ActiveViewChangeEventArgs {View = ActiveView, OldView = oldView});
            }
        }

        private void FireCollectionChangedEvent(NotifyCollectionChangeAction notifyCollectionChangeAction, int index, IView item)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangingEventArgs(notifyCollectionChangeAction, item, index, -1));
            }
        }

        private void FireCollectionChangingEvent(NotifyCollectionChangeAction action, int index, IView view)
        {
            if (CollectionChanging != null)
            {
                CollectionChanging(this, new NotifyCollectionChangingEventArgs(action, view, index, -1));
            }
        }

        private void FireChildViewChangedEvent(NotifyCollectionChangingEventArgs args, IView view)
        {
            if (ChildViewChanged != null)
            {
                ChildViewChanged(this, new NotifyCollectionChangingEventArgs(args.Action, view, args.Index, -1));
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

            FireCollectionChangingEvent(NotifyCollectionChangeAction.Add, index, view);

            views.Insert(index, view);

            dockingManager.Add(view, viewLocation);

            FireCollectionChangedEvent(NotifyCollectionChangeAction.Add, index, view);

            var compositeView = view as ICompositeView;
            if (compositeView != null)
            {
                childViewSubscribtionLookup[view] = (sender, args) => FireChildViewChangedEvent(args, view);
                compositeView.ChildViews.CollectionChanged += childViewSubscribtionLookup[view];
            }

            ActiveView = view;

            if (Gui != null && Gui.Plugins != null)
            {
                Gui.Plugins.ForEach(p => p.OnViewAdded(view));
            }
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
    }
}