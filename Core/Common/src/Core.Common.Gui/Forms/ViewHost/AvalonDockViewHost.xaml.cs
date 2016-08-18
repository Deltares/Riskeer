﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Core.Common.Controls.Views;
using Core.Common.Utils.Drawing;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Common.Gui.Forms.ViewHost
{
    /// <summary>
    /// Implementation of a view host based on AvalonDock.
    /// </summary>
    public partial class AvalonDockViewHost : IViewHost
    {
        public event EventHandler<EventArgs> ActiveDocumentViewChanging;
        public event EventHandler<EventArgs> ActiveDocumentViewChanged;
        public event EventHandler<ViewChangeEventArgs> ActiveViewChanged;
        public event EventHandler<EventArgs> ViewClosed;

        private readonly List<IView> toolViews;
        private readonly List<IView> documentViews;
        private readonly List<WindowsFormsHost> hostControls;

        private IView focussedView;
        private IView activeDocumentView;
        private bool removingProgrammatically;

        /// <summary>
        /// Creates a new instance of the <see cref="AvalonDockViewHost"/> class.
        /// </summary>
        public AvalonDockViewHost()
        {
            InitializeComponent();

            toolViews = new List<IView>();
            documentViews = new List<IView>();
            hostControls = new List<WindowsFormsHost>();

            dockingManager.ActiveContentChanged += OnActiveContentChanged;
        }

        public IEnumerable<IView> DocumentViews
        {
            get
            {
                return documentViews;
            }
        }

        public IEnumerable<IView> ToolViews
        {
            get
            {
                return toolViews;
            }
        }

        public IView ActiveDocumentView
        {
            get
            {
                return activeDocumentView;
            }
            private set
            {
                if (activeDocumentView == value)
                {
                    return;
                }

                OnActiveDocumentViewChangingEvent();

                activeDocumentView = value;

                OnActiveDocumentViewChangedEvent();
            }
        }

        public void AddDocumentView(IView view)
        {
            var control = view as Control;
            if (control == null)
            {
                return;
            }

            // If the view was already added, just focus it
            if (documentViews.Contains(view) || toolViews.Contains(view))
            {
                SetFocusToView(view);
                return;
            }

            var hostControl = new WindowsFormsHost
            {
                Child = control
            };
            var layoutDocument = new LayoutDocument
            {
                Title = view.Text,
                Content = hostControl
            };

            documentViews.Add(view);
            hostControls.Add(hostControl);
            AddLayoutDocument(layoutDocument);

            SetFocusToView(view);

            layoutDocument.Closing += OnLayoutDocumentClosing;
            layoutDocument.Closed += OnLayoutDocumentClosed;
        }

        public void AddToolView(IView view, ToolViewLocation toolViewLocation)
        {
            var control = view as Control;
            if (control == null)
            {
                return;
            }

            // If the view was already added, just focus it
            if (documentViews.Contains(view) || toolViews.Contains(view))
            {
                SetFocusToView(view);
                return;
            }

            var hostControl = new WindowsFormsHost
            {
                Child = control
            };
            var layoutAnchorable = new LayoutAnchorable
            {
                Content = hostControl,
                Title = view.Text
            };

            toolViews.Add(view);
            hostControls.Add(hostControl);

            AddLayoutAnchorable(layoutAnchorable, toolViewLocation);

            SetFocusToView(view);

            layoutAnchorable.Hiding += OnLayoutAnchorableHiding;
        }

        public void Remove(IView view)
        {
            removingProgrammatically = true;

            if (documentViews.Contains(view))
            {
                if (ActiveDocumentView == view)
                {
                    // Note: When removing the active document view programmatically, always set focus to this
                    // view in order to make the view host behave as expected (in this case AvalonDock will help
                    // us selecting the previous active document view based on active content changes).
                    SetFocusToView(view);
                }

                GetLayoutContent<LayoutDocument>(view).Close();
                UpdateDockingManager();
            }
            else if (toolViews.Contains(view))
            {
                GetLayoutContent<LayoutAnchorable>(view).Hide();
                UpdateDockingManager();
            }

            removingProgrammatically = false;
        }

        public void SetFocusToView(IView view)
        {
            if (documentViews.Contains(view))
            {
                var layoutDocument = GetLayoutContent<LayoutDocument>(view);
                if (!layoutDocument.IsActive)
                {
                    dockingManager.Layout.ActiveContent = layoutDocument;
                }
            }
            else if (toolViews.Contains(view))
            {
                var layoutAnchorable = GetLayoutContent<LayoutAnchorable>(view);
                if (!layoutAnchorable.IsActive)
                {
                    dockingManager.Layout.ActiveContent = layoutAnchorable;
                }
            }

            UpdateDockingManager();
        }

        public void SetImage(IView view, Image image)
        {
            var layoutContent = GetLayoutContent<LayoutContent>(view);
            if (layoutContent != null)
            {
                layoutContent.IconSource = image.AsBitmapImage();
            }
        }

        public void Dispose()
        {
            foreach (var view in documentViews.Concat(toolViews).ToArray())
            {
                Remove(view);
            }
        }

        private void OnActiveDocumentViewChangingEvent()
        {
            if (ActiveDocumentViewChanging != null)
            {
                ActiveDocumentViewChanging(this, new EventArgs());
            }
        }

        private void OnActiveDocumentViewChangedEvent()
        {
            if (ActiveDocumentViewChanged != null)
            {
                ActiveDocumentViewChanged(this, new EventArgs());
            }
        }

        private void OnActiveViewChangedEvent()
        {
            if (ActiveViewChanged != null)
            {
                ActiveViewChanged(this, new ViewChangeEventArgs(GetView(dockingManager.ActiveContent)));
            }
        }

        private void OnViewClosedEvent()
        {
            if (ViewClosed != null)
            {
                ViewClosed(this, new EventArgs());
            }
        }

        private void OnActiveContentChanged(object sender, EventArgs eventArgs)
        {
            // Note: Raise (un)focus related events manually as changing focus from one WindowsFormsHost to another does not raise them
            // (see https://msdn.microsoft.com/en-us/library/ms751797(v=vs.100).aspx#Windows_Presentation_Foundation_Application_Hosting).
            // While doing so:
            // - prevent unfocus actions when removing views programmatically (not necessary and might interfere with AvalonDock's active content change behavior);
            // - prevent circular active content changes (which explains the code structure below).
            if (focussedView != null && !removingProgrammatically)
            {
                dockingManager.ActiveContentChanged -= OnActiveContentChanged;
                var activeContent = dockingManager.ActiveContent;
                ControlHelper.UnfocusActiveControl(focussedView as IContainerControl);
                dockingManager.ActiveContent = activeContent;
                dockingManager.ActiveContentChanged += OnActiveContentChanged;
            }

            focussedView = GetView(dockingManager.ActiveContent);

            if (documentViews.Contains(focussedView))
            {
                ActiveDocumentView = focussedView;
                OnActiveViewChangedEvent();
            }
            else if (toolViews.Contains(focussedView))
            {
                OnActiveViewChangedEvent();
            }
            else if (dockingManager.ActiveContent == null)
            {
                ActiveDocumentView = null;
            }
        }

        private void OnLayoutDocumentClosing(object sender, CancelEventArgs e)
        {
            var layoutDocument = (LayoutDocument) sender;
            var view = GetView(layoutDocument.Content);

            if (ActiveDocumentView == view)
            {
                // Note: When removing the active document view via AvalonDock, always set focus to this
                // view in order to make the view host behave as expected (in this case AvalonDock will help
                // us selecting the previous active document view based on active content changes).
                SetFocusToView(view);
            }
        }

        private void OnLayoutDocumentClosed(object sender, EventArgs e)
        {
            var layoutDocument = (LayoutDocument) sender;

            layoutDocument.Closing -= OnLayoutDocumentClosing;
            layoutDocument.Closed -= OnLayoutDocumentClosed;

            OnViewClosed(GetView(layoutDocument.Content));
        }

        private void OnLayoutAnchorableHiding(object sender, CancelEventArgs e)
        {
            var layoutAnchorable = (LayoutAnchorable) sender;

            layoutAnchorable.Hiding -= OnLayoutAnchorableHiding;

            OnViewClosed(GetView(layoutAnchorable.Content));
        }

        private void OnViewClosed(IView view)
        {
            if (documentViews.Contains(view))
            {
                documentViews.Remove(view);
            }
            else
            {
                toolViews.Remove(view);
            }

            CleanupHostControl(view);

            view.Data = null;
            view.Dispose();

            OnViewClosedEvent();
        }

        /// <summary>
        /// This method can be called in order to get rid of problems caused by AvalonDock's latency.
        /// </summary>
        private void UpdateDockingManager()
        {
            dockingManager.UpdateLayout();
        }

        private void CleanupHostControl(IView view)
        {
            var hostControl = hostControls.First(hc => hc.Child == view);

            hostControl.Child = null; // Prevent views from getting disposed here by clearing the child
            hostControl.Dispose();

            hostControls.Remove(hostControl);
        }

        private T GetLayoutContent<T>(IView view) where T : LayoutContent
        {
            return dockingManager.Layout.Descendents()
                                 .OfType<T>()
                                 .FirstOrDefault(d => GetView(d.Content) == view);
        }

        private void AddLayoutDocument(LayoutDocument layoutDocument)
        {
            layoutDocumentPaneGroup.Descendents()
                                   .OfType<LayoutDocumentPane>()
                                   .First()
                                   .Children.Add(layoutDocument);
        }

        private void AddLayoutAnchorable(LayoutAnchorable layoutAnchorable, ToolViewLocation toolViewLocation)
        {
            var layoutAnchorablePaneGroup = new LayoutAnchorablePaneGroup();

            switch (toolViewLocation)
            {
                case ToolViewLocation.Left:
                    layoutAnchorablePaneGroup = leftLayoutAnchorablePaneGroup;
                    break;
                case ToolViewLocation.Bottom:
                    layoutAnchorablePaneGroup = bottomLayoutAnchorablePaneGroup;
                    break;
                case ToolViewLocation.Right:
                    layoutAnchorablePaneGroup = rightLayoutAnchorablePaneGroup;
                    break;
            }

            layoutAnchorablePaneGroup.Descendents()
                                     .OfType<LayoutAnchorablePane>()
                                     .First()
                                     .Children.Add(layoutAnchorable);
        }

        private static IView GetView(object content)
        {
            var windowsFormsHost = content as WindowsFormsHost;
            return windowsFormsHost != null ? windowsFormsHost.Child as IView : null;
        }
    }
}