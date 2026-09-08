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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;

namespace Core.Components.GraphShape.Forms.Test
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ZoomControlTest
    {
        [Test]
        public void ZoomTo_GivenViewport_ThenZoomStateUpdated()
        {
            // Given
            var control = new ZoomControl();
            InitializeSize(control, 200, 100);

            // When
            control.ZoomTo(new Rect(20, 10, 100, 50));

            // Then
            Assert.AreEqual(2, control.Zoom);
            Assert.AreEqual(-40, control.TranslateX);
            Assert.AreEqual(-20, control.TranslateY);
            Assert.AreEqual(new Rect(20, 10, 100, 50), control.ZoomBox);
        }

        [Test]
        public void GivenZoomedControl_WhenPanning_ThenViewportTranslated()
        {
            // Given
            var control = new ZoomControl();
            InitializeSize(control, 200, 100);
            control.ZoomTo(new Rect(20, 10, 100, 50));

            // When
            control.StartPanning(new Point(50, 40));
            control.PanTo(new Point(80, 70));
            control.StopPanning();

            // Then
            Assert.AreEqual(2, control.Zoom);
            Assert.AreEqual(-10, control.TranslateX);
            Assert.AreEqual(10, control.TranslateY);
            Assert.AreEqual(new Rect(5, -5, 100, 50), control.ZoomBox);
        }

        [Test]
        public void GivenZoomedAndPannedControl_WhenZoomToOriginal_ThenViewportReset()
        {
            // Given
            var control = new ZoomControl();
            InitializeSize(control, 200, 100);
            control.ZoomTo(new Rect(20, 10, 100, 50));
            control.StartPanning(new Point(0, 0));
            control.PanTo(new Point(30, 20));
            control.StopPanning();

            // When
            control.ZoomToOriginal();

            // Then
            Assert.AreEqual(1, control.Zoom);
            Assert.AreEqual(0, control.TranslateX);
            Assert.AreEqual(0, control.TranslateY);
            Assert.AreEqual(new Rect(0, 0, 200, 100), control.ZoomBox);
        }

        [Test]
        public void GivenContent_WhenZoomToFill_ThenViewportFitsContent()
        {
            // Given
            var control = new ZoomControl
            {
                Content = new Border
                {
                    Width = 400,
                    Height = 300
                }
            };
            control.Style = CreateStyle();
            InitializeSize(control, 200, 100);

            // When
            control.ZoomToFill();

            // Then
            Assert.AreEqual(1.0 / 3.0, control.Zoom, 1e-10);
            Assert.AreEqual(0, control.TranslateX);
            Assert.AreEqual(0, control.TranslateY);
        }

        private static void InitializeSize(FrameworkElement control, double width, double height)
        {
            control.Measure(new Size(width, height));
            control.Arrange(new Rect(0, 0, width, height));
            control.UpdateLayout();
        }

        private static Style CreateStyle()
        {
            return (Style) new ResourceDictionary
            {
                Source = new Uri("/Core.Components.GraphShape.Forms;component/Templates/ZoomControlTemplate.xaml", UriKind.Relative)
            }[typeof(ZoomControl)];
        }
    }
}
