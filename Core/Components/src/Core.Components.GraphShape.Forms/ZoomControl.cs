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

namespace Core.Components.GraphShape.Forms
{
    public enum ZoomControlModes
    {
        Fill,
        Original,
        Custom
    }

    public enum ZoomViewModifierMode
    {
        None,
        Pan,
        ZoomIn,
        ZoomOut,
        ZoomBox
    }

    public class ZoomControl : ContentControl
    {
        private const string presenterPartName = "PART_Presenter";

        private ZoomContentPresenter presenter;
        private bool isPanPending;
        private bool isPanning;
        private Point panStartPosition;
        private Cursor originalCursor;
        private readonly ScaleTransform scaleTransform = new ScaleTransform(1, 1);
        private readonly TranslateTransform translateTransform = new TranslateTransform();

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(ZoomControlModes), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(ZoomControlModes.Original));

        public static readonly DependencyProperty ModifierModeProperty =
            DependencyProperty.Register(nameof(ModifierMode), typeof(ZoomViewModifierMode), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(ZoomViewModifierMode.None));

        public static readonly DependencyProperty ZoomDeltaMultiplierProperty =
            DependencyProperty.Register(nameof(ZoomDeltaMultiplier), typeof(double), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(300d));

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register(nameof(MinZoom), typeof(double), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(0.2d));

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register(nameof(MaxZoom), typeof(double), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(5d));

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(1d, OnViewTransformPropertyChanged));

        public static readonly DependencyProperty TranslateXProperty =
            DependencyProperty.Register(nameof(TranslateX), typeof(double), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(0d, OnViewTransformPropertyChanged));

        public static readonly DependencyProperty TranslateYProperty =
            DependencyProperty.Register(nameof(TranslateY), typeof(double), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(0d, OnViewTransformPropertyChanged));

        public static readonly DependencyProperty MaxZoomDeltaProperty =
            DependencyProperty.Register(nameof(MaxZoomDelta), typeof(double), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(5d));

        public static readonly DependencyProperty AnimationLengthProperty =
            DependencyProperty.Register(nameof(AnimationLength), typeof(TimeSpan), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(TimeSpan.Zero));

        public static readonly DependencyProperty ZoomBoxProperty =
            DependencyProperty.Register(nameof(ZoomBox), typeof(Rect), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(Rect.Empty));

        public static readonly DependencyProperty ZoomBoxBorderBrushProperty =
            DependencyProperty.Register(nameof(ZoomBoxBorderBrush), typeof(Brush), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(Brushes.Silver));

        public static readonly DependencyProperty ZoomBoxBorderThicknessProperty =
            DependencyProperty.Register(nameof(ZoomBoxBorderThickness), typeof(Thickness), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(new Thickness(1)));

        public static readonly DependencyProperty ZoomBoxOpacityProperty =
            DependencyProperty.Register(nameof(ZoomBoxOpacity), typeof(double), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(0d));

        public static readonly DependencyProperty ZoomBoxBackgroundProperty =
            DependencyProperty.Register(nameof(ZoomBoxBackground), typeof(Brush), typeof(ZoomControl),
                                        new FrameworkPropertyMetadata(Brushes.Transparent));

        public ZoomControlModes Mode
        {
            get => (ZoomControlModes) GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public ZoomViewModifierMode ModifierMode
        {
            get => (ZoomViewModifierMode) GetValue(ModifierModeProperty);
            set => SetValue(ModifierModeProperty, value);
        }

        public double ZoomDeltaMultiplier
        {
            get => (double) GetValue(ZoomDeltaMultiplierProperty);
            set => SetValue(ZoomDeltaMultiplierProperty, value);
        }

        public double MinZoom
        {
            get => (double) GetValue(MinZoomProperty);
            set => SetValue(MinZoomProperty, value);
        }

        public double MaxZoom
        {
            get => (double) GetValue(MaxZoomProperty);
            set => SetValue(MaxZoomProperty, value);
        }

        public double Zoom
        {
            get => (double) GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public double TranslateX
        {
            get => (double) GetValue(TranslateXProperty);
            set => SetValue(TranslateXProperty, value);
        }

        public double TranslateY
        {
            get => (double) GetValue(TranslateYProperty);
            set => SetValue(TranslateYProperty, value);
        }

        public double MaxZoomDelta
        {
            get => (double) GetValue(MaxZoomDeltaProperty);
            set => SetValue(MaxZoomDeltaProperty, value);
        }

        public TimeSpan AnimationLength
        {
            get => (TimeSpan) GetValue(AnimationLengthProperty);
            set => SetValue(AnimationLengthProperty, value);
        }

        public Rect ZoomBox
        {
            get => (Rect) GetValue(ZoomBoxProperty);
            private set => SetValue(ZoomBoxProperty, value);
        }

        public Brush ZoomBoxBorderBrush
        {
            get => (Brush) GetValue(ZoomBoxBorderBrushProperty);
            set => SetValue(ZoomBoxBorderBrushProperty, value);
        }

        public Thickness ZoomBoxBorderThickness
        {
            get => (Thickness) GetValue(ZoomBoxBorderThicknessProperty);
            set => SetValue(ZoomBoxBorderThicknessProperty, value);
        }

        public double ZoomBoxOpacity
        {
            get => (double) GetValue(ZoomBoxOpacityProperty);
            set => SetValue(ZoomBoxOpacityProperty, value);
        }

        public Brush ZoomBoxBackground
        {
            get => (Brush) GetValue(ZoomBoxBackgroundProperty);
            set => SetValue(ZoomBoxBackgroundProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            presenter = GetTemplateChild(presenterPartName) as ZoomContentPresenter;
            ApplyTransform();
        }

        public void ZoomTo(Rect zoomToRectangle)
        {
            if (zoomToRectangle.IsEmpty
                || zoomToRectangle.Width <= 0
                || zoomToRectangle.Height <= 0
                || ActualWidth <= 0
                || ActualHeight <= 0)
            {
                return;
            }

            var maxZoom = GetMaximumZoom();
            var minZoom = GetMinimumZoom(maxZoom);
            var scale = Math.Min(ActualWidth / zoomToRectangle.Width, ActualHeight / zoomToRectangle.Height);
            scale = Math.Max(minZoom, Math.Min(maxZoom, scale));

            SetViewTransform(scale, -zoomToRectangle.X * scale, -zoomToRectangle.Y * scale);
        }

        public void ZoomToOriginal()
        {
            SetViewTransform(1d, 0d, 0d);
        }

        public void ZoomToFill()
        {
            if (presenter == null
                || presenter.ContentSize.Width <= 0
                || presenter.ContentSize.Height <= 0)
            {
                return;
            }

            ZoomTo(new Rect(new Point(0, 0), presenter.ContentSize));
        }

        internal void StartPanning(Point startPosition)
        {
            panStartPosition = startPosition;
            isPanning = true;
        }

        internal void PanTo(Point position)
        {
            if (!isPanning)
            {
                return;
            }

            Vector delta = position - panStartPosition;
            SetViewTransform(Zoom, TranslateX + delta.X, TranslateY + delta.Y);
            panStartPosition = position;
        }

        internal void StopPanning()
        {
            isPanning = false;
        }

        private static void OnViewTransformPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ((ZoomControl) dependencyObject).ApplyTransform();
        }

        private void ApplyTransform()
        {
            scaleTransform.ScaleX = Zoom;
            scaleTransform.ScaleY = Zoom;

            translateTransform.X = TranslateX;
            translateTransform.Y = TranslateY;

            if (presenter != null)
            {
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleTransform);
                transformGroup.Children.Add(translateTransform);
                presenter.RenderTransform = transformGroup;
                presenter.RenderTransformOrigin = new Point(0, 0);
            }

            ZoomBox = GetViewport();
        }

        private void SetViewTransform(double zoom, double translateX, double translateY)
        {
            var maximumZoom = GetMaximumZoom();
            var minimumZoom = GetMinimumZoom(maximumZoom);

            Zoom = Math.Max(minimumZoom, Math.Min(maximumZoom, zoom));
            TranslateX = translateX;
            TranslateY = translateY;
        }

        private double GetMaximumZoom()
        {
            return Math.Max(1d, MaxZoom);
        }

        private double GetMinimumZoom(double maximumZoom)
        {
            return Math.Max(1d / Math.Max(1d, MaxZoomDelta), Math.Min(MinZoom, maximumZoom));
        }

        private Rect GetViewport()
        {
            if (ActualWidth <= 0
                || ActualHeight <= 0
                || Zoom <= 0)
            {
                return Rect.Empty;
            }

            return new Rect(-TranslateX / Zoom,
                            -TranslateY / Zoom,
                            ActualWidth / Zoom,
                            ActualHeight / Zoom);
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
                if (Math.Abs(currentPosition.X - panStartPosition.X) < SystemParameters.MinimumHorizontalDragDistance
                    && Math.Abs(currentPosition.Y - panStartPosition.Y) < SystemParameters.MinimumVerticalDragDistance)
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

        public ZoomControl()
        {
            PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            PreviewMouseMove += OnPreviewMouseMove;
            PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            LostMouseCapture += OnLostMouseCapture;
            SizeChanged += (sender, args) => ApplyTransform();
        }
    }
}
