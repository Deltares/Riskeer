using OxyPlot;

namespace Core.Components.OxyPlot
{
    /// <summary>
    /// This class represents a controller for which certain interactions can be toggled.
    /// </summary>
    internal sealed class DynamicPlotController : ControllerBase, IPlotController
    {
        /// <summary>
        /// Gets a value representing whether panning is enabled for the <see cref="DynamicPlotController"/>.
        /// </summary>
        public bool IsPanningEnabled { get; private set; }

        /// <summary>
        /// Gets a value representing whether zooming by rectangle is enabled for the <see cref="DynamicPlotController"/>.
        /// </summary>
        public bool IsRectangleZoomingEnabled { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="DynamicPlotController"/>.
        /// </summary>
        public DynamicPlotController()
        {
            EnableScrollWheelZooming();
        }

        /// <summary>
        /// Toggles panning by click and holding the left mouse button while moving.
        /// </summary>
        public void TogglePanning()
        {
            var enablePanning = !IsPanningEnabled;
            ResetDefaultInteraction();
            if (enablePanning)
            {
                EnablePanning();
            }
        }

        /// <summary>
        /// Toggles zooming by drawing a rectangle with the mouse.
        /// </summary>
        public void ToggleRectangleZooming()
        {
            var enableRectangleZoom = !IsRectangleZoomingEnabled;
            ResetDefaultInteraction();
            if (enableRectangleZoom)
            {
                EnableRectangleZoom();
            }
        }

        /// <summary>
        /// Resets all the toggleable interaction with the <see cref="DynamicPlotController"/>.
        /// </summary>
        private void ResetDefaultInteraction()
        {
            UnbindAll();
            IsPanningEnabled = false;
            IsRectangleZoomingEnabled = false;

            EnableScrollWheelZooming();
        }

        /// <summary>
        /// Enables zooming in and out by using the scroll wheel of the <see cref="DynamicPlotController"/>.
        /// </summary>
        private void EnableScrollWheelZooming()
        {
            this.BindMouseWheel(PlotCommands.ZoomWheel);
        }

        /// <summary>
        /// Enables panning of the <see cref="DynamicPlotController"/>. Panning is invoked by clicking the left mouse-button.
        /// </summary>
        private void EnablePanning()
        {
            this.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
            IsPanningEnabled = true;
        }

        /// <summary>
        /// Enables zooming by rectangle of the <see cref="DynamicPlotController"/>. Zooming by rectangle is invoked by clicking the left mouse-button.
        /// </summary>
        private void EnableRectangleZoom()
        {
            this.BindMouseDown(OxyMouseButton.Left, PlotCommands.ZoomRectangle);
            IsRectangleZoomingEnabled = true;
        }
    }
}