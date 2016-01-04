using System.Collections.ObjectModel;
using OxyPlot;

namespace Core.Components.OxyPlot.Data
{
    /// <summary>
    /// This class represents a collection of data which can be added to a <see cref="BaseChart"/>.
    /// </summary>
    public class CollectionData : IChartData
    {
        readonly Collection<IChartData> dataCollection = new Collection<IChartData>();

        public void AddTo(PlotModel model)
        {
            foreach (IChartData data in dataCollection)
            {
                data.AddTo(model);
            }
        }

        /// <summary>
        /// Add new data to the <see cref="CollectionData"/>.
        /// </summary>
        /// <param name="data">The data to add to the <see cref="CollectionData"/>.</param>
        public void Add(IChartData data)
        {
            dataCollection.Add(data);
        }
    }
}