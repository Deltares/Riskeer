﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.GIS.SharpMap.Api.Collections;

using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Plugins.SharpMapGis.Gui.Forms
{
    public partial class MapViewTabControl : UserControl
    {
        public event EventHandler<NotifyCollectionChangedEventArgs> ViewCollectionChanged;
        private readonly DockingManager dockingManager = new DockingManager();
        private readonly IEventedList<IView> views = new EventedList<IView>();
        private readonly IList<WindowsFormsHost> viewHosts = new List<WindowsFormsHost>();

        private bool nested = false;

        public MapViewTabControl()
        {
            InitializeComponent();

            elementHost.Child = dockingManager;
            dockingManager.Layout.RootPanel.Children.Clear();

            dockingManager.DocumentClosed += DockingManagerOnDocumentClosed;
        }

        public IView ActiveView
        {
            get
            {
                var activeContent = dockingManager.Layout.ActiveContent;
                if (activeContent == null)
                {
                    return null;
                }
                var contentIndex = viewHosts.IndexOf((WindowsFormsHost) activeContent.Content);
                return contentIndex == -1 ? null : views[contentIndex];
            }
            set
            {
                if (!views.Contains(value))
                {
                    return;
                }

                var viewHost = viewHosts.First(h => h.Child == value);
                var layoutDocument = GetDocumentFor(viewHost);

                // fast fix - check why it is null!
                if (layoutDocument == null)
                {
                    return;
                }

                if (!layoutDocument.IsActive)
                {
                    dockingManager.Layout.ActiveContent = layoutDocument;
                }

                var control = value as Control;
                if (control != null)
                {
                    control.Focus();
                }
            }
        }

        public IEventedList<IView> ChildViews
        {
            get
            {
                return views;
            }
        }

        public void AddView(IView view)
        {
            var viewHost = new WindowsFormsHost
            {
                Child = (Control) view
            };

            var pane = GetAllDocumentPanes().FirstOrDefault();
            if (pane == null)
            {
                pane = new LayoutDocumentPane();
                dockingManager.Layout.RootPanel.Children.Add(pane);
            }

            pane.Children.Add(new LayoutDocument
            {
                Title = view.Text, IsSelected = true, CanFloat = false, Content = viewHost
            });

            views.Add(view);
            viewHosts.Add(viewHost);
            ActiveView = view;
            OnViewCollectionChanged(NotifyCollectionChangedAction.Add, view);
        }

        public void RemoveView(IView view)
        {
            if (!views.Contains(view))
            {
                return;
            }

            var viewHost = viewHosts[views.IndexOf(view)];

            var layoutDocument = GetDocumentFor(viewHost);
            layoutDocument.Close();

            views.Remove(view);
            viewHosts.Remove(viewHost);
            viewHost.Child = null;
            viewHost.Dispose();

            OnViewCollectionChanged(NotifyCollectionChangedAction.Remove, view);
        }

        private void DockingManagerOnDocumentClosed(object sender, DocumentClosedEventArgs documentClosedEventArgs)
        {
            var viewHost = (WindowsFormsHost) documentClosedEventArgs.Document.Content;
            var view = (IView) viewHost.Child;

            views.Remove(view);
            viewHosts.Remove(viewHost);
            viewHost.Child = null;
            viewHost.Dispose();

            ControlHelper.UnfocusActiveControl(view as IContainerControl, true);
            OnViewCollectionChanged(NotifyCollectionChangedAction.Remove, view);
        }

        private void OnViewCollectionChanged(NotifyCollectionChangedAction action, IView view)
        {
            var control = view as Control;
            if (control != null && action == NotifyCollectionChangedAction.Add)
            {
                control.TextChanged += ControlTextChanged;
            }

            if (control != null && action == NotifyCollectionChangedAction.Remove)
            {
                control.TextChanged -= ControlTextChanged;
            }

            if (ViewCollectionChanged != null)
            {
                ViewCollectionChanged(this, new NotifyCollectionChangedEventArgs(action, view));
            }
        }

        private void ControlTextChanged(object sender, EventArgs e)
        {
            var view = sender as IView;
            if (view == null)
            {
                return;
            }

            var layoutContent = GetDocumentFor(viewHosts[views.IndexOf(view)]);
            if (layoutContent == null)
            {
                return;
            }

            layoutContent.Title = view.Text;
        }

        private IEnumerable<LayoutDocumentPane> GetAllDocumentPanes()
        {
            return dockingManager.Layout.RootPanel.Descendents().OfType<LayoutDocumentPane>();
        }

        private LayoutDocument GetDocumentFor(WindowsFormsHost viewHost)
        {
            return GetAllDocumentPanes().SelectMany(d => d.Descendents()).OfType<LayoutDocument>().FirstOrDefault(d => Equals(d.Content, viewHost));
        }
    }
}