using Core.Common.Base;
using Core.Components.Charting.Data;

namespace Core.Components.Charting
{
    public interface IChart {
        /// <summary>
        /// Gets a value representing whether the chart can be panned with the left mouse button.
        /// </summary>
        bool IsPanningEnabled { get; }

        /// <summary>
        /// Gets a value representing whether the chart can be zoomed by rectangle with the left mouse button.
        /// </summary>
        bool IsRectangleZoomingEnabled { get; }

        /// <summary>
        /// Gets or sets the data to show in the <see cref="IChart"/>.
        /// </summary>
        /// <remarks>The returned collection is a copy of the previously set data.</remarks>
        ChartData Data { get; set; }

        /// <summary>
        /// Toggles panning of the <see cref="IChart"/>. Panning is invoked by clicking the left mouse-button.
        /// </summary>
        void TogglePanning();

        /// <summary>
        /// Toggles rectangle zooming of the <see cref="IChart"/>. Rectangle zooming is invoked by clicking the left mouse-button.
        /// </summary>
        void ToggleRectangleZooming();

        /// <summary>
        /// Zooms to a level so that everything is in view.
        /// </summary>
        void ZoomToAll();
    }
}