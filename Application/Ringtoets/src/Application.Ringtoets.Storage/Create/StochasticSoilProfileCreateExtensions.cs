using System;
using Application.Ringtoets.Storage.Create;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class StochasticSoilProfileCreateExtensions
    {
        public static StochasticSoilProfileEntity Create(this StochasticSoilProfile profile, CreateConversionCollector collector)
        {
            var entity = new StochasticSoilProfileEntity
            {
                Probability = Convert.ToDecimal(profile.Probability),
                SoilProfileEntity = profile.SoilProfile.Create(collector)
            };

            collector.Create(entity, profile);
            return entity;
        }
    }
}