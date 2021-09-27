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
    /// This class helps in uniquely naming the different hydraulic boundary location calculations within
    /// an <see cref="IAssessmentSection"/>.
    /// </summary>
    public static class HydraulicBoundaryLocationCalculationsNamingHelper
    {
        private static readonly NoProbabilityValueDoubleConverter noProbabilityValueDoubleConverter = new NoProbabilityValueDoubleConverter();

        public string GetUniqueNameForWaterLevelCalculations(IAssessmentSection assessmentSection, IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            var nonUniqueWaterLevelCalculationsNameLookup = new Dictionary<object, string>
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
                nonUniqueWaterLevelCalculationsNameLookup.Add(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations,
                                                              noProbabilityValueDoubleConverter.ConvertToString(calculationsForTargetProbability.TargetProbability));
            }

            if (!nonUniqueWaterLevelCalculationsNameLookup.ContainsKey(calculations))
            {
                throw new InvalidOperationException("The provided calculations object is not part of the water level calculations within the assessment section.");
            }

            return GetUniqueNameLookup(nonUniqueWaterLevelCalculationsNameLookup)[calculations];
        }

        private static Dictionary<object, string> GetUniqueNameLookup(IDictionary<object, string> nonUniqueNameLookup)
        {
            var uniqueNameLookup = new Dictionary<object, string>();

            while (nonUniqueNameLookup.Any())
            {
                KeyValuePair<object, string> firstElement = nonUniqueNameLookup.First();

                IList<KeyValuePair<object, string>> elementsWithNonUniqueName = nonUniqueNameLookup.Where(e => e.Value.Equals(firstElement.Value))
                                                                                                   .ToList();

                if (elementsWithNonUniqueName.Count > 1)
                {
                    for (var i = 0; i < elementsWithNonUniqueName.Count; i++)
                    {
                        KeyValuePair<object, string> elementWithNonUniqueName = elementsWithNonUniqueName.ElementAt(i);

                        uniqueNameLookup.Add(elementWithNonUniqueName.Key, elementWithNonUniqueName.Value + $"({i + 1})");
                        nonUniqueNameLookup.Remove(elementWithNonUniqueName.Key);
                    }
                }
                else
                {
                    uniqueNameLookup.Add(firstElement.Key, firstElement.Value);
                    nonUniqueNameLookup.Remove(firstElement.Key);
                }
            }

            return uniqueNameLookup;
        }
    }
}