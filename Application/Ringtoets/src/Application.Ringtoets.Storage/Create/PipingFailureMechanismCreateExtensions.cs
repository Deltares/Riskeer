using System;
using Application.Ringtoets.Storage.Create;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class PipingFailureMechanismCreateExtensions
    {
        public static FailureMechanismEntity Create(this PipingFailureMechanism mechanism, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            FailureMechanismEntity entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short) FailureMechanismType.Piping,
                IsRelevant = Convert.ToByte(mechanism.IsRelevant)
            };

            CreateStochasticSoilModels(mechanism, collector, entity);

            collector.Add(entity, mechanism);
            return entity;
        }

        private static void CreateStochasticSoilModels(PipingFailureMechanism mechanism, CreateConversionCollector collector, FailureMechanismEntity entity)
        {
            foreach (var stochasticSoilModel in mechanism.StochasticSoilModels)
            {
                entity.StochasticSoilModelEntities.Add(stochasticSoilModel.Create(collector));
            }
        }
    }
}