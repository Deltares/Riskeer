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
        }

        [Test]
        public void TogglePanning_PanningDisabled_PanningEnabled()
        {
            // Setup
            var controller = new DynamicPlotController();

            // Call
            controller.TogglePanning();

            // Assert
            Assert.IsTrue(controller.IsPanningEnabled);
            Assert.IsFalse(controller.IsRectangleZoomingEnabled);
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
        }
    }
}