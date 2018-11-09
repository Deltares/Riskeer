// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Drawing;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Common.Gui.Forms.ViewHost
{
    /// <summary>
    /// Implementation of a view host based on AvalonDock.
    /// </summary>
    public partial class AvalonDockViewHost : IViewHost
    {
        private readonly List<IView> toolViews;
        private readonly List<IView> documentViews;
        private readonly List<WindowsFormsHost> hostControls;

        private IView activeView;
        private IView activeDocumentView;

        public event EventHandler<EventArgs> ActiveDocumentViewChanging;
        public event EventHandler<EventArgs> ActiveDocumentViewChanged;
        public event EventHandler<ViewChangeEventArgs> ActiveViewChanged;
        public event EventHandler<ViewChangeEventArgs> ViewOpened;
        public event EventHandler<ViewChangeEventArgs> ViewBroughtToFront;
        public event EventHandler<ViewChangeEventArgs> ViewClosed;

        /// <summary>
        /// Creates a new instance of the <see cref="AvalonDockViewHost"/> class.
        /// </summary>
        public AvalonDockViewHost()
        {
            InitializeComponent();
            DummyPanelA.Hide();
            DummyPanelB.Hide();

            toolViews = new List<IView>();
            documentViews = new List<IView>();
            hostControls = new List<WindowsFormsHost>();

            DockingManager.ActiveContentChanged += OnActiveContentChanged;
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

                OnActiveDocumentViewChanging();

                activeDocumentView = value;

                OnActiveDocumentViewChanged();
            }
        }

        public void AddDocumentView(IView view)
        {
            var control = view as Control;
            if (control == null)
            {
                return;
            }

            // If the view was already added, just bring it to front
            if (documentViews.Contains(view) || toolViews.Contains(view))
            {
                BringToFront(view);
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

            PerformWithoutChangingActiveContent(() => AddLayoutDocument(layoutDocument));
            BringToFront(layoutDocument);

            documentViews.Add(view);
            hostControls.Add(hostControl);
            ActiveDocumentView = view;
            layoutDocument.Closed += OnLayoutDocumentClosed;

            OnViewOpened(view);
        }

        public void AddToolView(IView view, ToolViewLocation toolViewLocation)
        {
            var control = view as Control;
            if (control == null)
            {
                return;
            }

            // If the view was already added, just bring it to front
            if (documentViews.Contains(view) || toolViews.Contains(view))
            {
                BringToFront(view);
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

            PerformWithoutChangingActiveContent(() => AddLayoutAnchorable(layoutAnchorable, toolViewLocation));

            BringToFront(layoutAnchorable);

            toolViews.Add(view);
            hostControls.Add(hostControl);

            layoutAnchorable.Hiding += OnLayoutAnchorableHiding;
            layoutAnchorable.Closing += OnLayoutAnchorableClosing;

            OnViewOpened(view);
        }

        public void Remove(IView view)
        {
            if (documentViews.Contains(view))
            {
                var layoutDocument = GetLayoutContent<LayoutDocument>(view);

                PerformWithoutChangingActiveContent(() =>
                {
                    layoutDocument.Close();
                    UpdateDockingManager();
                });

                if (ReferenceEquals(ActiveDocumentView, view))
                {
                    ActiveDocumentView = null;
                }
            }
            else if (toolViews.Contains(view))
            {
                var layoutAnchorable = GetLayoutContent<LayoutAnchorable>(view);

                PerformWithoutChangingActiveContent(() =>
                {
                    layoutAnchorable.Hide();
                    UpdateDockingManager();
                });
            }
        }

        public void BringToFront(IView view)
        {
            LayoutContent layoutContent = documentViews.Contains(view)
                                              ? (LayoutContent) GetLayoutContent<LayoutDocument>(view)
                                              : toolViews.Contains(view)
                                                  ? GetLayoutContent<LayoutAnchorable>(view)
                                                  : null;

            BringToFront(layoutContent);

            OnViewBroughtToFront(view);
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            foreach (IView view in documentViews.Concat(toolViews).ToArray())
            {
                Remove(view);
            }

            DockingManager.ActiveContent = null;
        }

        private void BringToFront(LayoutContent layoutContent)
        {
            PerformWithoutChangingActiveContent(() =>
            {
                if (layoutContent != null && !layoutContent.IsActive)
                {
                    layoutContent.IsActive = true;

                    UpdateDockingManager();
                }
            });
        }

        private void OnActiveDocumentViewChanging()
        {
            ActiveDocumentViewChanging?.Invoke(this, new EventArgs());
        }

        private void OnActiveDocumentViewChanged()
        {
            ActiveDocumentViewChanged?.Invoke(this, new EventArgs());
        }

        private void OnActiveViewChanged()
        {
            ActiveViewChanged?.Invoke(this, new ViewChangeEventArgs(GetView(DockingManager.ActiveContent)));
        }

        private void OnViewOpened(IView view)
        {
            ViewOpened?.Invoke(this, new ViewChangeEventArgs(view));
        }

        private void OnViewBroughtToFront(IView view)
        {
            ViewBroughtToFront?.Invoke(this, new ViewChangeEventArgs(view));
        }

        private void OnViewClosed(IView view)
        {
            ViewClosed?.Invoke(this, new ViewChangeEventArgs(view));
        }

        private void OnActiveContentChanged(object sender, EventArgs eventArgs)
        {
            UnfocusActiveView();

            activeView = GetView(DockingManager.ActiveContent);

            if (documentViews.Contains(activeView))
            {
                ActiveDocumentView = activeView;
            }
            else if (DockingManager.ActiveContent == null)
            {
                ActiveDocumentView = null;
            }

            OnActiveViewChanged();
        }

        /// <summary>
        /// Performs unfocus action for the current active view.
        /// </summary>
        /// <remarks>
        /// Raising unfocus events manually is necessary when changing focus from one WindowsFormsHost to another (also see
        /// https://msdn.microsoft.com/en-us/library/ms751797(v=vs.100).aspx#Windows_Presentation_Foundation_Application_Hosting).
        ///</remarks>
        private void UnfocusActiveView()
        {
            var containerControl = activeView as IContainerControl;
            if (containerControl == null)
            {
                return;
            }

            while (containerControl.ActiveControl is IContainerControl)
            {
                containerControl = (IContainerControl) containerControl.ActiveControl;
            }

            PerformWithoutChangingActiveContent(() => containerControl.ActiveControl = null);
        }

        private void OnLayoutDocumentClosed(object sender, EventArgs e)
        {
            var layoutDocument = (LayoutDocument) sender;

            layoutDocument.Closed -= OnLayoutDocumentClosed;

            CloseView(GetView(layoutDocument.Content));
        }

        private static void OnLayoutAnchorableClosing(object sender, CancelEventArgs eventArgs)
        {
            var layoutAnchorable = (LayoutAnchorable) sender;

            layoutAnchorable.Hide();

            eventArgs.Cancel = true;
        }

        private void OnLayoutAnchorableHiding(object sender, EventArgs e)
        {
            var layoutAnchorable = (LayoutAnchorable) sender;

            layoutAnchorable.Hiding -= OnLayoutAnchorableHiding;
            layoutAnchorable.Closing -= OnLayoutAnchorableClosing;

            CloseView(GetView(layoutAnchorable.Content));
        }

        private void CloseView(IView view)
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

            OnViewClosed(view);
        }

        private void PerformWithoutChangingActiveContent(Action actionToPerform)
        {
            DockingManager.ActiveContentChanged -= OnActiveContentChanged;
            object currentActiveContent = DockingManager.ActiveContent;

            actionToPerform();

            DockingManager.ActiveContent = currentActiveContent;
            DockingManager.ActiveContentChanged += OnActiveContentChanged;
        }

        /// <summary>
        /// This method can be called in order to get rid of problems caused by AvalonDock's latency.
        /// </summary>
        private void UpdateDockingManager()
        {
            DockingManager.UpdateLayout();
        }

        private void CleanupHostControl(IView view)
        {
            WindowsFormsHost hostControl = hostControls.First(hc => hc.Child == view);

            hostControl.Child = null; // Prevent views from getting disposed here by clearing the child
            hostControl.Dispose();

            hostControls.Remove(hostControl);
        }

        private T GetLayoutContent<T>(IView view) where T : LayoutContent
        {
            return DockingManager.Layout.Descendents()
                                 .OfType<T>()
                                 .FirstOrDefault(d => GetView(d.Content) == view);
        }

        private void AddLayoutDocument(LayoutDocument layoutDocument)
        {
            LayoutDocumentPaneGroup.Descendents()
                                   .OfType<LayoutDocumentPane>()
                                   .First()
                                   .Children.Add(layoutDocument);
        }

        private void AddLayoutAnchorable(LayoutAnchorable layoutAnchorable, ToolViewLocation toolViewLocation)
        {
            LayoutAnchorablePaneGroup layoutAnchorablePaneGroup;
            if (!Enum.IsDefined(typeof(ToolViewLocation), toolViewLocation))
            {
                throw new InvalidEnumArgumentException(nameof(toolViewLocation), (int) toolViewLocation, typeof(ToolViewLocation));
            }

            switch (toolViewLocation)
            {
                case ToolViewLocation.Left:
                    if (LeftLayoutAnchorablePaneGroup.Parent == null)
                    {
                        LeftLayoutAnchorablePaneGroup.Children.Add(new LayoutAnchorablePane());
                        LeftRightLayoutTarget.Children.Insert(0, LeftLayoutAnchorablePaneGroup);
                    }

                    layoutAnchorablePaneGroup = LeftLayoutAnchorablePaneGroup;
                    break;
                case ToolViewLocation.Bottom:
                    if (BottomLayoutAnchorablePaneGroup.Parent == null)
                    {
                        BottomLayoutAnchorablePaneGroup.Children.Add(new LayoutAnchorablePane());
                        BottomLayoutTarget.Children.Add(BottomLayoutAnchorablePaneGroup);
                    }

                    layoutAnchorablePaneGroup = BottomLayoutAnchorablePaneGroup;
                    break;
                case ToolViewLocation.Right:
                    if (RightLayoutAnchorablePaneGroup.Parent == null)
                    {
                        RightLayoutAnchorablePaneGroup.Children.Add(new LayoutAnchorablePane());
                        LeftRightLayoutTarget.Children.Add(RightLayoutAnchorablePaneGroup);
                    }

                    layoutAnchorablePaneGroup = RightLayoutAnchorablePaneGroup;
                    break;
                default:
                    throw new NotSupportedException();
            }

            layoutAnchorablePaneGroup.Descendents()
                                     .OfType<LayoutAnchorablePane>()
                                     .First()
                                     .Children.Add(layoutAnchorable);
        }

        private static IView GetView(object content)
        {
            var windowsFormsHost = content as WindowsFormsHost;
            return windowsFormsHost?.Child as IView;
        }
    }
}