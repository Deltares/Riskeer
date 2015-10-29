using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application.Ringtoets.Properties;
using Core.Common.Controls;
using Core.Common.Controls.Swf;
using Core.Common.Gui;
using Core.Common.Utils.Reflection;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Orientation = System.Windows.Controls.Orientation;
using Point = System.Drawing.Point;

namespace Application.Ringtoets.Forms.ViewManager
{
    public class AvalonDockDockingManager : IDockingManager
    {
        public event EventHandler<DockTabClosingEventArgs> ViewBarClosing;

        public event EventHandler<ActiveViewChangeEventArgs> ViewActivated;

        public event Action<object, MouseEventArgs, IView> ViewSelectionMouseDown;
        private static readonly Bitmap LockImage = Resources.lock_edit;

        private readonly DockingManager dockingManager;
        private readonly ViewLocation[] dockingLocations;

        private readonly List<IView> views;
        private IView activeView;

        private readonly List<WindowsFormsHost> hostControls;

        public AvalonDockDockingManager(DockingManager dockingManager, ViewLocation[] dockingLocations)
        {
            this.dockingManager = dockingManager;
            this.dockingLocations = dockingLocations;

            views = new List<IView>();
            hostControls = new List<WindowsFormsHost>();

            this.dockingManager.DocumentClosing += DockingManagerOnDocumentClosing;
            this.dockingManager.MouseDown += DockingManagerMouseDown;
            this.dockingManager.ActiveContentChanged += DockingManagerOnActiveContentChanged;
            this.dockingManager.DocumentContextMenu = null; // don't show avalons own context menu for documents
            this.dockingManager.LostKeyboardFocus += (sender, args) =>
            {
                // Validate activeView when switching to other wpf control like ribbon
                var control = activeView as UserControl;
                if (control != null)
                {
                    control.Validate();
                }
            };
        }

        public IEnumerable<IView> Views
        {
            get
            {
                return views;
            }
        }

        /// <summary>
        /// Called after layout is changed (e.g. loaded).
        /// </summary>
        public void OnLayoutChange()
        {
            var existingContents = dockingManager.Layout.Descendents().OfType<LayoutContent>().ToArray();
            var existingHostControls = existingContents.Select(c => c.Content as WindowsFormsHost);

            foreach (var view in views.ToArray())
            {
                if (existingHostControls.Any(c => c != null && c.Child == view))
                {
                    continue;
                }

                // view removed
                var index = views.IndexOf(view);
                var control = view as Control;
                if (control != null)
                {
                    control.TextChanged -= ControlOnTextChanged;
                }

                view.Dispose();
                view.Data = null;
                views.RemoveAt(index);
                hostControls.RemoveAt(index);

                if (views.Count != hostControls.Count)
                {
                    throw new InvalidOperationException();
                }
            }

            // subscribe to anchorables (this must be much simpler)
            var anchorables = dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().ToArray();

            foreach (var layoutAnchorable in anchorables)
            {
                layoutAnchorable.PropertyChanged -= AnchorableOnPropertyChanged;
                layoutAnchorable.PropertyChanged += AnchorableOnPropertyChanged;
            }
        }

        public void Dispose() {}

        public void Add(IView view, ViewLocation location)
        {
            if (views.Contains(view))
            {
                throw new InvalidOperationException(Resources.AvalonDockDockingManager_Add_View_was_already_added__activate_it_instead_of_add);
            }

            var control = view as Control;
            if (control != null)
            {
                control.TextChanged += ControlOnTextChanged;
            }

            views.Add(view);

            if (dockingLocations.Contains(ViewLocation.Document))
            {
                AddDocumentView(view);
            }
            else
            {
                AddToolView(view, location);
            }

            if (ViewActivated != null)
            {
                ViewActivated(this, new ActiveViewChangeEventArgs
                {
                    View = view
                });
            }
        }

