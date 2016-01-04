using System;
using OxyPlot;

namespace Core.Components.OxyPlot.Data
{
    /// <summary>
    /// This interface describes the data which can be added to the <see cref="BaseChart"/>.
    /// </summary>
    public interface IChartData {

        /// <summary>
        /// Adds the information in the <see cref="LineData"/> as a series of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PlotModel"/> to add a series to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        void AddTo(PlotModel model);
    }
}