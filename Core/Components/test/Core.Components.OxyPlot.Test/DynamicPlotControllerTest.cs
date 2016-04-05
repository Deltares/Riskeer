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
            AssertDefaultPanAtCommand(controller, 0);
            AssertDefaultWheelZoomCommand(controller, 1);
            AssertCustomPanCommand(controller, 2);
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
            AssertDefaultPanAtCommand(controller, 0);
            AssertDefaultWheelZoomCommand(controller, 1);
            AssertCustomPanCommand(controller, 2);
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
            Assert.AreEqual(3, controller.InputCommandBindings.Count);
            AssertDefaultPanAtCommand(controller, 0);
            AssertDefaultWheelZoomCommand(controller, 1);
            AssertCustomRectangleZoomCommand(controller, 2);
        }

        private static void AssertDefaultPanAtCommand(DynamicPlotController controller, int index)
        {
            var panAtCommand = controller.InputCommandBindings[index];
            Assert.AreEqual(OxyMouseButton.Middle, ((OxyMouseDownGesture) panAtCommand.Gesture).MouseButton);
            Assert.AreEqual(PlotCommands.PanAt, panAtCommand.Command);
        }

        private static void AssertDefaultWheelZoomCommand(DynamicPlotController controller, int index)
        {
            var wheelZoomCommand = controller.InputCommandBindings[index];
            Assert.IsTrue(wheelZoomCommand.Gesture is OxyMouseWheelGesture);
            Assert.AreEqual(PlotCommands.ZoomWheel, wheelZoomCommand.Command);
        }

        private void AssertCustomPanCommand(DynamicPlotController controller, int index)
        {
            var panAtCommand = controller.InputCommandBindings[index];
            Assert.AreEqual(OxyMouseButton.Left, ((OxyMouseDownGesture) panAtCommand.Gesture).MouseButton);
            Assert.AreEqual(PlotCommands.PanAt, panAtCommand.Command);
        }

        private void AssertCustomRectangleZoomCommand(DynamicPlotController controller, int index)
        {
            var rectangleZoomCommand = controller.InputCommandBindings[index];
            Assert.AreEqual(OxyMouseButton.Left, ((OxyMouseDownGesture) rectangleZoomCommand.Gesture).MouseButton);
            Assert.AreEqual(PlotCommands.ZoomRectangle, rectangleZoomCommand.Command);
        }
    }
}