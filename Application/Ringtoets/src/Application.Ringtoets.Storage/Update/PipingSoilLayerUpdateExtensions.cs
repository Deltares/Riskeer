using System;
using System.Linq;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Update;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class SoilLayerUpdateExtensions
    {
        public static void Update(this PipingSoilLayer layer, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = GetSingleSoilLayer(layer, context);

            entity.IsAquifer = Convert.ToByte(layer.IsAquifer);
            entity.Top = Convert.ToDecimal(layer.Top);
            
            collector.Update(entity);
        }

        private static SoilLayerEntity GetSingleSoilLayer(PipingSoilLayer layer, IRingtoetsEntities context)
        {
            try
            {
                return context.SoilLayerEntities.Single(sle => sle.SoilLayerEntityId == layer.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(SoilLayerEntity).Name, layer.StorageId), exception);
            } 
        }
    }
}