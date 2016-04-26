using System;
using Application.Ringtoets.Storage.Create;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class SoilProfileCreateExtensions
    {
        public static SoilProfileEntity Create(this PipingSoilProfile profile, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            if (collector.Contains(profile))
            {
                return collector.Get(profile);
            }
            var entity = new SoilProfileEntity
            {
                Name = profile.Name,
                Bottom = Convert.ToDecimal(profile.Bottom)
            };

            CreatePipingSoilLayers(profile, collector, entity);

            collector.Add(entity, profile);
            return entity;
        }

        private static void CreatePipingSoilLayers(PipingSoilProfile profile, CreateConversionCollector collector, SoilProfileEntity entity)
        {
            foreach (var pipingSoilLayer in profile.Layers)
            {
                entity.SoilLayerEntities.Add(pipingSoilLayer.Create(collector));
            }
        }
    }
}