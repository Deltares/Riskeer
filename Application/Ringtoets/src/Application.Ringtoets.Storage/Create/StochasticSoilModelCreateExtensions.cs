using System;
using Application.Ringtoets.Storage.Create;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class StochasticSoilModelCreateExtensions
    {
        public static StochasticSoilModelEntity Create(this StochasticSoilModel model, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new StochasticSoilModelEntity
            {
                Name = model.Name,
                SegmentName = model.SegmentName
            };

            foreach (var stochasticSoilProfile in model.StochasticSoilProfiles)
            {
                entity.StochasticSoilProfileEntities.Add(stochasticSoilProfile.Create(collector));
            }

            collector.Add(entity, model);
            return entity;
        }
    }
}