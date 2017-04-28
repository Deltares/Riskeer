// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Probability;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="ProbabilityAssessmentOutput"/> related to creating an
    /// <see cref="IProbabilityAssessmentOutputEntity"/>.
    /// </summary>
    internal static class ProbabilityAssessmentOutputCreateExtensions
    {
        /// <summary>
        /// Creates an <see cref="IProbabilityAssessmentOutputEntity"/> based on
        /// the information of the <see cref="ProbabilityAssessmentOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output to create a database entity for.</param>
        /// <returns>A new <see cref="IProbabilityAssessmentOutputEntity"/>.</returns>
        internal static T Create<T>(this ProbabilityAssessmentOutput output)
            where T : IProbabilityAssessmentOutputEntity, new()
        {
            return new T
            {
                RequiredProbability = output.RequiredProbability.ToNaNAsNull(),
                RequiredReliability = output.RequiredReliability.ToNaNAsNull(),
                Probability = output.Probability.ToNaNAsNull(),
                Reliability = output.Reliability.ToNaNAsNull(),
                FactorOfSafety = output.FactorOfSafety.ToNaNAsNull()
            };
        }
    }
}