        public void Remove(IView view, bool removeTabFromDockingbar)
        {
            if (removeTabFromDockingbar)
            {
                var layoutContent = GetLayoutContent(view);
                var index = views.IndexOf(view);

                CleanupHostControl(index);

                if (layoutContent != null)
                {
                    layoutContent.Close();
                }
            }
            else
            {
                var index = views.IndexOf(view);
                CleanupHostControl(index);
            }

            var control = view as Control;
            if (control != null)
            {
                control.TextChanged -= ControlOnTextChanged;
            }

            var reusable = view as IReusableView;
            if (reusable != null)
            {
                reusable.LockedChanged -= ReusableLockedChanged;
            }

            views.Remove(view);

            if (views.Count != hostControls.Count)
            {
                throw new InvalidOperationException();
            }

            view.Dispose();

            activeView = GetView(dockingManager.ActiveContent);

            if (ViewActivated != null)
            {
                ViewActivated(this, new ActiveViewChangeEventArgs
                {
                    View = activeView
                });
            }
        }

        public void SetToolTip(IView view, string tooltip)
        {
            var layoutDocument = GetLayoutContent(view);
            if (layoutDocument != null)
            {
                layoutDocument.ToolTip = tooltip;
            }
        }

        public void ActivateView(IView view)
        {
            if (view == null || !views.Contains(view))
            {
                return;
            }

            if (dockingLocations.Contains(ViewLocation.Document))
            {
                var layoutDocument = dockingManager.Layout.Descendents().OfType<LayoutDocument>().FirstOrDefault(a => ContainsView(a, view));

                // fast fix - check why it is null!
                if (layoutDocument == null)
                {
                    return;
                }

                if (!layoutDocument.IsActive)
                {
                    dockingManager.Layout.ActiveContent = layoutDocument;
                }
            }
            else
            {
                var layoutAnchorable = dockingManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(a => ContainsView(a, view));

                // fast fix - check why it is null!
                if (layoutAnchorable == null)
                {
                    return;
                }

                if (layoutAnchorable.IsHidden)
                {
                    layoutAnchorable.Show();
                }
                else if (layoutAnchorable.IsVisible)
                {
                    layoutAnchorable.IsActive = true;
                }
                else
                {
                    throw new NotImplementedException();
                }
                // anchorable.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
            }
        }

        private void DockingManagerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                return; // view already being closed by avalon dock itself
            }

            if (ViewSelectionMouseDown == null)
            {
                return;
            }

            var layoutDocument = GetLayoutDocumentFromUIElement(Mouse.DirectlyOver);
            if (layoutDocument == null)
            {
                return;
            }

            var view = ((WindowsFormsHost) layoutDocument.Content).Child as IView;
            if (view == null)
            {
                return;
            }

