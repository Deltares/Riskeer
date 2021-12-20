﻿using System;
using Core.Common.Base.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GeneralInput"/>
    /// based on the <see cref="StrengthStabilityLengthwiseConstructionFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class StrengthStabilityLengthwiseConstructionFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="StrengthStabilityLengthwiseConstructionFailureMechanismMetaEntity"/> and use the information to
        /// update the <see cref="GeneralInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StrengthStabilityLengthwiseConstructionFailureMechanismMetaEntity"/> to update
        /// <see cref="GeneralInput"/> for.</param>
        /// <param name="generalInput">The <see cref="GeneralInput"/> to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this StrengthStabilityLengthwiseConstructionFailureMechanismMetaEntity entity, GeneralInput generalInput)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            generalInput.N = (RoundedDouble) entity.N;
        }
    }
}