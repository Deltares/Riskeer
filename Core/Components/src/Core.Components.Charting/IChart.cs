using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base;
using Core.Components.Charting.Data;

namespace Core.Components.Charting
{
    public interface IChart : IObservable {
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        ICollection<ChartData> Data { get; set; }

        /// <summary>
        /// Sets the visibility of a series in this <see cref="IChart"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to set the visibility for.</param>
        /// <param name="visibility">A boolean value representing the new visibility.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        void SetVisibility(ChartData data, bool visibility);

        /// <summary>
        /// Sets the position of the <see cref="ChartData"/> amongst the other data of the <see cref="IChart"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to change the position for.</param>
        /// <param name="position">The new position.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="position"/> is out of range.</exception>
        void SetPosition(ChartData data, int position);

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