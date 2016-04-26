using System;
using System.Linq;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Update;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class SoilProfileUpdateExtensions
    {
        public static void Update(this PipingSoilProfile profile, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = GetSinglePipingSoilProfile(profile, context);
            entity.Name = profile.Name;
            entity.Bottom = Convert.ToDecimal(profile.Bottom);

            foreach (var pipingSoilLayer in profile.Layers)
            {
                if (pipingSoilLayer.IsNew())
                {
                    entity.SoilLayerEntities.Add(pipingSoilLayer.Create(collector));
                }
                else
                {
                    pipingSoilLayer.Update(collector, context);
                }
            }

            collector.Update(entity);
        }

        private static SoilProfileEntity GetSinglePipingSoilProfile(PipingSoilProfile profile, IRingtoetsEntities context)
        {
            try
            {
                return context.SoilProfileEntities.Single(spe => spe.SoilProfileEntityId == profile.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(SoilProfileEntity).Name, profile.StorageId), exception);
            } 
        }
    }
}