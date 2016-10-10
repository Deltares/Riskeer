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
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// This class contains validations methods for <see cref="StructuresParameterRow"/> objects.
    /// </summary>
    public static class StructuresParameterRowsValidator
    {
        private const string alphanumericalValueColumn = "AlphanumeriekeWaarde";
        private const string numericalValueColumn = "NumeriekeWaarde";
        private const string varianceValueColumn = "Standarddeviatie.variance";
        private const string varianceTypeColumn = "Boolean";

        /// <summary>
        /// Denotes a small enough value, taking possible rounding into account, that the
        /// value is too close to the value <c>0.0</c> that makes a coefficient of variation
        /// to unreliable.
        /// </summary>
        private const double valueTooCloseToZero = 1e-4;

        private static readonly List<string> closingStructureInflowModelRuleKeywords = new List<string>
        {
            "verticalewand",
            "lagedrempel",
            "verdronkenkoker"
        };

        private static readonly List<string> stabilityPointStructureInflowModelRuleKeywords = new List<string>
        {
            "lagedrempel",
            "verdronkenkoker"
        };

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
                    "KW_HOOGTE4", VariationCoefficientLogNormalDistributionRule
                },
                {
                    "KW_HOOGTE5", VariationCoefficientNormalDistributionRule
                },
                {
                    "KW_HOOGTE6", ProbabilityRule
                },
                {
                    "KW_HOOGTE7", VariationCoefficientLogNormalDistributionRule
                },
                {
                    "KW_HOOGTE8", LogNormalDistributionRule
                }
            };

        private static readonly Dictionary<string, Func<StructuresParameterRow, List<string>>> closingStructuresRules =
            new Dictionary<string, Func<StructuresParameterRow, List<string>>>
            {
                {
                    "KW_BETSLUIT1", VariationCoefficientLogNormalDistributionRule
                },
                {
                    "KW_BETSLUIT2", LogNormalDistributionRule
                },
                {
                    "KW_BETSLUIT3", StructureNormalOrientation
                },
                {
                    "KW_BETSLUIT4", VariationCoefficientNormalDistributionRule
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
                    "KW_BETSLUIT9", VariationCoefficientLogNormalDistributionRule
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
                    "KW_BETSLUIT13", IdenticalApertures
                },
                {
                    "KW_BETSLUIT14", ProbabilityRule
                },
                {
                    "KW_BETSLUIT15", ClosingStructureInflowModelRule
                }
            };

        private static readonly Dictionary<string, Func<StructuresParameterRow, List<string>>> stabilityPointStructuresRules =
            new Dictionary<string, Func<StructuresParameterRow, List<string>>>
            {
                {
                    "KW_STERSTAB1", StructureNormalOrientation
                },
                {
                    "KW_STERSTAB2", VariationCoefficientLogNormalDistributionRule
                },
                {
                    "KW_STERSTAB3", LogNormalDistributionRule
                },
                {
                    "KW_STERSTAB4", VariationCoefficientNormalDistributionRule
                },
                {
                    "KW_STERSTAB5", NormalDistributionRule
                },
                {
                    "KW_STERSTAB6", NormalDistributionRule
                },
                {
                    "KW_STERSTAB7", VariationCoefficientLogNormalDistributionRule
                },
                {
                    "KW_STERSTAB8", LogNormalDistributionRule
                },
                {
                    "KW_STERSTAB9", VariationCoefficientLogNormalDistributionRule
                },
                {
                    "KW_STERSTAB10", VariationCoefficientLogNormalDistributionRule
                },
                {
                    "KW_STERSTAB11", NormalDistributionRule
                },
                {
                    "KW_STERSTAB12", NormalDistributionRule
                },
                {
                    "KW_STERSTAB13", DoubleRule
                },
                {
                    "KW_STERSTAB14", NormalDistributionRule
                },
                {
                    "KW_STERSTAB15", PositiveDoubleRule
                },
                {
                    "KW_STERSTAB16", ProbabilityRule
                },
                {
                    "KW_STERSTAB17", PositiveDoubleRule
                },
                {
                    "KW_STERSTAB18", VariationCoefficientNormalDistributionRule
                },
                {
                    "KW_STERSTAB19", VariationCoefficientNormalDistributionRule
                },
                {
                    "KW_STERSTAB20", PositiveDoubleRule
                },
                {
                    "KW_STERSTAB21", PositiveDoubleRule
                },
                {
                    "KW_STERSTAB22", NormalDistributionRule
                },
                {
                    "KW_STERSTAB23", VariationCoefficientLogNormalDistributionRule
                },
                {
                    "KW_STERSTAB24", VariationCoefficientLogNormalDistributionRule
                },
                {
                    "KW_STERSTAB25", LogNormalDistributionRule
                },
                {
                    "KW_STERSTAB26", StabilityPointStructureInflowModelRule
                },
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

        /// <summary>
        /// Validates a collection of <see cref="StructuresParameterRow"/> for a stability point structure.
        /// </summary>
        /// <param name="structureParameterRows">The <see cref="StructuresParameterRow"/> objects to validate.</param>
        /// <returns>A <see cref="ValidationResult"/> object containing the validation result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="structureParameterRows"/> is <c>null</c>.</exception>
        public static ValidationResult ValidateStabilityPointStructuresParameters(IList<StructuresParameterRow> structureParameterRows)
        {
            return ValidateStructuresParameters(structureParameterRows, stabilityPointStructuresRules);
        }

        private static ValidationResult ValidateStructuresParameters(IList<StructuresParameterRow> structureParameterRows,
                                                                     Dictionary<string, Func<StructuresParameterRow, List<string>>> rules)
        {
            if (structureParameterRows == null)
            {
                throw new ArgumentNullException("structureParameterRows");
            }

            var errorMessages = new List<string>();

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

        private static List<string> DoubleRule(StructuresParameterRow row)
        {
            var messages = new List<string>();

            double value = row.NumericalValue;
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_probability_out_of_range, row.LineNumber, numericalValueColumn));
            }

            return messages;
        }

        private static List<string> PositiveDoubleRule(StructuresParameterRow row)
        {
            List<string> messages = new List<string>();

            double value = row.NumericalValue;
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_probability_out_of_range, row.LineNumber, numericalValueColumn));
            }

            return messages;
        }

        private static List<string> ProbabilityRule(StructuresParameterRow row)
        {
            var messages = new List<string>();

            double mean = row.NumericalValue;
            if (double.IsNaN(mean) || double.IsInfinity(mean) || mean <= 0 || mean > 1)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_probability_out_of_range, row.LineNumber, numericalValueColumn));
            }

            return messages;
        }

        private static List<string> NormalDistributionRule(StructuresParameterRow row)
        {
            return ValidateStochasticVariableParameters(row, false, true);
        }

        private static List<string> VariationCoefficientNormalDistributionRule(StructuresParameterRow row)
        {
            return ValidateStochasticVariableParameters(row, false, false);
        }

        private static List<string> LogNormalDistributionRule(StructuresParameterRow row)
        {
            return ValidateStochasticVariableParameters(row, true, true);
        }

        private static List<string> VariationCoefficientLogNormalDistributionRule(StructuresParameterRow row)
        {
            return ValidateStochasticVariableParameters(row, true, false);
        }

        private static List<string> ValidateStochasticVariableParameters(StructuresParameterRow row, bool meanMustBeGreaterThanZero, bool variationAsStandardDeviation)
        {
            var messages = new List<string>();

            double mean = row.NumericalValue;
            if (double.IsNaN(mean) || double.IsInfinity(mean) || (meanMustBeGreaterThanZero && mean <= 0))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_value_invalid, row.LineNumber, numericalValueColumn));
            }

            VarianceType type = row.VarianceType;
            if (type != VarianceType.StandardDeviation && type != VarianceType.CoefficientOfVariation)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_value_invalid, row.LineNumber, varianceTypeColumn));
            }

            double variance = row.VarianceValue;
            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0.0)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_value_invalid, row.LineNumber, varianceValueColumn));
            }

            if (variationAsStandardDeviation)
            {
                if (row.VarianceType == VarianceType.CoefficientOfVariation && mean < valueTooCloseToZero)
                {
                    messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Mean_on_Line_0_ColumnName_1_causes_unreliable_variation_value_conversion,
                                               row.LineNumber, StructureFilesKeywords.NumericalValueColumnName));
                }
            }
            else
            {
                if (row.VarianceType == VarianceType.StandardDeviation && mean < valueTooCloseToZero)
                {
                    messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Mean_on_Line_0_ColumnName_1_causes_unreliable_variation_value_conversion,
                                               row.LineNumber, StructureFilesKeywords.NumericalValueColumnName));
                }
            }

            return messages;
        }

        private static List<string> StructureNormalOrientation(StructuresParameterRow row)
        {
            var messages = new List<string>();

            double orientation = row.NumericalValue;
            if (!(orientation >= 0 && orientation <= 360))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_orientation_out_of_range, row.LineNumber, numericalValueColumn));
            }

            return messages;
        }

        private static List<string> IdenticalApertures(StructuresParameterRow row)
        {
            var messages = new List<string>();
            double value = row.NumericalValue;
            if (value < 0)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_value_cannot_be_smaller_than_zero, row.LineNumber, numericalValueColumn));
            }
            return messages;
        }

        private static List<string> ClosingStructureInflowModelRule(StructuresParameterRow row)
        {
            var messages = new List<string>();
            string value = row.AlphanumericValue.ToLower();
            if (!closingStructureInflowModelRuleKeywords.Contains(value))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_value_invalid, row.LineNumber, alphanumericalValueColumn));
            }
            return messages;
        }

        private static List<string> StabilityPointStructureInflowModelRule(StructuresParameterRow row)
        {
            var messages = new List<string>();
            string value = row.AlphanumericValue.ToLower();
            if (!stabilityPointStructureInflowModelRuleKeywords.Contains(value))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_Line_0_ColumnName_1_value_invalid, row.LineNumber, alphanumericalValueColumn));
            }
            return messages;
        }
    }
}