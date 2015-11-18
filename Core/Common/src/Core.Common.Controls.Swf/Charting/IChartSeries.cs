using System;
using System.Collections.Generic;
using System.Drawing;

namespace Core.Common.Controls.Swf.Charting
{
    /// <summary>
    /// Adapter of TChart series
    /// </summary>
    public interface IChartSeries
    {
        /// <summary>
        /// The title of the series
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Color for this series
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Visibility of this series
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Chart that this series belongs to
        /// </summary>
        IChart Chart { get; set; }

        /// <summary>
        /// Show the object in a legend
        /// </summary>
        bool ShowInLegend { get; set; }

        /// <summary>
        /// Default null value
        /// </summary>
        double DefaultNullValue { get; set; }

        /// <summary>
        /// Vertical axis that this series uses for displaying its values
        /// </summary>
        VerticalAxis VertAxis { get; set; }

        /// <summary>
        /// Show the series be updated?
        /// </summary>
        bool RefreshRequired { get; }

        /// <summary>
        /// Data source containing the series data
        /// </summary>
        object DataSource { get; set; }

        /// <summary>
        /// DataMember for horizontal axis
        /// </summary>
        string XValuesDataMember { get; set; }

        /// <summary>
        /// DataMember for vertical axis
        /// </summary>
        string YValuesDataMember { get; set; }

        /// <summary>
        /// Values that should not be rendered
        /// </summary>
        IList<double> NoDataValues { get; set; }

        /// <summary>
        /// Should the series wait for the chart view timer to handle updates?
        /// True enables asynchronous timer based updates. False update immediately (slow)
        /// </summary>
        bool UpdateASynchronously { get; set; }

        /// <summary>
        /// Used to identify the ChartSeries.
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// Adds a value to the series 
        /// </summary>
        void Add(double? x, double? y);

        /// <summary>
        /// Adds a value to the series 
        /// </summary>
        void Add(DateTime dateTime, double value);

        /// <summary>
        /// Add values of <paramref name="xValues"/> and <paramref name="yValues"/> to the series
        /// </summary>
        /// <param name="xValues">Values for the x axis</param>
        /// <param name="yValues">Values for the y axis</param>
        void Add(double?[] xValues, double?[] yValues);

        /// <summary>
        /// Clear data of the series
        /// </summary>
        void Clear();

        /// <summary>
        /// Refreshes data used in the chart.
        /// </summary>
        void CheckDataSource();

        /// <summary>
        /// Updates the series
        /// </summary>
        void Refresh();

        /// <summary>
        /// Gives the x value for the provided <paramref name="x"/> (screen position)
        /// </summary>
        /// <param name="x">The X screen position</param>
        /// <returns>Associated x value for the provided <paramref name="x"/></returns>
        double XScreenToValue(int x);
    }
}