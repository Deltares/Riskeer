using System;
using System.Linq;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Update;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class PipingFailureMechanismUpdateExtensions
    {
        public static void Update(this PipingFailureMechanism mechanism, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = GetSingleFailureMechanism(mechanism, context);
            entity.IsRelevant = Convert.ToByte(mechanism.IsRelevant);

            foreach (var stochasticSoilModel in mechanism.StochasticSoilModels)
            {
                if (stochasticSoilModel.IsNew())
                {
                    entity.StochasticSoilModelEntities.Add(stochasticSoilModel.Create(collector));
                }
                else
                {
                    stochasticSoilModel.Update(collector, context);
                }
            }

            collector.Update(entity);
        }

        private static FailureMechanismEntity GetSingleFailureMechanism(PipingFailureMechanism mechanism, IRingtoetsEntities context)
        {
            try
            {
                return context.FailureMechanismEntities.Single(fme => fme.FailureMechanismEntityId == mechanism.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(FailureMechanismEntity).Name, mechanism.StorageId), exception);
            } 
        }
    }
}