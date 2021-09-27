// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.Linq;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.DuneErosion.Data;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// This class helps in obtaining unique display names for different types of calculations within
    /// an <see cref="IAssessmentSection"/>.
    /// </summary>
    public static class CalculationsDisplayNameHelper
    {
        private static readonly NoProbabilityValueDoubleConverter noProbabilityValueDoubleConverter = new NoProbabilityValueDoubleConverter();

        /// <summary>
        /// Gets a unique water level calculations display name for the provided <paramref name="calculations"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculations"/> belong to.</param>
        /// <param name="calculations">The water level calculations to get the unique display name for.</param>
        /// <returns>A unique water level calculations display name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="calculations"/> is not part of the water level
        /// calculations within <paramref name="assessmentSection"/>.</exception>
        public string GetUniqueDisplayNameForWaterLevelCalculations(IAssessmentSection assessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            var nonUniqueWaterLevelCalculationsDisplayNameLookup = new Dictionary<object, string>
            {
                {
                    assessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                    noProbabilityValueDoubleConverter.ConvertToString(assessmentSection.FailureMechanismContribution.LowerLimitNorm)
                },
                {
                    assessmentSection.WaterLevelCalculationsForSignalingNorm,
                    noProbabilityValueDoubleConverter.ConvertToString(assessmentSection.FailureMechanismContribution.SignalingNorm)
                }
            };

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability in assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                nonUniqueWaterLevelCalculationsDisplayNameLookup.Add(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations,
                                                                     noProbabilityValueDoubleConverter.ConvertToString(calculationsForTargetProbability.TargetProbability));
            }

            if (!nonUniqueWaterLevelCalculationsDisplayNameLookup.ContainsKey(calculations))
            {
                throw new InvalidOperationException("The provided calculations object is not part of the water level calculations within the assessment section.");
            }

            return GetUniqueDisplayNameLookup(nonUniqueWaterLevelCalculationsDisplayNameLookup)[calculations];
        }

        /// <summary>
        /// Gets a unique wave height calculations display name for the provided <paramref name="calculations"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculations"/> belong to.</param>
        /// <param name="calculations">The wave height calculations to get the unique display name for.</param>
        /// <returns>A unique wave height calculations display name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="calculations"/> is not part of the wave height
        /// calculations within <paramref name="assessmentSection"/>.</exception>
        public string GetUniqueDisplayNameForWaveHeightCalculations(IAssessmentSection assessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            Dictionary<object, string> nonUniqueWaveHeightCalculationsDisplayNameLookup =
                assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                 .ToDictionary(whc => (object) whc.HydraulicBoundaryLocationCalculations,
                                               whc => noProbabilityValueDoubleConverter.ConvertToString(whc.TargetProbability));

            if (!nonUniqueWaveHeightCalculationsDisplayNameLookup.ContainsKey(calculations))
            {
                throw new InvalidOperationException("The provided calculations object is not part of the wave height calculations within the assessment section.");
            }

            return GetUniqueDisplayNameLookup(nonUniqueWaveHeightCalculationsDisplayNameLookup)[calculations];
        }

        /// <summary>
        /// Gets a unique dune location calculations display name for the provided <paramref name="calculations"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section the <paramref name="calculations"/> belong to.</param>
        /// <param name="calculations">The dune location calculations to get the unique display name for.</param>
        /// <returns>A unique dune location calculations display name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="calculations"/> is not part of the dune location
        /// calculations within <paramref name="assessmentSection"/>.</exception>
        public string GetUniqueDisplayNameForDuneLocationCalculations(IAssessmentSection assessmentSection, IEnumerable<DuneLocationCalculation> calculations)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            Dictionary<object, string> nonUniqueDuneLocationCalculationsDisplayNameLookup =
                assessmentSection.GetFailureMechanisms()
                                 .OfType<DuneErosionFailureMechanism>()
                                 .First()
                                 .DuneLocationCalculationsForUserDefinedTargetProbabilities
                                 .ToDictionary(whc => (object) whc.DuneLocationCalculations,
                                               whc => noProbabilityValueDoubleConverter.ConvertToString(whc.TargetProbability));

            if (!nonUniqueDuneLocationCalculationsDisplayNameLookup.ContainsKey(calculations))
            {
                throw new InvalidOperationException("The provided calculations object is not part of the dune location calculations within the assessment section.");
            }

            return GetUniqueDisplayNameLookup(nonUniqueDuneLocationCalculationsDisplayNameLookup)[calculations];
        }

        private static Dictionary<object, string> GetUniqueDisplayNameLookup(IDictionary<object, string> nonUniqueDisplayNameLookup)
        {
            var uniqueDisplayNameLookup = new Dictionary<object, string>();

            while (nonUniqueDisplayNameLookup.Any())
            {
                KeyValuePair<object, string> firstElement = nonUniqueDisplayNameLookup.First();

                IList<KeyValuePair<object, string>> elementsWithSameDisplayNameAsFirstElement = nonUniqueDisplayNameLookup.Where(e => e.Value.Equals(firstElement.Value))
                                                                                                                          .ToList();

                if (elementsWithSameDisplayNameAsFirstElement.Count > 1)
                {
                    for (var i = 0; i < elementsWithSameDisplayNameAsFirstElement.Count; i++)
                    {
                        KeyValuePair<object, string> elementWithNonUniqueDisplayName = elementsWithSameDisplayNameAsFirstElement.ElementAt(i);

                        uniqueDisplayNameLookup.Add(elementWithNonUniqueDisplayName.Key, elementWithNonUniqueDisplayName.Value + $"({i + 1})");
                        nonUniqueDisplayNameLookup.Remove(elementWithNonUniqueDisplayName.Key);
                    }
                }
                else
                {
                    uniqueDisplayNameLookup.Add(firstElement.Key, firstElement.Value);
                    nonUniqueDisplayNameLookup.Remove(firstElement.Key);
                }
            }

            return uniqueDisplayNameLookup;
        }
    }
}