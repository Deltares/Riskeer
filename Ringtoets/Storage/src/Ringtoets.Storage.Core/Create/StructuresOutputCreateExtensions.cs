// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Storage.Core.Create.IllustrationPoints;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="StructuresOutput"/> related to creating structures 
    /// calculation output entities.
    /// </summary>
    internal static class StructuresOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="TOutputEntity"/> based on the information of the 
        /// <paramref name="structuresOutput"/>.
        /// </summary>
        /// <param name="structuresOutput">The structures output to create a database entity for.</param>
        /// <returns>A new <see cref="TOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="structuresOutput"/>
        /// is <c>null.</c></exception>
        public static TOutputEntity Create<TOutputEntity>(this StructuresOutput structuresOutput)
            where TOutputEntity : IStructuresOutputEntity,
            IHasGeneralResultFaultTreeIllustrationPointEntity,
            new()
        {
            if (structuresOutput == null)
            {
                throw new ArgumentNullException(nameof(structuresOutput));
            }

            var outputEntity = new TOutputEntity
            {
                Reliability = structuresOutput.Reliability.ToNaNAsNull()
            };

            SetGeneralResult(structuresOutput, outputEntity);

            return outputEntity;
        }

        private static void SetGeneralResult(StructuresOutput structuresOutput, IHasGeneralResultFaultTreeIllustrationPointEntity outputEntity)
        {
            if (structuresOutput.HasGeneralResult)
            {
                outputEntity.GeneralResultFaultTreeIllustrationPointEntity =
                    structuresOutput.GeneralResult
                                    .CreateGeneralResultFaultTreeIllustrationPointEntity();
            }
        }
    }
}