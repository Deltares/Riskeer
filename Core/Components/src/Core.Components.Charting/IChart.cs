using System.Collections.Generic;
using Core.Common.Base;
using Core.Components.Charting.Data;

namespace Core.Components.Charting
{
    public interface IChart : IObservable {
        /// <summary>
        /// Gets a value representing whether the chart can be panned with the left mouse button.
        /// </summary>
        bool IsPanning { get; }

        /// <summary>
        /// Gets a value representing whether the chart can be zoomed by rectangle with the left mouse button.
        /// </summary>
        bool IsRectangleZooming { get; }

        /// <summary>
        /// Gets or sets the data to show in the <see cref="IChart"/>.
        /// </summary>
        /// <remarks>The returned collection is a copy of the previously set data.</remarks>
        ICollection<ChartData> Data { get; set; }

        /// <summary>
        /// Sets the visibility of a series in this <see cref="IChart"/>.
        /// </summary>
        /// <param name="serie">The <see cref="ChartData"/> to set the visibility for.</param>
        /// <param name="visibility">A boolean value representing the new visibility.</param>
        void SetVisibility(ChartData serie, bool visibility);

        /// <summary>
        /// Sets the position of the <see cref="ChartData"/> amongst the other data of the <see cref="IChart"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to change the position for.</param>
        /// <param name="position">The new position.</param>
        void SetPosition(ChartData data, int position);

        /// <summary>
        /// Toggles panning of the <see cref="IChart"/>. Panning is invoked by clicking the left mouse-button.
        /// </summary>
        void TogglePanning();

        /// <summary>
        /// Toggles rectangle zooming of the <see cref="IChart"/>. Rectangle zooming is invoked by clicking the left mouse-button.
        /// </summary>
        void ToggleRectangleZooming();
    }
}