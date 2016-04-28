using System;
using Application.Ringtoets.Storage.Create;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class HydraulicBoundaryLocationCreateExtensions
    {
        public static HydraulicLocationEntity Create(this HydraulicBoundaryLocation location, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new HydraulicLocationEntity
            {
                LocationId = location.Id,
                Name = location.Name,
                LocationX = Convert.ToDecimal(location.Location.X),
                LocationY = Convert.ToDecimal(location.Location.Y),
                DesignWaterLevel = Double.IsNaN(location.DesignWaterLevel) ? (double?) null : Convert.ToDouble(location.DesignWaterLevel)
            };

            collector.Create(entity, location);
            return entity;
        }
    }
}