using System;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Create
{
    public static class FailureMechanismPlaceholderCreateExtensions
    {
        public static FailureMechanismEntity Create(this FailureMechanismPlaceholder mechanism, FailureMechanismType type, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            FailureMechanismEntity entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short)type,
                IsRelevant = Convert.ToByte(mechanism.IsRelevant)
            };

            collector.Add(entity, mechanism);
            return entity;
        }
    }
}