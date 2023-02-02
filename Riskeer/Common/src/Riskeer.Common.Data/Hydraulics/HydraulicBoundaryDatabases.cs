using Core.Common.Base;

namespace Riskeer.Common.Data.Hydraulics
{
    public class HydraulicBoundaryDatabases : Observable
    {
        private readonly ObservableList<HydraulicBoundaryDatabase> hydraulicBoundaryDatabaseInstances;
        
        public HydraulicBoundaryDatabases()
        {
            hydraulicBoundaryDatabaseInstances = new ObservableList<HydraulicBoundaryDatabase>();
            HydraulicLocationConfigurationSettings = new HydraulicLocationConfigurationSettings();
        }

        public void AddHydraulicBoundaryDatabaseInstance(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            hydraulicBoundaryDatabaseInstances.Add(hydraulicBoundaryDatabase);
        }

        public IObservableEnumerable<HydraulicBoundaryDatabase> HydraulicBoundaryDatabaseInstances => hydraulicBoundaryDatabaseInstances;

        /// <summary>
        /// Gets the <see cref="Hydraulics.HydraulicLocationConfigurationSettings"/>.
        /// </summary>
        public HydraulicLocationConfigurationSettings HydraulicLocationConfigurationSettings { get; }
    }
}