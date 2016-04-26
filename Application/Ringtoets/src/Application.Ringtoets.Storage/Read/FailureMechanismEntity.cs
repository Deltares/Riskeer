using System;
using Application.Ringtoets.Storage.Read;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class FailureMechanismEntity
    {
        public PipingFailureMechanism ReadAsPipingFailureMechanism(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = FailureMechanismEntityId,
                IsRelevant = IsRelevant == 1
            };

            foreach (var stochasticSoilModelEntity in StochasticSoilModelEntities)
            {
                failureMechanism.StochasticSoilModels.Add(stochasticSoilModelEntity.Read(collector));
            }

            return failureMechanism;
        }
    }
}