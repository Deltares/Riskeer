using System;
using System.Linq;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Update;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class StochasticSoilModelUpdateExtensions
    {
        public static void Update(this StochasticSoilModel model, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = GetSingleStochasticSoilModel(model, context);
            entity.Name = model.Name;
            entity.SegmentName = model.SegmentName;

            foreach (var stochasticSoilProfile in model.StochasticSoilProfiles)
            {
                if (stochasticSoilProfile.IsNew())
                {
                    entity.StochasticSoilProfileEntities.Add(stochasticSoilProfile.Create(collector));
                }
                else
                {
                    stochasticSoilProfile.Update(collector, context);
                }
            }

            collector.Update(entity);
        }

        private static StochasticSoilModelEntity GetSingleStochasticSoilModel(StochasticSoilModel model, IRingtoetsEntities context)
        {
            try
            {
                return context.StochasticSoilModelEntities.Single(ssme => ssme.StochasticSoilModelEntityId == model.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(StochasticSoilModelEntity).Name, model.StorageId), exception);
            } 
        }
    }
}