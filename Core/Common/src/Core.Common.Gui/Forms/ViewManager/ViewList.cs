// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
        private static readonly ILog log = LogManager.GetLogger(typeof(ViewList));

        private readonly ViewLocation? defaultLocation;
        private readonly IDockingManager dockingManager;
        private readonly IList<IView> views;

        private IView activeView;

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

        /// <summary>
        /// Gets or sets the action to be used to update the name of the view.
        /// </summary>
        public Action<IView> UpdateViewNameAction { get; set; }

        /// <summary>
        /// Sets up the controller for context menus on docked views.
        /// </summary>
        public void EnableTabContextMenus()
        {
            new ViewSelectionMouseController(dockingManager, this);
        }

        /// <summary>
        /// Removes all views in the list except for those specified.
        /// </summary>
        /// <param name="viewsToKeep">The views to keep.</param>
        public void Remove(IView[] viewsToKeep)
        {
            foreach (var view in views.ToArray())
            {
                if (!viewsToKeep.Contains(view))
                {
                    views.Remove(view);
                }
            }
        }

        /// <summary>
        /// Updates the image used for the docking panel hosting the view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="image">The image.</param>
        public void SetImage(IView view, Image image)
        {
            dockingManager.SetImage(view, image);
        }

        private bool Close(IView view, bool removeTabFromDockingManager, bool activateNextView)
        {
            if (ViewResolver.IsViewOpening)
            {
                throw new InvalidOperationException(Resources.ViewList_Close_View_is_being_closed_while_it_is_being_opened);
            }

            if (!Contains(view))
            {
                return false;
            }

            if (activateNextView && ActiveView == view)
            {
                ChangeActiveViewToPrevious();
            }

            int oldIndex = views.IndexOf(view);

            var succesfullyRemovedView = views.Remove(view);

            FireCollectionChangedEvent(NotifyCollectionChangeEventArgs.CreateCollectionRemoveArgs(view, oldIndex));

            // remove from docking manager
            dockingManager.Remove(view, removeTabFromDockingManager);

            ForceViewCleanup(view);

            return succesfullyRemovedView;
        }

        private void ChangeActiveViewToPrevious()
        {
            var activeViewIndex = views.IndexOf(activeView);

            if (Count > 1)
            {
                var viewIndexToActivate = activeViewIndex > 0 ?
                                              activeViewIndex - 1 :
                                              1; // There is no previous, so use next instead

                ActiveView = views[viewIndexToActivate];
            }
            else
            {
                ActiveView = null;
            }
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
            log.DebugFormat(Resources.ViewList_DockingManagerViewBarClosing_Closing_view_0_, e.View);
            Close(e.View, false, true);
        }

        #region Implementation: IViewList

        public event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanging;

        public event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IView this[int index]
        {
            get
            {
                return views[index];
            }

            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

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
            while (Count > 0)
            {
                Close(views[0], true, false);
            }

            ActiveView = null;

            FireCollectionChangedEvent(NotifyCollectionChangeEventArgs.CreateCollectionResetArgs());
        }

        public void Clear(IView viewToKeep)
        {
            var viewToKeepIndex = views.IndexOf(viewToKeep);

            for (int i = views.Count - 1; i >= 0; i--)
            {
                if (i != viewToKeepIndex)
                {
                    Close(views[i], true, false);
                }
            }

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

            Insert(index, view, (ViewLocation)defaultLocation);
        }

        public bool Remove(IView view)
        {
            return Close(view, true, true);
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
            var disposableView = view as Control;
            if (disposableView != null && disposableView.IsDisposed)
            {
                return; // skip view activation when it is disposed (happens e.g. when closing app)
            }

            if (IgnoreActivation)
            {
                return;
            }

            if (activeView == view && activeView != null && ((Control)activeView).Visible)
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
                    log.Debug(Resources.ViewList_ActivateView_Item_not_found_in_list_of_views);
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

        private void FireActiveViewChangingEvent(IView oldView, IView newView)
        {
            if (ActiveViewChanging != null)
            {
                ActiveViewChanging(this, new ActiveViewChangeEventArgs(newView, oldView));
            }
        }

        private void FireActiveViewChangedEvent(IView oldView)
        {
            if (ActiveViewChanged != null)
            {
                ActiveViewChanged(this, new ActiveViewChangeEventArgs(ActiveView, oldView));
            }
        }

        private void FireCollectionChangedEvent(NotifyCollectionChangeEventArgs eventArgs)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, eventArgs);
            }
        }

        #endregion
    }
}