            var position = ToViewCoordinate(e, view);
            ViewSelectionMouseDown(view, new MouseEventArgs(GetMouseButtons(e.LeftButton, e.MiddleButton, e.RightButton),
                                                            1, position.X, position.Y, 0), view);
        }

        private static Point ToViewCoordinate(MouseButtonEventArgs e, IView view)
        {
            var screen = e.GetPosition(null);
            return ((Control) view).PointToClient(new Point((int) screen.X, (int) screen.Y));
        }

        private static LayoutDocument GetLayoutDocumentFromUIElement(IInputElement inputElement)
        {
            var frameworkElement = inputElement as FrameworkElement;
            if (frameworkElement != null)
            {
                return frameworkElement.DataContext as LayoutDocument;
            }

            var frameworkContentElement = inputElement as FrameworkContentElement;
            if (frameworkContentElement != null)
            {
                return frameworkContentElement.DataContext as LayoutDocument;
            }

            return null;
        }

        private static MouseButtons GetMouseButtons(MouseButtonState leftButton, MouseButtonState middleButton, MouseButtonState rightButton)
        {
            if (leftButton == MouseButtonState.Pressed)
            {
                return MouseButtons.Left;
            }
            if (middleButton == MouseButtonState.Pressed)
            {
                return MouseButtons.Middle;
            }
            if (rightButton == MouseButtonState.Pressed)
            {
                return MouseButtons.Right;
            }
            return MouseButtons.None;
        }

        private void DockingManagerOnActiveContentChanged(object sender, EventArgs eventArgs)
        {
            var view = GetView(dockingManager.ActiveContent);

            if (view == null)
            {
                return;
            }

            if (view == activeView)
            {
                return;
            }

            if (!views.Contains(view))
            {
                return; // not our view
            }

            ControlHelper.UnfocusActiveControl(activeView as IContainerControl);

            activeView = view;

            if (ViewActivated != null)
            {
                ViewActivated(this, new ActiveViewChangeEventArgs
                {
                    View = view
                });
            }
        }

        private void DockingManagerOnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            var view = ((WindowsFormsHost) e.Document.Content).Child as IView;
            if (view == null)
            {
                return;
            }

            OnCloseView(view, e);
        }

        private void OnCloseView(IView view, CancelEventArgs e)
        {
            if (views.Contains(view))
            {
                if (ViewBarClosing != null)
                {
                    var viewEventArgs = new DockTabClosingEventArgs
                    {
                        View = view
                    };
                    ViewBarClosing(this, viewEventArgs);
                    if (viewEventArgs.Cancel)
                    {
                        e.Cancel = true;
                    }
                    ControlHelper.UnfocusActiveControl(activeView as IContainerControl, true);
                }
            }
        }

        private void AddToolView(IView view, ViewLocation location)
        {
            var control = (Control) view;
            control.Dock = DockStyle.Fill;
            var hostControl = new WindowsFormsHost
            {
                Child = control
            };
            hostControls.Add(hostControl);

            var anchorable = new LayoutAnchorable
            {
                Content = hostControl,
                Title = view.Text,
                ContentId = view.Text
            };

            anchorable.Closing += AnchorableOnClosing;
            anchorable.Hiding += AnchorableOnHiding;
            anchorable.PropertyChanged += AnchorableOnPropertyChanged;

            AnchorSide anchorSide;

            // TODO: this logic is messy - find a better way
            LayoutAnchorablePane anchorablePane;
            if (location.HasFlag(ViewLocation.Left))
            {
                anchorSide = AnchorSide.Left;

                anchorablePane = dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.GetSide() == anchorSide);

                if ((location & ViewLocation.Bottom) == ViewLocation.Bottom)
                {
                    anchorablePane = dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().LastOrDefault(p => p.GetSide() == anchorSide);
                }

                if (anchorablePane == null)
                {
                    var paneGroup = new LayoutAnchorablePaneGroup
                    {
                        Orientation = Orientation.Vertical, DockWidth = new GridLength(220)
                    };
                    anchorablePane = new LayoutAnchorablePane
                    {
                        DockWidth = new GridLength(220.0, GridUnitType.Pixel)
                    };
                    paneGroup.Children.Add(anchorablePane);

                    dockingManager.Layout.RootPanel.Children.Insert(0, paneGroup);
                }
            }
            else if (location.HasFlag(ViewLocation.Right))
            {
                anchorSide = AnchorSide.Right;
                anchorablePane = dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.GetSide() == anchorSide);

                // check if Top or Bottom is specified
                if ((location & ViewLocation.Bottom) == ViewLocation.Bottom)
                {
                    anchorablePane = dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().LastOrDefault(p => p.GetSide() == anchorSide);
                }

                if (anchorablePane == null)
                {
                    var paneGroup = new LayoutAnchorablePaneGroup
                    {
                        Orientation = Orientation.Vertical, DockWidth = new GridLength(220)
                    };
                    anchorablePane = new LayoutAnchorablePane
                    {
                        DockWidth = new GridLength(220.0, GridUnitType.Pixel)
                    };
                    paneGroup.Children.Add(anchorablePane);

                    dockingManager.Layout.RootPanel.Children.Add(paneGroup);
                }
            }
            else if (location.HasFlag(ViewLocation.Bottom))
            {
                anchorSide = AnchorSide.Bottom;
                anchorablePane = dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.GetSide() == anchorSide);

                if (anchorablePane == null)
                {
                    anchorablePane = new LayoutAnchorablePane
                    {
                        DockWidth = new GridLength(220.0, GridUnitType.Pixel)
                    };
                    var subLayoutPanels =
                        dockingManager.Layout.RootPanel.Children.OfType<LayoutPanel>().FirstOrDefault();
                    if (subLayoutPanels != null)
                    {
                        subLayoutPanels.Children.Add(anchorablePane);
                    }
                    else
                    {
                        dockingManager.Layout.RootPanel.Children.Add(anchorablePane);
                    }
                }
            }
            else if (location.HasFlag(ViewLocation.Top))
            {
                anchorSide = AnchorSide.Top;
                anchorablePane = dockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.GetSide() == anchorSide);
            }
            else
            {
                throw new NotImplementedException(string.Format(Resources.AvalonDockDockingManager_AddToolView_View_location__0__is_not_implemented_yet, location));
            }

            if (anchorablePane == null) {}

            anchorablePane.Children.Add(anchorable);

            if (anchorSide == AnchorSide.Bottom || anchorSide == AnchorSide.Top)
            {
                anchorablePane.DockMinHeight = 100;
            }
            else if (anchorSide == AnchorSide.Left || anchorSide == AnchorSide.Right)
            {
                anchorablePane.DockMinWidth = 200;
            }

            dockingManager.Layout.ActiveContent = anchorable;
        }

        private void AnchorableOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var anchorable = sender as LayoutAnchorable;

            if (anchorable != null && e.PropertyName == TypeUtils.GetMemberName<LayoutAnchorable>(o => o.IsHidden) && anchorable.IsHidden)
            {
                AnchorableOnClosing(sender, new CancelEventArgs());

                anchorable.Close();
            }
        }

        private void AnchorableOnHiding(object sender, CancelEventArgs e)
        {
            AnchorableOnClosing(sender, e);
        }

        private void AnchorableOnClosing(object sender, CancelEventArgs e)
        {
            var anchorable = sender as LayoutAnchorable;

            if (anchorable == null)
            {
                return;
            }

            var hostControl = anchorable.Content as WindowsFormsHost;

            if (hostControl == null)
            {
                return;
            }

            var view = hostControl.Child as IView;

            if (view == null)
            {
                return;
            }

            OnCloseView(view, e);
        }

        private void AddDocumentView(IView view)
        {
            var reusable = view as IReusableView;
            if (reusable != null)
            {
                reusable.LockedChanged += ReusableLockedChanged;
            }

            var hostControl = new WindowsFormsHost
            {
                Child = (Control) view
            };
            hostControls.Add(hostControl);

            var layoutDocument = new LayoutDocument
            {
                Title = view.Text,
                Content = hostControl,
                ContentId = view.Text,
                IconSource = GetImage(view)
            };

            var firstDocumentPane = dockingManager.Layout.Descendents().OfType<LayoutDocumentPane>().First();
            firstDocumentPane.Children.Add(layoutDocument);
            dockingManager.Layout.ActiveContent = layoutDocument;
        }

        private static BitmapImage BitmapImageFromBitmap(Image bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            using (var memory = new MemoryStream())
            {
                var bitmapImage = new BitmapImage();
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.DecodePixelHeight = bitmap.Height;
                bitmapImage.DecodePixelWidth = bitmap.Width;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private void CleanupHostControl(int index)
        {
            var hostControl = hostControls[index];
            if (hostControl != null)
            {
                object adapter =
                    typeof(WindowsFormsHost).GetProperty("HostContainerInternal",
                                                         BindingFlags.GetProperty | BindingFlags.NonPublic |
                                                         BindingFlags.Instance).GetValue(hostControl, null);
                var disposable = adapter as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                hostControl.Dispose();
                hostControl.Child = null;
            }
            hostControls.RemoveAt(index);
        }

        private void ReusableLockedChanged(object sender, EventArgs e)
        {
            var view = sender as IView;
            if (view == null)
            {
                return;
            }

            var document = GetLayoutContent(view) as LayoutDocument;
            if (document == null)
            {
                return;
            }

            document.IconSource = GetImage(view);
        }

        private ImageSource GetImage(IView view)
        {
            var reusable = view as IReusableView;
            var isLocked = reusable != null && reusable.Locked;
            return BitmapImageFromBitmap(isLocked ? LockImage : view.Image);
        }

        private LayoutContent GetLayoutContent(IView view)
        {
            var descendents = dockingManager.Layout.Descendents();
            return descendents.OfType<LayoutContent>().FirstOrDefault(d => d.Content is WindowsFormsHost && ((WindowsFormsHost) d.Content).Child == view);
        }

        private static IView GetView(object content)
        {
            var host = content as WindowsFormsHost;
            if (host != null)
            {
                return host.Child as IView;
            }

            return null;
        }

        private static bool ContainsView(LayoutContent content, IView view)
        {
            return content.Content is WindowsFormsHost && ((WindowsFormsHost) content.Content).Child == view;
        }

        private void ControlOnTextChanged(object sender, EventArgs eventArgs)
        {
            var view = sender as IView;
            if (view == null)
            {
                return;
            }

            var layoutContent = GetLayoutContent(view);
            if (layoutContent == null)
            {
                return;
            }

            layoutContent.Title = view.Text;
            layoutContent.IconSource = GetImage(view); //update image aswell
        }
    }
}