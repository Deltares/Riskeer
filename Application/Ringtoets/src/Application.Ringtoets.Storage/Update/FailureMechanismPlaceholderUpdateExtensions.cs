using System;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Update
{
    public static class FailureMechanismPlaceholderUpdateExtensions
    {
        public static void Update(this FailureMechanismPlaceholder mechanism, UpdateConversionCollector collector, IRingtoetsEntities context)
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

            collector.Update(entity);
        }

        private static FailureMechanismEntity GetSingleFailureMechanism(FailureMechanismPlaceholder mechanism, IRingtoetsEntities context)
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