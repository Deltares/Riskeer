using System;
using Application.Ringtoets.Storage.Create;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class SoilLayerCreateExtensions
    {
        public static SoilLayerEntity Create(this PipingSoilLayer layer, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new SoilLayerEntity
            {
                IsAquifer = Convert.ToByte(layer.IsAquifer),
                Top = Convert.ToDecimal(layer.Top)
            };

            collector.Add(entity, layer);
            return entity;
        }
    }
}