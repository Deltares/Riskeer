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

using System.Linq;
using System.Windows;

using Core.Common.Gui;
using Core.Components.Charting.Forms;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Legend;
using Fluent;
using NUnit.Framework;
using Rhino.Mocks;
using Button = Fluent.Button;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using ICommand = Core.Common.Controls.Commands.ICommand;
using ToggleButton = Fluent.ToggleButton;

namespace Core.Plugins.OxyPlot.Test
{
    [TestFixture]
    public class ChartingRibbonTest
    {
        [Test]
        [RequiresSTA]
        public void Commands_NoCommandsAssigned_ReturnsEmptyCommandsCollection()
        {
            // Setup
            var ribbon = new ChartingRibbon();

            // Call & Assert
            CollectionAssert.IsEmpty(ribbon.Commands);
        }

        [Test]
        [RequiresSTA]
        public void Commands_CommandsAssigned_ReturnsAssignedCommands()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.Stub<IToolViewController>();
            mocks.ReplayAll();

            var toggleLegendViewCommand = new ToggleLegendViewCommand(new ChartLegendController(toolViewController));
            var ribbon = new ChartingRibbon
            {
                ToggleLegendViewCommand = toggleLegendViewCommand,
            };

            // Call
            var commands = ribbon.Commands.ToArray();

            // Assert
            CollectionAssert.AreEqual(new ICommand[]{toggleLegendViewCommand}, commands);

            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void DefaultConstructor_Always_CreatesControl()
        {
            // Setup
            var ribbon = new ChartingRibbon();

            // Call & Assert
            Assert.IsInstanceOf<System.Windows.Controls.Control>(ribbon.GetRibbonControl());
        }
        
        [Test]
        [RequiresSTA]
        public void IsContextualTabVisible_Always_ReturnsFalse()
        {
            // Setup
            var ribbon = new ChartingRibbon();

            // Call & Assert
            Assert.IsFalse(ribbon.IsContextualTabVisible(null,null));
        }

        [Test]
        [RequiresSTA]
        public void TogglePanning_OnClick_TogglePanning()
        {
            // Setup
            var mocks = new MockRepository();
            var chart = mocks.DynamicMock<IChartControl>();
            chart.Expect(c => c.TogglePanning());

            mocks.ReplayAll();

            var ribbon = new ChartingRibbon
            {
                Chart = chart
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
        [RequiresSTA]
        public void ToggleRectangleZooming_OnClick_ToggleRectangleZooming()
        {
            // Setup
            var mocks = new MockRepository();
            var chart = mocks.DynamicMock<IChartControl>();
            chart.Expect(c => c.ToggleRectangleZooming());

            mocks.ReplayAll();

            var ribbon = new ChartingRibbon
            {
                Chart = chart
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
        [RequiresSTA]
        public void ToggleLegendViewButton_OnClick_ExecutesToggleLegendViewCommand()
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Execute());

            mocks.ReplayAll();

            var ribbon = new ChartingRibbon
            {
                ToggleLegendViewCommand = command
            };
            var button = ribbon.GetRibbonControl().FindName("ToggleLegendViewButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle legend view button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ZoomToAll_OnClick_ExecutesChartZoomToAll()
        {
            // Setup
            var ribbon = new ChartingRibbon();
            var mocks = new MockRepository();
            var chart = mocks.StrictMock<IChartControl>();

            chart.Expect(c => c.IsPanningEnabled).Return(true);
            chart.Expect(c => c.IsRectangleZoomingEnabled).Return(true);
            chart.Expect(c => c.ZoomToAll());

            mocks.ReplayAll();

            ribbon.Chart = chart;

            var button = ribbon.GetRibbonControl().FindName("ZoomToAllButton") as Button;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a zoom to all button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void Chart_Always_AttachesToChartAndUpdatesChartingCommands(bool buttonChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var chart = mocks.StrictMock<IChartControl>();
            var ribbon = new ChartingRibbon();

            chart.Expect(c => c.IsPanningEnabled).Return(buttonChecked);
            chart.Expect(c => c.IsRectangleZoomingEnabled).Return(buttonChecked);

            mocks.ReplayAll();

            var togglePanningButton = ribbon.GetRibbonControl().FindName("TogglePanningButton") as ToggleButton;
            var toggleRectangleZoomingButton = ribbon.GetRibbonControl().FindName("ToggleRectangleZoomingButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(togglePanningButton, "Ribbon should have a toggle panning button.");
            Assert.IsNotNull(toggleRectangleZoomingButton, "Ribbon should have a rectangle zoom panning button.");

            // Call
            ribbon.Chart = chart;

            // Assert
            Assert.AreEqual(buttonChecked, togglePanningButton.IsChecked);
            Assert.AreEqual(buttonChecked, toggleRectangleZoomingButton.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void Chart_WithOrWithoutChart_UpdatesChartingContextualVisibility(bool chartVisible)
        {
            // Setup
            var mocks = new MockRepository();
            var chart = mocks.Stub<IChartControl>();

            mocks.ReplayAll();

            var ribbon = new ChartingRibbon();

            var contextualGroup = ribbon.GetRibbonControl().FindName("ChartingContextualGroup") as RibbonContextualTabGroup;

            // Precondition
            Assert.IsNotNull(contextualGroup, "Ribbon should have a charting contextual group button.");

            // Call
            ribbon.Chart = chartVisible ? chart : null;

            // Assert
            Assert.AreEqual(chartVisible ? Visibility.Visible : Visibility.Collapsed, contextualGroup.Visibility);
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_ToggleLegendViewIsCheckedEqualToCommandChecked(bool commandChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Checked).Return(commandChecked);

            mocks.ReplayAll();

            var ribbon = new ChartingRibbon
            {
                ToggleLegendViewCommand = command,
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
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_TogglePanningIsCheckedEqualToChartIsPanning(bool buttonChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var chart = mocks.StrictMock<IChartControl>();

            var ribbon = new ChartingRibbon();

            chart.Expect(c => c.IsPanningEnabled).Return(buttonChecked).Repeat.Twice();
            chart.Expect(c => c.IsRectangleZoomingEnabled).Return(buttonChecked).Repeat.Twice();

            mocks.ReplayAll();

            ribbon.Chart = chart;

            var togglePanningButton = ribbon.GetRibbonControl().FindName("TogglePanningButton") as ToggleButton;
            var toggleRectangleZoomingButton = ribbon.GetRibbonControl().FindName("ToggleRectangleZoomingButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(togglePanningButton, "Ribbon should have a toggle panning button.");
            Assert.IsNotNull(toggleRectangleZoomingButton, "Ribbon should have a rectangle zoom panning button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(buttonChecked, togglePanningButton.IsChecked);
            Assert.AreEqual(buttonChecked, toggleRectangleZoomingButton.IsChecked);
            mocks.VerifyAll();
        }
    }
}