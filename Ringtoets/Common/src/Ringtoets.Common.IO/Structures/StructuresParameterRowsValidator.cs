﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// This class contains validations methods for <see cref="StructuresParameterRow"/> objects.
    /// </summary>
    public static class StructuresParameterRowsValidator
    {
        private const int numericalValueColumn = 18;
        private const int varianceValueColumn = 19;
        private const int varianceTypeColumn = 20;

        private static readonly Dictionary<string, Func<StructuresParameterRow, List<string>>> heightStructuresRules =
            new Dictionary<string, Func<StructuresParameterRow, List<string>>>
            {
                {
                    "KW_HOOGTE1", StructureNormalOrientation
                },
                {
                    "KW_HOOGTE2", NormalDistributionRule
                },
                {
                    "KW_HOOGTE3", LogNormalDistributionRule
                },
                {
                    "KW_HOOGTE4", LogNormalDistributionRule
                },
                {
                    "KW_HOOGTE5", NormalDistributionRule
                },
                {
                    "KW_HOOGTE6", ProbabilityRule
                },
                {
                    "KW_HOOGTE7", LogNormalDistributionRule
                },
                {
                    "KW_HOOGTE8", LogNormalDistributionRule
                }
            };

        private static readonly Dictionary<string, Func<StructuresParameterRow, List<string>>> closingStructuresRules =
            new Dictionary<string, Func<StructuresParameterRow, List<string>>>
            {
                {
                    "KW_BETSLUIT1", LogNormalDistributionRule
                },
                {
                    "KW_BETSLUIT2", LogNormalDistributionRule
                },
                {
                    "KW_BETSLUIT3", StructureNormalOrientation
                },
                {
                    "KW_BETSLUIT4", NormalDistributionRule
                },
                {
                    "KW_BETSLUIT5", NormalDistributionRule
                },
                {
                    "KW_BETSLUIT6", NormalDistributionRule
                },
                {
                    "KW_BETSLUIT7", NormalDistributionRule
                },
                {
                    "KW_BETSLUIT8", LogNormalDistributionRule
                },
                {
                    "KW_BETSLUIT9", LogNormalDistributionRule
                },
                {
                    "KW_BETSLUIT10", LogNormalDistributionRule
                },
                {
                    "KW_BETSLUIT11", ProbabilityRule
                },
                {
                    "KW_BETSLUIT12", ProbabilityRule
                },
                {
                    "KW_BETSLUIT13", NumberOfIdenticalApertures
                },
                {
                    "KW_BETSLUIT14", ProbabilityRule
                },
                {
                    "KW_BETSLUIT15", InflowModel
                }
            };

        /// <summary>
        /// Validates a collection of <see cref="StructuresParameterRow"/> for a height structure.
        /// </summary>
        /// <param name="structureParameterRows">The <see cref="StructuresParameterRow"/> objects to validate.</param>
        /// <returns>A <see cref="ValidationResult"/> object containing the validation result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="structureParameterRows"/> is <c>null</c>.</exception>
        public static ValidationResult ValidateHeightStructuresParameters(IList<StructuresParameterRow> structureParameterRows)
        {
            return ValidateStructuresParameters(structureParameterRows, heightStructuresRules);
        }

        /// <summary>
        /// Validates a collection of <see cref="StructuresParameterRow"/> for a closing structure.
        /// </summary>
        /// <param name="structureParameterRows">The <see cref="StructuresParameterRow"/> objects to validate.</param>
        /// <returns>A <see cref="ValidationResult"/> object containing the validation result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="structureParameterRows"/> is <c>null</c>.</exception>
        public static ValidationResult ValidateClosingStructuresParameters(IList<StructuresParameterRow> structureParameterRows)
        {
            return ValidateStructuresParameters(structureParameterRows, closingStructuresRules);
        }

        private static ValidationResult ValidateStructuresParameters(IList<StructuresParameterRow> structureParameterRows,
                                                                     Dictionary<string, Func<StructuresParameterRow, List<string>>> rules)
        {
            if (structureParameterRows == null)
            {
                throw new ArgumentNullException("structureParameterRows");
            }

            List<string> errorMessages = new List<string>();

            foreach (string name in rules.Keys)
            {
                int count = structureParameterRows.Count(row => string.Equals(row.ParameterId, name, StringComparison.OrdinalIgnoreCase));

                if (count < 1)
                {
                    errorMessages.Add(string.Format(Resources.StructuresParameterRowsValidator_Parameter_0_missing, name));
                    continue;
                }

                if (count > 1)
                {
                    errorMessages.Add(string.Format(Resources.StructuresParameterRowsValidator_Parameter_0_repeated, name));
                }

                errorMessages.AddRange(rules[name](structureParameterRows.First(row => string.Equals(row.ParameterId, name, StringComparison.OrdinalIgnoreCase))));
            }

            return new ValidationResult(errorMessages);
        }

        private static List<string> ProbabilityRule(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();

            double mean = row.NumericalValue;
            if (double.IsNaN(mean) || double.IsInfinity(mean))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_probability_out_of_range, row.LineNumber, numericalValueColumn));
            }

            return messages;
        }

        private static List<string> NormalDistributionRule(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();

            double mean = row.NumericalValue;
            if (double.IsNaN(mean) || double.IsInfinity(mean))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_value_invalid, row.LineNumber, numericalValueColumn));
            }

            VarianceType type = row.VarianceType;
            if (type != VarianceType.StandardDeviation && type != VarianceType.CoefficientOfVariation)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_value_invalid, row.LineNumber, varianceTypeColumn));
            }

            double variance = row.VarianceValue;
            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0.0)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_value_invalid, row.LineNumber, varianceValueColumn));
            }

            return messages;
        }

        private static List<string> LogNormalDistributionRule(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();

            double mean = row.NumericalValue;
            if (double.IsNaN(mean) || double.IsInfinity(mean) || mean <= 0)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_value_invalid, row.LineNumber, numericalValueColumn));
            }

            VarianceType type = row.VarianceType;
            if (type != VarianceType.StandardDeviation && type != VarianceType.CoefficientOfVariation)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_value_invalid, row.LineNumber, varianceTypeColumn));
            }

            double variance = row.VarianceValue;
            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0.0)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_value_invalid, row.LineNumber, varianceValueColumn));
            }

            return messages;
        }

        private static List<string> StructureNormalOrientation(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();

            double orientation = row.NumericalValue;
            if (!(orientation >= 0 && orientation <= 360))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_orientation_out_of_range, row.LineNumber, numericalValueColumn));
            }

            return messages;
        }

        private static List<string> NumberOfIdenticalApertures(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double value = row.NumericalValue;
            if (value < 0)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_value_cannot_be_smaller_than_zero, row.LineNumber, numericalValueColumn));
            }
            return messages;
        }

        private static List<string> InflowModel(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();
            double value = row.NumericalValue;
            if (!(value >= 0) && (value <= 2))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_column_1_type_out_of_range, row.LineNumber, numericalValueColumn));
            }
            return messages;
        }
    }
}