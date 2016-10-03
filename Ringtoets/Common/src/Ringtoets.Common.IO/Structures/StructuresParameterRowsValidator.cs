// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// This class contains validations methods for <see cref="StructuresParameterRow"/> objects.
    /// </summary>
    public static class StructuresParameterRowsValidator
    {
        private static readonly Dictionary<string, Func<StructuresParameterRow, List<string>>> heightStructuresRules =
            new Dictionary<string, Func<StructuresParameterRow, List<string>>>
            {
                {
                    "KW_HOOGTE1", OrientationOfTheNormalOfTheStructure
                },
                {
                    "KW_HOOGTE2", LevelOfCrestOfStructure
                },
                {
                    "KW_HOOGTE3", FlowWidthAtBottomProtection
                },
                {
                    "KW_HOOGTE4", CriticalOvertoppingDischarge
                },
                {
                    "KW_HOOGTE5", WidthOfFlowApertures
                },
                {
                    "KW_HOOGTE6", FailureProbabilityOfStructureGivenErosion
                },
                {
                    "KW_HOOGTE7", StorageStructureArea
                },
                {
                    "KW_HOOGTE8", AllowableIncreaseOfLevelForStorage
                }
            };

        /// <summary>
        /// Validates a collection of <see cref="StructuresParameterRow"/>.
        /// </summary>
        /// <param name="structureParameterRows">The <see cref="StructuresParameterRow"/> objects to validate.</param>
        /// <returns>A <see cref="ValidationResult"/> object containing the validation result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="structureParameterRows"/> is <c>null</c>.</exception>
        public static ValidationResult ValidateHeightStructuresParameters(IList<StructuresParameterRow> structureParameterRows)
        {
            if (structureParameterRows == null)
            {
                throw new ArgumentNullException("structureParameterRows");
            }

            List<string> errorMessages = new List<string>();

            foreach (string name in heightStructuresRules.Keys)
            {
                int count = structureParameterRows.Count(row => row.ParameterId.Equals(name));

                if (count < 1)
                {
                    errorMessages.Add(string.Format("Parameter '{0}' ontbreekt.", name));
                    continue;
                }

                if (count > 1)
                {
                    errorMessages.Add(string.Format("Parameter '{0}' komt meermaals voor.", name));
                }

                errorMessages.AddRange(heightStructuresRules[name](structureParameterRows.First(row => row.ParameterId.Equals(name))));
            }

            return new ValidationResult(errorMessages);
        }

        #region HeightStructuesRules

        private static List<string> OrientationOfTheNormalOfTheStructure(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double orientation = row.NumericalValue;
            if (!(orientation >= 0 && orientation <= 360))
            {
                messages.Add("De oriëntatie van het kunstwerk valt buiten het bereik [0, 360].");
            }
            return messages;
        }

        private static List<string> LevelOfCrestOfStructure(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double meanCrestLevel = row.NumericalValue;
            if (double.IsNaN(meanCrestLevel) || double.IsInfinity(meanCrestLevel))
            {
                messages.Add("De kerende hoogte van het kunstwerk heeft een ongeldige waarde.");
            }
            double variance = row.VarianceValue;
            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0.0 || row.VarianceType != VarianceType.StandardDeviation)
            {
                messages.Add("De standaard afwijking van de kerende hoogte normaalverdeling heeft een ongeldige waarde.");
            }
            return messages;
        }

        private static List<string> FlowWidthAtBottomProtection(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double meanFlowWidth = row.NumericalValue;
            if (double.IsNaN(meanFlowWidth) || double.IsInfinity(meanFlowWidth))
            {
                messages.Add("De stroomvoerende breedte bij bodembescherming van het kunstwerk heeft een ongeldige waarde.");
            }
            double variance = row.VarianceValue;
            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0.0 || row.VarianceType != VarianceType.CoefficientOfVariation)
            {
                messages.Add("De variantie van de stroomvoerende breedte bij bodembescherming lognormaalverdeling heeft een ongeldige waarde.");
            }
            return messages;
        }

        private static List<string> CriticalOvertoppingDischarge(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double meanDischange = row.NumericalValue;
            if (double.IsNaN(meanDischange) || double.IsInfinity(meanDischange))
            {
                messages.Add("Het kritieke overslagdebiet per strekkende meter van het kunstwerk heeft een ongeldige waarde.");
            }
            double variance = row.VarianceValue;
            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0.0 || row.VarianceType != VarianceType.CoefficientOfVariation)
            {
                messages.Add("De variantie van de kritieke overslagdebiet per strekkende meter lognormaalverdeling heeft een ongeldige waarde.");
            }
            return messages;
        }

        private static List<string> WidthOfFlowApertures(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double meanWidth = row.NumericalValue;
            if (double.IsNaN(meanWidth) || double.IsInfinity(meanWidth))
            {
                messages.Add("De breedte van de kruin van het kunstwerk heeft een ongeldige waarde.");
            }
            double variance = row.VarianceValue;
            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0.0 || row.VarianceType != VarianceType.StandardDeviation)
            {
                messages.Add("De standaard afwijking van de breedte van de kruin normaalverdeling heeft een ongeldige waarde.");
            }
            return messages;
        }

        private static List<string> FailureProbabilityOfStructureGivenErosion(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double failureProbability = row.NumericalValue;
            if (failureProbability < 0.0 || failureProbability > 1.0)
            {
                messages.Add("De waarde voor de faalkans van het kunstwerk valt buiten het bereik [0, 1].");
            }
            return messages;
        }

        private static List<string> StorageStructureArea(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double meanArea = row.NumericalValue;
            if (double.IsNaN(meanArea) || double.IsInfinity(meanArea))
            {
                messages.Add("Het kombergend oppervlak van het kunstwerk heeft een ongeldige waarde.");
            }
            double variance = row.VarianceValue;
            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0.0 || row.VarianceType != VarianceType.CoefficientOfVariation)
            {
                messages.Add("De variantie van de kombergend oppervlak lognormaalverdeling heeft een ongeldige waarde.");
            }
            return messages;
        }

        private static List<string> AllowableIncreaseOfLevelForStorage(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double meanAllowableIncrease = row.NumericalValue;
            if (double.IsNaN(meanAllowableIncrease) || double.IsInfinity(meanAllowableIncrease))
            {
                messages.Add("De toegestane peilverhoging op het kombergend oppervlak van het kunstwerk heeft een ongeldige waarde.");
            }
            double variance = row.VarianceValue;
            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0.0 || row.VarianceType != VarianceType.CoefficientOfVariation)
            {
                messages.Add("De variantie van de toegestane peilverhoging op het kombergend oppervlak lognormaalverdeling heeft een ongeldige waarde.");
            }
            return messages;
        }

        #endregion
    }
}