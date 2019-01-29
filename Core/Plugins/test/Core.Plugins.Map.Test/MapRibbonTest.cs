// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Core.Common.Controls.Commands;
using Core.Components.Gis.Forms;
using Fluent;
using NUnit.Framework;
using Rhino.Mocks;
using Button = Fluent.Button;
using ToggleButton = Fluent.ToggleButton;

namespace Core.Plugins.Map.Test
{
    [TestFixture]
    public class MapRibbonTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void DefaultConstructor_Always_CreatesControlWithContextualGroupCollapsed()
        {
            // Call
            var ribbon = new MapRibbon();

            // Assert
            Ribbon ribbonControl = ribbon.GetRibbonControl();
            Assert.IsInstanceOf<Control>(ribbonControl);

            var contextualGroup = ribbonControl.FindName("MapContextualGroup") as RibbonContextualTabGroup;
            Assert.IsNotNull(contextualGroup);
            Assert.AreEqual(Visibility.Collapsed, contextualGroup.Visibility);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_WithOrWithoutMap_UpdatesMapContextualVisiblity(bool mapVisible)
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.Stub<IMapControl>();

            mocks.ReplayAll();

            var ribbon = new MapRibbon();
            var contextualGroup = ribbon.GetRibbonControl().FindName("MapContextualGroup") as RibbonContextualTabGroup;

            // Precondition
            Assert.IsNotNull(contextualGroup, "Ribbon should have a map contextual group button");

            // Call
            ribbon.Map = mapVisible ? map : null;

            // Assert
            Assert.AreEqual(mapVisible ? Visibility.Visible : Visibility.Collapsed, contextualGroup.Visibility);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_ToggleLegendViewIsCheckedEqualToCommandChecked(bool commandChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Checked).Return(commandChecked);

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                ToggleLegendViewCommand = command
            };

            var toggleLegendViewButton = ribbon.GetRibbonControl().FindName("ToggleLegendViewButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(toggleLegendViewButton, "Ribbon should have a toggle legend view button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(commandChecked, toggleLegendViewButton.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_TogglePanningButtonIsCheckedEqualToPanningChecked(bool panningChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Stub(m => m.IsPanningEnabled).Return(panningChecked);
            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };

            var button = ribbon.GetRibbonControl().FindName("TogglePanningButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle panning button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(panningChecked, button.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_ToggleRectangleZoomingButtonIsCheckedEqualToRectangleZoomChecked(bool rectangleZoomChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Stub(m => m.IsRectangleZoomingEnabled).Return(rectangleZoomChecked);
            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };

            var button = ribbon.GetRibbonControl().FindName("ToggleRectangleZoomingButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle rectangle zooming button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(rectangleZoomChecked, button.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_ToggleMouseCoordinatesButtonIsCheckedEqualToMouseCoordinatesChecked(bool mouseCoordinatesChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Stub(m => m.IsMouseCoordinatesVisible).Return(mouseCoordinatesChecked);
            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };

            var button = ribbon.GetRibbonControl().FindName("ToggleMouseCoordinatesButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle mouse coordinate visibility button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(mouseCoordinatesChecked, button.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ToggleLegendViewButton_OnClick_ExecutesToggleLegendViewCommand()
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Execute());

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                ToggleLegendViewCommand = command
            };
            var button = ribbon.GetRibbonControl().FindName("ToggleLegendViewButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle legend view button");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void TogglePanning_OnClick_TogglePanning()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Expect(c => c.TogglePanning());

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };
            var button = ribbon.GetRibbonControl().FindName("TogglePanningButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle panning button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ToggleRectangleZooming_OnClick_ToggleRectangleZooming()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Expect(c => c.ToggleRectangleZooming());

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };
            var button = ribbon.GetRibbonControl().FindName("ToggleRectangleZoomingButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle rectangle zooming button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ZoomToAll_OnClick_ZoomToAll()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Expect(c => c.ZoomToAllVisibleLayers());

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };
            var button = ribbon.GetRibbonControl().FindName("ZoomToAllButton") as Button;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a zoom to all button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ToggleMouseCoordinatesVisibility_OnClick_ToggleMouseCoordinates()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Expect(c => c.ToggleMouseCoordinatesVisibility());
            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };
            var button = ribbon.GetRibbonControl().FindName("ToggleMouseCoordinatesButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle mouse coordinates button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
    }
}