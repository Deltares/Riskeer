using System;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class HydraulicLocationEntity
    {
        public HydraulicBoundaryLocation Read()
        {
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(
                LocationId, 
                Name, 
                Convert.ToDouble(LocationX), 
                Convert.ToDouble(LocationY))
            {
                StorageId = HydraulicLocationEntityId
            };

            if (DesignWaterLevel.HasValue)
            {
                hydraulicBoundaryLocation.DesignWaterLevel = Convert.ToDouble(DesignWaterLevel);
            }

            return hydraulicBoundaryLocation;
        }
    }
}