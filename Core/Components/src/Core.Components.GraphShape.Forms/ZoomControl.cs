// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Core.Components.GraphShape.Forms
{
    /// <summary>
    /// Control that supports zooming and panning its content.
    /// </summary>
    public class ZoomControl : ContentControl
    {
        private const string presenterPartName = "PART_Presenter";
        private const double zoomTolerance = 1e-10;
        private const double minZoom = 0.25;
        private const double maxZoom = 4;
        private const double zoomDeltaMultiplier = 300;
        private const double maxZoomDelta = 4;
        private readonly ScaleTransform scaleTransform = new(1, 1);
        private readonly TranslateTransform translateTransform = new();
        private readonly TransformGroup transformGroup = new();

        private ZoomContentPresenter presenter;
        private bool isPanPending;
        private bool isPanning;
        private Point panStartPosition;
        private Cursor originalCursor;
        private bool isInitialViewSet;
        private bool isInitialZoomToFillQueued;

        /// <summary>
        /// Creates a new instance of <see cref="ZoomControl"/>.
        /// </summary>
        public ZoomControl()
        {
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            PreviewMouseMove += OnPreviewMouseMove;
            PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            LostMouseCapture += OnLostMouseCapture;
            PreviewMouseWheel += OnPreviewMouseWheel;
        }

        /// <summary>
        /// Zooms to and centers the entire measured content within the control, subject to the zoom limits.
        /// </summary>
        /// <remarks>This is done automatically once, as soon as both the content and the control
        /// have a size, unless the view has been changed before.</remarks>
        public void ZoomToFill()
        {
            if (presenter == null || presenter.ContentSize.Width <= 0 || presenter.ContentSize.Height <= 0)
            {
                return;
            }

            ZoomTo(new Rect(new Point(0, 0), presenter.ContentSize));
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            presenter?.ContentSizeChanged -= OnContentSizeChanged;

            presenter = GetTemplateChild(presenterPartName) as ZoomContentPresenter;
            if (presenter == null)
            {
                return;
            }

            presenter.RenderTransform = transformGroup;
            presenter.RenderTransformOrigin = new Point(0, 0);
            presenter.ContentSizeChanged += OnContentSizeChanged;
        }

        private double ZoomFactor => scaleTransform.ScaleX;
        private double TranslateX => translateTransform.X;
        private double TranslateY => translateTransform.Y;

        private Rect ZoomBox
        {
            get
            {
                if (ActualWidth <= 0 || ActualHeight <= 0)
                {
                    return Rect.Empty;
                }

                return new Rect(-TranslateX / ZoomFactor, -TranslateY / ZoomFactor, ActualWidth / ZoomFactor, ActualHeight / ZoomFactor);
            }
        }

        private void ZoomTo(Rect zoomToRectangle)
        {
            if (zoomToRectangle.IsEmpty || zoomToRectangle.Width <= 0 || zoomToRectangle.Height <= 0 || ActualWidth <= 0 || ActualHeight <= 0)
            {
                return;
            }

            double scale = ClampZoom(Math.Min(ActualWidth / zoomToRectangle.Width, ActualHeight / zoomToRectangle.Height));

            double offsetX = (ActualWidth - zoomToRectangle.Width * scale) / 2d;
            double offsetY = (ActualHeight - zoomToRectangle.Height * scale) / 2d;

            SetViewTransform(scale, offsetX - zoomToRectangle.X * scale, offsetY - zoomToRectangle.Y * scale);
        }

        private void ZoomBy(double zoomFactor, Point anchorPosition)
        {
            double currentZoom = ZoomFactor;

            if (zoomFactor <= 0 || double.IsNaN(zoomFactor) || double.IsInfinity(zoomFactor))
            {
                return;
            }

            double newZoom = ClampZoom(currentZoom * zoomFactor);

            if (Math.Abs(newZoom - currentZoom) < zoomTolerance)
            {
                return;
            }

            double contentX = (anchorPosition.X - TranslateX) / currentZoom;
            double contentY = (anchorPosition.Y - TranslateY) / currentZoom;
            SetViewTransform(newZoom, anchorPosition.X - contentX * newZoom, anchorPosition.Y - contentY * newZoom);
        }

        private void StartPanning(Point startPosition)
        {
            panStartPosition = startPosition;
            isPanning = true;
        }

        private void PanTo(Point position)
        {
            if (!isPanning)
            {
                return;
            }

            Vector delta = position - panStartPosition;
            SetViewTransform(ZoomFactor, TranslateX + delta.X, TranslateY + delta.Y);
            panStartPosition = position;
        }

        private void StopPanning()
        {
            isPanning = false;
        }

        private static double ClampZoom(double zoom)
        {
            return Math.Max(minZoom, Math.Min(maxZoom, zoom));
        }

        private void SetViewTransform(double zoom, double translateX, double translateY)
        {
            isInitialViewSet = true;

            scaleTransform.ScaleX = zoom;
            scaleTransform.ScaleY = zoom;
            translateTransform.X = translateX;
            translateTransform.Y = translateY;
        }

        private void OnContentSizeChanged(object sender, EventArgs e)
        {
            QueueInitialZoomToFill();
        }

        private void QueueInitialZoomToFill()
        {
            if (isInitialViewSet || isInitialZoomToFillQueued)
            {
                return;
            }

            isInitialZoomToFillQueued = true;
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                isInitialZoomToFillQueued = false;
                if (!isInitialViewSet)
                {
                    ZoomToFill();
                }
            }));
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ZoomBy(GetMouseWheelZoomFactor(e.Delta), e.GetPosition(this));
            e.Handled = true;
        }

        private static double GetMouseWheelZoomFactor(int delta)
        {
            double viewportFactor = Math.Max(1d / maxZoomDelta, Math.Min(maxZoomDelta, 1d - delta / zoomDeltaMultiplier));
            return 1d / viewportFactor;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPanPending = true;
            panStartPosition = e.GetPosition(this);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!isPanPending)
            {
                return;
            }

            Point currentPosition = e.GetPosition(this);
            if (!isPanning)
            {
                if (Math.Abs(currentPosition.X - panStartPosition.X) < SystemParameters.MinimumHorizontalDragDistance && Math.Abs(currentPosition.Y - panStartPosition.Y) < SystemParameters.MinimumVerticalDragDistance)
                {
                    return;
                }

                originalCursor = Cursor;
                StartPanning(panStartPosition);
                CaptureMouse();
                Cursor = Cursors.Hand;
            }

            PanTo(currentPosition);
            e.Handled = true;
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bool wasPanning = isPanning;
            ResetPanState();

            if (wasPanning)
            {
                e.Handled = true;
            }
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            ResetPanState();
        }

        private void ResetPanState()
        {
            isPanPending = false;
            if (!isPanning)
            {
                return;
            }

            StopPanning();
            Cursor = originalCursor;

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }
    }
}