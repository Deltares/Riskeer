using Core.Common.Base;

namespace Riskeer.Common.Data.Hydraulics
{
    public class HydraulicBoundaryDatabases : Observable
    {
        public HydraulicBoundaryDatabases()
        {
            HydraulicBoundaryDatabaseInstances = new ObservableList<HydraulicBoundaryDatabase>();
            HydraulicLocationConfigurationSettings = new HydraulicLocationConfigurationSettings();
        }

        public IObservableEnumerable<HydraulicBoundaryDatabase> HydraulicBoundaryDatabaseInstances { get; }

        /// <summary>
        /// Gets the <see cref="Hydraulics.HydraulicLocationConfigurationSettings"/>.
        /// </summary>
        public HydraulicLocationConfigurationSettings HydraulicLocationConfigurationSettings { get; }
    }
}