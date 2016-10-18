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
        /// <summary>
        /// Denotes a small enough value, taking possible rounding into account, that the
        /// value is too close to the value <c>0.0</c> that makes a coefficient of variation
        /// too unreliable.
        /// </summary>
        private const double valueTooCloseToZero = 1e-4;

        private static readonly List<string> closingStructureInflowModelTypeRuleKeywords = new List<string>
        {
            "verticalewand",
            "lagedrempel",
            "verdronkenkoker"
        };

        private static readonly List<string> stabilityPointStructureInflowModelTypeRuleKeywords = new List<string>
        {
            "lagedrempel",
            "verdronkenkoker"
        };

        private static readonly Dictionary<string, Func<StructuresParameterRow, List<string>>> heightStructuresRules =
            new Dictionary<string, Func<StructuresParameterRow, List<string>>>
            {
                {
                    StructureFilesKeywords.HeightStructureParameterKeyword1, StructureNormalOrientation
                },
                {
                    StructureFilesKeywords.HeightStructureParameterKeyword2, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.HeightStructureParameterKeyword3, LogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.HeightStructureParameterKeyword4, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.HeightStructureParameterKeyword5, VariationCoefficientNormalDistributionRule
                },
                {
                    StructureFilesKeywords.HeightStructureParameterKeyword6, ProbabilityRule
                },
                {
                    StructureFilesKeywords.HeightStructureParameterKeyword7, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.HeightStructureParameterKeyword8, LogNormalDistributionRule
                }
            };

        private static readonly Dictionary<string, Func<StructuresParameterRow, List<string>>> closingStructuresRules =
            new Dictionary<string, Func<StructuresParameterRow, List<string>>>
            {
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword1, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword2, LogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword3, StructureNormalOrientation
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword4, VariationCoefficientNormalDistributionRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword5, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword6, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword7, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword8, LogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword9, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword10, LogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword11, ProbabilityRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword12, ProbabilityRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword13, IdenticalApertures
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword14, ProbabilityRule
                },
                {
                    StructureFilesKeywords.ClosingStructureParameterKeyword15, ClosingStructureInflowModelTypeRule
                }
            };

        private static readonly Dictionary<string, Func<StructuresParameterRow, List<string>>> stabilityPointStructuresRules =
            new Dictionary<string, Func<StructuresParameterRow, List<string>>>
            {
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword1, StructureNormalOrientation
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword2, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword3, LogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword4, VariationCoefficientNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword5, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword6, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword7, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword8, LogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword9, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword10, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword11, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword12, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword13, DoubleRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword14, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword15, PositiveDoubleRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword16, ProbabilityRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword17, PositiveDoubleRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword18, VariationCoefficientNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword19, VariationCoefficientNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword20, PositiveDoubleRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword21, PositiveDoubleRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword22, NormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword23, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword24, VariationCoefficientLogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword25, LogNormalDistributionRule
                },
                {
                    StructureFilesKeywords.StabilityPointStructureParameterKeyword26, StabilityPointStructureInflowModelTypeRule
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
            return ValidateDoubleParameter(row, StructureFilesKeywords.NumericalValueColumnName);
        }

        private static List<string> PositiveDoubleRule(StructuresParameterRow row)
        {
            return ValidatePositiveDoubleParameter(row, StructureFilesKeywords.NumericalValueColumnName);
        }

        private static List<string> ValidateDoubleParameter(StructuresParameterRow row, string columnName)
        {
            var messages = new List<string>();

            double value = GetValueFromRowForColumn(row, columnName);
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ValidateDoubleParameter_ParameterId_0_Line_1_ColumnName_2_not_number,
                                           row.ParameterId, row.LineNumber, columnName));
            }

            return messages;
        }

        private static List<string> ValidatePositiveDoubleParameter(StructuresParameterRow row, string columnName)
        {
            var messages = new List<string>();

            double value = GetValueFromRowForColumn(row, columnName);
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ValidatePositiveDoubleParameter_ParameterId_0_Line_1_ColumnName_2_must_be_a_positive_number,
                                           row.ParameterId, row.LineNumber, columnName));
            }

            return messages;
        }

        private static double GetValueFromRowForColumn(StructuresParameterRow row, string columnName)
        {
            switch (columnName)
            {
                case StructureFilesKeywords.NumericalValueColumnName:
                    return row.NumericalValue;
                case StructureFilesKeywords.VariationValueColumnName:
                    return row.VarianceValue;
                default:
                    throw new NotImplementedException();
            }
        }

        private static List<string> ProbabilityRule(StructuresParameterRow row)
        {
            var messages = new List<string>();

            double mean = row.NumericalValue;
            if (double.IsNaN(mean) || double.IsInfinity(mean) || mean <= 0 || mean > 1)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ProbabilityRule_ParameterId_0_Line_1_ColumnName_2_probability_out_of_range,
                                           row.ParameterId, row.LineNumber, StructureFilesKeywords.NumericalValueColumnName));
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
            var numericalValueColumn1 = StructureFilesKeywords.NumericalValueColumnName;
            messages.AddRange(meanMustBeGreaterThanZero ?
                                  ValidatePositiveDoubleParameter(row, numericalValueColumn1) :
                                  ValidateDoubleParameter(row, numericalValueColumn1));

            VarianceType type = row.VarianceType;
            if (type != VarianceType.StandardDeviation && type != VarianceType.CoefficientOfVariation)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ParameterId_0_Line_1_ColumnName_2_invalid_variancetype_value,
                                           row.ParameterId, row.LineNumber, StructureFilesKeywords.VariationTypeColumnName));
            }

            messages.AddRange(ValidatePositiveDoubleParameter(row, StructureFilesKeywords.VariationValueColumnName));

            double absoluteMean = Math.Abs(mean);
            if (variationAsStandardDeviation)
            {
                if (row.VarianceType == VarianceType.CoefficientOfVariation && absoluteMean < valueTooCloseToZero)
                {
                    messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ParameterId_0_Line_1_ColumnName_2_mean_too_small_for_reliable_variation_value_conversion,
                                               row.ParameterId, row.LineNumber, StructureFilesKeywords.NumericalValueColumnName));
                }
            }
            else
            {
                if (row.VarianceType == VarianceType.StandardDeviation && absoluteMean < valueTooCloseToZero)
                {
                    messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ParameterId_0_Line_1_ColumnName_2_mean_too_small_for_reliable_variation_value_conversion,
                                               row.ParameterId, row.LineNumber, StructureFilesKeywords.NumericalValueColumnName));
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
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ParameterId_0_Line_1_ColumnName_2_orientation_out_of_range,
                                           row.ParameterId, row.LineNumber, StructureFilesKeywords.NumericalValueColumnName));
            }

            return messages;
        }

        private static List<string> IdenticalApertures(StructuresParameterRow row)
        {
            var messages = new List<string>();
            double value = row.NumericalValue;
            if (!IsValueWholeNumber(value) || value < 0)
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ParameterId_0_Line_1_ColumnName_2_value_must_be_positive_whole_number,
                                           row.ParameterId, row.LineNumber, StructureFilesKeywords.NumericalValueColumnName));
            }
            return messages;
        }

        private static bool IsValueWholeNumber(double value)
        {
            return (value%1) < double.Epsilon;
        }

        private static List<string> ClosingStructureInflowModelTypeRule(StructuresParameterRow row)
        {
            var messages = new List<string>();
            string value = row.AlphanumericValue.ToLower();
            if (!closingStructureInflowModelTypeRuleKeywords.Contains(value))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ParameterId_0_Line_1_ColumnName_2_structure_type_invalid,
                                           row.ParameterId, row.LineNumber, StructureFilesKeywords.AlphanumericalValueColumnName));
            }
            return messages;
        }

        private static List<string> StabilityPointStructureInflowModelTypeRule(StructuresParameterRow row)
        {
            var messages = new List<string>();
            string value = row.AlphanumericValue.ToLower();
            if (!stabilityPointStructureInflowModelTypeRuleKeywords.Contains(value))
            {
                messages.Add(string.Format(Resources.StructuresParameterRowsValidator_ParameterId_0_Line_1_ColumnName_2_structure_type_invalid,
                                           row.ParameterId, row.LineNumber, StructureFilesKeywords.AlphanumericalValueColumnName));
            }
            return messages;
        }
    }
}