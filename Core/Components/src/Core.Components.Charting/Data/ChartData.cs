using Core.Common.Base;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// Abstract class for data with the purpose of becoming visible in charting components.
    /// </summary>
    public abstract class ChartData : Observable
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartData"/>.
        /// </summary>
        protected ChartData()
        {
        }
    }
}