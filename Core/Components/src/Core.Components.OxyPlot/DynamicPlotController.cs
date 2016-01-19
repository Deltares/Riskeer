using OxyPlot;

namespace Core.Components.OxyPlot
{
    public sealed class DynamicPlotController : PlotController
    {
        public DynamicPlotController()
        {
            DisableInteraction();
        }

        public bool IsPanningEnabled { get; private set; }
        public bool IsRectangleZoomingEnabled { get; private set; }

        /// <summary>
        /// Toggles panning by click and holding the left mouse button while moving.
        /// </summary>
        public void TogglePanning()
        {
            var enablePanning = !IsPanningEnabled;
            DisableInteraction();
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
            DisableInteraction();
            if (enableRectangleZoom)
            {
                EnableRectangleZoom();
            }
        }

        /// <summary>
        /// Disables all the interaction with the <see cref="DynamicPlotController"/>.
        /// </summary>
        private void DisableInteraction()
        {
            UnbindAll();
            IsPanningEnabled = false;
            IsRectangleZoomingEnabled = false;
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