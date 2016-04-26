using System;
using System.Linq;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Update;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class HydraulicBoundaryLocationUpdateExtensions
    {
        public static void Update(this HydraulicBoundaryLocation location, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = GetSingleHydraulicBoundaryLocation(location, context);
            entity.Name = location.Name;
            entity.LocationX = Convert.ToDecimal(location.Location.X);
            entity.LocationY = Convert.ToDecimal(location.Location.Y);
            entity.DesignWaterLevel = double.IsNaN(location.DesignWaterLevel) ? (double?) null : Convert.ToDouble(location.DesignWaterLevel);

            collector.Update(entity);
        }

        private static HydraulicLocationEntity GetSingleHydraulicBoundaryLocation(HydraulicBoundaryLocation location, IRingtoetsEntities context)
        {
            try
            {
                return context.HydraulicLocationEntities.Single(hle => hle.HydraulicLocationEntityId == location.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(HydraulicLocationEntity).Name, location.StorageId), exception);
            } 
        }
    }
}