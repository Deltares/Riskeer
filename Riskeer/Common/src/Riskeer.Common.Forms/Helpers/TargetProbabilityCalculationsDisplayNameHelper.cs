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

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// This class helps in obtaining unique display names for different types of calculations that can be identified
    /// by their target probability.
    /// </summary>
    public static class TargetProbabilityCalculationsDisplayNameHelper
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
        public static string GetUniqueDisplayNameForWaterLevelCalculations(IAssessmentSection assessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
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
        /// Gets a unique display name for the provided <paramref name="calculations"/>.
        /// </summary>
        /// <param name="allCalculations">The enumeration of all calculations (containing <paramref name="calculations"/>).</param>
        /// <param name="calculations">The calculations to get the unique display name for.</param>
        /// <param name="getTargetProbabilityFunc">The function to obtain the target probability for elements within <paramref name="allCalculations"/>.</param>
        /// <returns>A unique calculations display name.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="calculations"/> is not part of <paramref name="allCalculations"/>.</exception>
        public static string GetUniqueDisplayNameForCalculations<T>(T calculations, IEnumerable<T> allCalculations, Func<T, double> getTargetProbabilityFunc)
        {
            if (allCalculations == null)
            {
                throw new ArgumentNullException(nameof(allCalculations));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            Dictionary<object, string> nonUniqueCalculationsDisplayNameLookup =
                allCalculations.ToDictionary(c => (object) c,
                                             c => noProbabilityValueDoubleConverter.ConvertToString(getTargetProbabilityFunc(c)));

            if (!nonUniqueCalculationsDisplayNameLookup.ContainsKey(calculations))
            {
                throw new InvalidOperationException("The provided calculations object is not part of the enumeration.");
            }

            return GetUniqueDisplayNameLookup(nonUniqueCalculationsDisplayNameLookup)[calculations];
        }

        private static Dictionary<object, string> GetUniqueDisplayNameLookup(IDictionary<object, string> nonUniqueDisplayNameLookup)
        {
            var uniqueDisplayNameLookup = new Dictionary<object, string>();

            while (nonUniqueDisplayNameLookup.Any())
            {
                KeyValuePair<object, string> firstElement = nonUniqueDisplayNameLookup.First();

                IList<KeyValuePair<object, string>> elementsWithSameDisplayNameAsFirstElement = nonUniqueDisplayNameLookup.Where(e => e.Value.Equals(firstElement.Value))
                                                                                                                          .ToList();

                uniqueDisplayNameLookup.Add(firstElement.Key, firstElement.Value);
                nonUniqueDisplayNameLookup.Remove(firstElement.Key);

                if (elementsWithSameDisplayNameAsFirstElement.Count > 1)
                {
                    for (var i = 1; i < elementsWithSameDisplayNameAsFirstElement.Count; i++)
                    {
                        KeyValuePair<object, string> elementWithNonUniqueDisplayName = elementsWithSameDisplayNameAsFirstElement.ElementAt(i);

                        uniqueDisplayNameLookup.Add(elementWithNonUniqueDisplayName.Key, elementWithNonUniqueDisplayName.Value + $" ({i})");
                        nonUniqueDisplayNameLookup.Remove(elementWithNonUniqueDisplayName.Key);
                    }
                }
            }

            return uniqueDisplayNameLookup;
        }
    }
}