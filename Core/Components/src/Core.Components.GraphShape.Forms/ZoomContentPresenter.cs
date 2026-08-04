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
using System.Windows.Media;

namespace Core.Components.GraphShape.Forms
{
    public class ZoomContentPresenter : ContentPresenter
    {
        public event EventHandler ContentSizeChanged;

        public Size ContentSize { get; private set; }

        protected override Size MeasureOverride(Size constraint)
        {
            Size contentSize = base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));
            SetContentSize(contentSize);

            return new Size(GetConstrainedLength(contentSize.Width, constraint.Width),
                            GetConstrainedLength(contentSize.Height, constraint.Height));
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (VisualChildrenCount == 0)
            {
                return arrangeBounds;
            }

            var child = (UIElement) VisualTreeHelper.GetChild(this, 0);
            child.Arrange(new Rect(ContentSize));

            return arrangeBounds;
        }

        private void SetContentSize(Size contentSize)
        {
            if (ContentSize.Equals(contentSize))
            {
                return;
            }

            ContentSize = contentSize;
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        private static double GetConstrainedLength(double contentLength, double availableLength)
        {
            return double.IsInfinity(availableLength)
                       ? contentLength
                       : Math.Min(contentLength, availableLength);
        }
    }
}
