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

using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test
{
    [TestFixture]
    public class DynamicPlotControllerTest
    {
        [Test]
        public void DefaultConstructor_ReturnsIPlotController()
        {
            // Call
            var controller = new DynamicPlotController();

            // Assert
            Assert.IsInstanceOf<IPlotController>(controller);
            Assert.IsInstanceOf<ControllerBase>(controller);
            Assert.IsTrue(controller.IsPanningEnabled);
            Assert.IsFalse(controller.IsRectangleZoomingEnabled);
            Assert.AreEqual(3, controller.InputCommandBindings.Count);
            AssertWheelZoomCommandBinding(controller, 0);
            AssertMousePanAtCommandBinding(controller, 1, OxyMouseButton.Left);
            AssertMousePanAtCommandBinding(controller, 2, OxyMouseButton.Middle);
        }

        [Test]
        public void TogglePanning_PanningDisabled_PanningEnabled()
        {
            // Setup
            var controller = new DynamicPlotController();
            controller.ToggleRectangleZooming();

            // Call
            controller.TogglePanning();

            // Assert
            Assert.IsTrue(controller.IsPanningEnabled);
            Assert.IsFalse(controller.IsRectangleZoomingEnabled);
            Assert.AreEqual(3, controller.InputCommandBindings.Count);
            AssertWheelZoomCommandBinding(controller, 0);
            AssertMousePanAtCommandBinding(controller, 1, OxyMouseButton.Left);
            AssertMousePanAtCommandBinding(controller, 2, OxyMouseButton.Middle);
        }

        [Test]
        public void ToggleRectangleZooming_RectangleZoomingDisabled_RectangleZoomingEnabled()
        {
            // Setup
            var controller = new DynamicPlotController();

            // Call
            controller.ToggleRectangleZooming();

            // Assert
            Assert.IsTrue(controller.IsRectangleZoomingEnabled);
            Assert.IsFalse(controller.IsPanningEnabled);
            Assert.AreEqual(2, controller.InputCommandBindings.Count);
            AssertWheelZoomCommandBinding(controller, 0);
            AssertCustomRectangleZoomCommand(controller, 1);
        }

        private static void AssertMousePanAtCommandBinding(DynamicPlotController controller, int index, OxyMouseButton expectedMouseButton)
        {
            InputCommandBinding panAtCommand = controller.InputCommandBindings[index];
            Assert.AreEqual(expectedMouseButton, ((OxyMouseDownGesture) panAtCommand.Gesture).MouseButton);
            Assert.AreEqual(PlotCommands.PanAt, panAtCommand.Command);
        }

        private static void AssertWheelZoomCommandBinding(DynamicPlotController controller, int index)
        {
            InputCommandBinding wheelZoomCommand = controller.InputCommandBindings[index];
            Assert.IsTrue(wheelZoomCommand.Gesture is OxyMouseWheelGesture);
            Assert.AreEqual(PlotCommands.ZoomWheel, wheelZoomCommand.Command);
        }

        private static void AssertCustomRectangleZoomCommand(DynamicPlotController controller, int index)
        {
            InputCommandBinding rectangleZoomCommand = controller.InputCommandBindings[index];
            Assert.AreEqual(OxyMouseButton.Left, ((OxyMouseDownGesture) rectangleZoomCommand.Gesture).MouseButton);
            Assert.AreEqual(PlotCommands.ZoomRectangle, rectangleZoomCommand.Command);
        }
    }
}