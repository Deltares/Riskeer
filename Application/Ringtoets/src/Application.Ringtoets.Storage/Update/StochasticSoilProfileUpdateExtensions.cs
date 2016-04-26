using System;
using System.Linq;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Update;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class StochasticSoilProfileUpdateExtensions
    {
        public static void Update(this StochasticSoilProfile profile, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = GetSingleStochasticSoilProfile(profile, context);
            entity.Probability = Convert.ToDecimal(profile.Probability);

            if (profile.SoilProfile.IsNew())
            {
                entity.SoilProfileEntity = profile.SoilProfile.Create(collector);
            }
            else
            {
                profile.SoilProfile.Update(collector, context);
            }

            collector.Update(entity);
        }

        private static StochasticSoilProfileEntity GetSingleStochasticSoilProfile(StochasticSoilProfile profile, IRingtoetsEntities context)
        {
            try
            {
                return context.StochasticSoilProfileEntities.Single(sspe => sspe.StochasticSoilProfileEntityId == profile.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(StochasticSoilProfileEntity).Name, profile.StorageId), exception);
            } 
        }
    }
}