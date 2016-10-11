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
using NUnit.Framework;
using Ringtoets.Common.IO.Structures;

namespace Ringtoets.Common.IO.Test.Structures
{
    [TestFixture]
    public class StructuresParameterRowsValidatorTest
    {
        [Test]
        public void ValidateHeightStructuresParameters_StructureParameterRowsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StructuresParameterRowsValidator.ValidateHeightStructuresParameters(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structureParameterRows", paramName);
        }

        [Test]
        public void ValidateHeightStructuresParameters_ParameterIdsMissingOrDuplicated_ValidIsFalseAndErrorMessages()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword1,
                NumericalValue = 180.0
            };

            List<StructuresParameterRow> structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                structuresParameterRow
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            List<string> expectedErrorMessages = new List<string>
            {
                "Parameter 'KW_HOOGTE1' komt meermaals voor.",
                "Parameter 'KW_HOOGTE2' ontbreekt.",
                "Parameter 'KW_HOOGTE3' ontbreekt.",
                "Parameter 'KW_HOOGTE4' ontbreekt.",
                "Parameter 'KW_HOOGTE5' ontbreekt.",
                "Parameter 'KW_HOOGTE6' ontbreekt.",
                "Parameter 'KW_HOOGTE7' ontbreekt.",
                "Parameter 'KW_HOOGTE8' ontbreekt."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateHeightStructuresParameters_ParametersAllInvalid_ValidIsFalseAndErrorMessages()
        {
            // Setup
            List<StructuresParameterRow> structureParameterRows = new List<StructuresParameterRow>
            {
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword1,
                    NumericalValue = double.NaN,
                    LineNumber = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword2,
                    NumericalValue = double.NaN,
                    VarianceValue = -10.0,
                    LineNumber = 2
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword3,
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN,
                    LineNumber = 3
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword4,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = double.PositiveInfinity,
                    LineNumber = 4
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword5,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 5
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword6,
                    NumericalValue = double.NegativeInfinity,
                    LineNumber = 6
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword7,
                    NumericalValue = 9.9e-5,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 7
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword8,
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 8
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            List<string> expectedErrorMessages = new List<string>
            {
                "De waarde op regel 1, kolom 'NumeriekeWaarde' valt buiten het bereik [0, 360].",
                "De waarde op regel 2, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 2, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 2, kolom 'Standarddeviatie.variance' is ongeldig.",
                "De waarde op regel 3, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 3, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 3, kolom 'Standarddeviatie.variance' is ongeldig.",
                "De waarde op regel 4, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 4, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 4, kolom 'Standarddeviatie.variance' is ongeldig.",
                "De waarde op regel 5, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 5, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 6, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 7, kolom numeriekewaarde) niet te dicht op 0 zijn.",
                "De waarde op regel 8, kolom 'NumeriekeWaarde' is ongeldig.",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 8, kolom numeriekewaarde) niet te dicht op 0 zijn."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateHeightStructuresParameters_VarianceValueConversionWithNegativeMean_NoErrorMessage()
        {
            // Setup
            var parameters = new[]
            {
                // Parameters of interest for test:
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword2,
                    NumericalValue = -1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword5,
                    NumericalValue = -1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.StandardDeviation // Expected Coefficient of Variation for normal distribution
                },

                // Remainder:
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword1,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword3,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword4,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword6,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword7,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword8,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(parameters);

            // Assert
            Assert.IsTrue(validationResult.IsValid,
                          "Expected to be valid, but found following errors: {0}",
                          string.Join(Environment.NewLine, validationResult.ErrorMessages));
        }

        [Test]
        public void ValidateClosingStructuresParameters_StructureParameterRowsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StructuresParameterRowsValidator.ValidateClosingStructuresParameters(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structureParameterRows", paramName);
        }

        [Test]
        public void ValidateClosingStructuresParameters_ParameterIdsMissingOrDuplicated_ValidIsFalseAndErrorMessages()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = "KW_BETSLUIT3",
                NumericalValue = 180.0
            };

            List<StructuresParameterRow> structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                structuresParameterRow
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            List<string> expectedErrorMessages = new List<string>
            {
                "Parameter 'KW_BETSLUIT1' ontbreekt.",
                "Parameter 'KW_BETSLUIT2' ontbreekt.",
                "Parameter 'KW_BETSLUIT3' komt meermaals voor.",
                "Parameter 'KW_BETSLUIT4' ontbreekt.",
                "Parameter 'KW_BETSLUIT5' ontbreekt.",
                "Parameter 'KW_BETSLUIT6' ontbreekt.",
                "Parameter 'KW_BETSLUIT7' ontbreekt.",
                "Parameter 'KW_BETSLUIT8' ontbreekt.",
                "Parameter 'KW_BETSLUIT9' ontbreekt.",
                "Parameter 'KW_BETSLUIT10' ontbreekt.",
                "Parameter 'KW_BETSLUIT11' ontbreekt.",
                "Parameter 'KW_BETSLUIT12' ontbreekt.",
                "Parameter 'KW_BETSLUIT13' ontbreekt.",
                "Parameter 'KW_BETSLUIT14' ontbreekt.",
                "Parameter 'KW_BETSLUIT15' ontbreekt."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateClosingStructuresParameters_ParametersAllInvalid_ValidIsFalseAndErrorMessages()
        {
            // Setup
            List<StructuresParameterRow> structureParameterRows = new List<StructuresParameterRow>
            {
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT1",
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT2",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 2
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT3",
                    NumericalValue = double.NaN,
                    LineNumber = 3
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT4",
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 4
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT5",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 5
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT6",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 6
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT7",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 7
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT8",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = double.PositiveInfinity,
                    LineNumber = 8
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT9",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = double.PositiveInfinity,
                    LineNumber = 9
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT10",
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN,
                    LineNumber = 10
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT11",
                    NumericalValue = double.NegativeInfinity,
                    LineNumber = 11
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT12",
                    NumericalValue = double.NegativeInfinity,
                    LineNumber = 12
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT13",
                    NumericalValue = -11,
                    LineNumber = 13
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT14",
                    NumericalValue = double.NegativeInfinity,
                    LineNumber = 14
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT15",
                    AlphanumericValue = "oei",
                    LineNumber = 15
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            List<string> expectedErrorMessages = new List<string>
            {
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 1, kolom numeriekewaarde) niet te dicht op 0 zijn.",
                "De waarde op regel 2, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 3, kolom 'NumeriekeWaarde' valt buiten het bereik [0, 360].",
                "De waarde op regel 5, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 5, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 6, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 6, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 7, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 7, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 8, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 8, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 8, kolom 'Standarddeviatie.variance' is ongeldig.",
                "De waarde op regel 9, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 9, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 9, kolom 'Standarddeviatie.variance' is ongeldig.",
                "De waarde op regel 10, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 10, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 10, kolom 'Standarddeviatie.variance' is ongeldig.",
                "De waarde op regel 11, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "De waarde op regel 12, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "De waarde op regel 13, kolom 'NumeriekeWaarde' mag niet kleiner dan nul zijn.",
                "De waarde op regel 14, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "De waarde op regel 15, kolom 'AlphanumeriekeWaarde' is ongeldig."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateClosingStructuresParameters_VarianceValueConversionWithNegativeMean_NoErrorMessage()
        {
            // Setup
            var parameters = new[]
            {
                // Parameters of interest for test:
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword4,
                    NumericalValue = -1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.StandardDeviation // Expected Coefficient of Variation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword5,
                    NumericalValue = -1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword6,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword7,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },

                // Remainder:
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword1,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword2,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword3,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword8,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword9,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword10,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword11,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword12,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword13,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword14,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword15,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    AlphanumericValue = "verticalewand"
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(parameters);

            // Assert
            Assert.IsTrue(validationResult.IsValid,
                          "Expected to be valid, but found following errors: {0}",
                          string.Join(Environment.NewLine, validationResult.ErrorMessages));
        }

        [Test]
        public void ValidateStabilityPointStructuresParameters_StructureParameterRowsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structureParameterRows", paramName);
        }

        [Test]
        public void ValidateStabilityPointStructuresParameters_ParameterIdsMissingOrDuplicated_ValidIsFalseAndErrorMessages()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = "KW_STERSTAB1",
                NumericalValue = 180.0
            };

            List<StructuresParameterRow> structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                structuresParameterRow
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            List<string> expectedErrorMessages = new List<string>
            {
                "Parameter 'KW_STERSTAB1' komt meermaals voor.",
                "Parameter 'KW_STERSTAB2' ontbreekt.",
                "Parameter 'KW_STERSTAB3' ontbreekt.",
                "Parameter 'KW_STERSTAB4' ontbreekt.",
                "Parameter 'KW_STERSTAB5' ontbreekt.",
                "Parameter 'KW_STERSTAB6' ontbreekt.",
                "Parameter 'KW_STERSTAB7' ontbreekt.",
                "Parameter 'KW_STERSTAB8' ontbreekt.",
                "Parameter 'KW_STERSTAB9' ontbreekt.",
                "Parameter 'KW_STERSTAB10' ontbreekt.",
                "Parameter 'KW_STERSTAB11' ontbreekt.",
                "Parameter 'KW_STERSTAB12' ontbreekt.",
                "Parameter 'KW_STERSTAB13' ontbreekt.",
                "Parameter 'KW_STERSTAB14' ontbreekt.",
                "Parameter 'KW_STERSTAB15' ontbreekt.",
                "Parameter 'KW_STERSTAB16' ontbreekt.",
                "Parameter 'KW_STERSTAB17' ontbreekt.",
                "Parameter 'KW_STERSTAB18' ontbreekt.",
                "Parameter 'KW_STERSTAB19' ontbreekt.",
                "Parameter 'KW_STERSTAB20' ontbreekt.",
                "Parameter 'KW_STERSTAB21' ontbreekt.",
                "Parameter 'KW_STERSTAB22' ontbreekt.",
                "Parameter 'KW_STERSTAB23' ontbreekt.",
                "Parameter 'KW_STERSTAB24' ontbreekt.",
                "Parameter 'KW_STERSTAB25' ontbreekt.",
                "Parameter 'KW_STERSTAB26' ontbreekt."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateStabilityPointStructuresParameters_ParametersAllInvalid_ValidIsFalseAndErrorMessages()
        {
            // Setup
            List<StructuresParameterRow> structureParameterRows = new List<StructuresParameterRow>
            {
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB1",
                    NumericalValue = double.NaN,
                    LineNumber = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB2",
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 2
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB3",
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN,
                    LineNumber = 3
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB4",
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 4
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB5",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 5
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB6",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 6
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB7",
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 7
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB8",
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN,
                    LineNumber = 8
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB9",
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 9
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB10",
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 10
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB11",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 11
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB12",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 12
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB13",
                    NumericalValue = double.NaN,
                    LineNumber = 13
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB14",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 14
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB15",
                    NumericalValue = -1.0,
                    LineNumber = 15
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB16",
                    NumericalValue = double.NegativeInfinity,
                    LineNumber = 16
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB17",
                    NumericalValue = -1.0,
                    LineNumber = 17
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB18",
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 18
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB19",
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 19
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB20",
                    NumericalValue = -1.0,
                    LineNumber = 20
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB21",
                    NumericalValue = -1.0,
                    LineNumber = 21
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB22",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 22
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB23",
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 23
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB24",
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 24
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB25",
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN,
                    LineNumber = 25
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_STERSTAB26",
                    AlphanumericValue = "oei",
                    LineNumber = 26
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            List<string> expectedErrorMessages = new List<string>
            {
                "De waarde op regel 1, kolom 'NumeriekeWaarde' valt buiten het bereik [0, 360].",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 2, kolom numeriekewaarde) niet te dicht op 0 zijn.",
                "De waarde op regel 3, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 3, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 3, kolom 'Standarddeviatie.variance' is ongeldig.",
                "De waarde op regel 5, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 5, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 6, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 6, kolom 'Boolean' is ongeldig.",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 7, kolom numeriekewaarde) niet te dicht op 0 zijn.",
                "De waarde op regel 8, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 8, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 8, kolom 'Standarddeviatie.variance' is ongeldig.",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 9, kolom numeriekewaarde) niet te dicht op 0 zijn.",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 10, kolom numeriekewaarde) niet te dicht op 0 zijn.",
                "De waarde op regel 11, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 11, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 12, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 12, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 13, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "De waarde op regel 14, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 14, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 15, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "De waarde op regel 16, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "De waarde op regel 17, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "De waarde op regel 20, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "De waarde op regel 21, kolom 'NumeriekeWaarde' valt buiten het bereik (0, 1].",
                "De waarde op regel 22, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 22, kolom 'Boolean' is ongeldig.",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 23, kolom numeriekewaarde) niet te dicht op 0 zijn.",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 24, kolom numeriekewaarde) niet te dicht op 0 zijn.",
                "De waarde op regel 25, kolom 'NumeriekeWaarde' is ongeldig.",
                "De waarde op regel 25, kolom 'Boolean' is ongeldig.",
                "De waarde op regel 25, kolom 'Standarddeviatie.variance' is ongeldig.",
                "De waarde op regel 26, kolom 'AlphanumeriekeWaarde' is ongeldig."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateStabilityPointStructuresParameters_VarianceValueConversionWithNegativeMean_NoErrorMessage()
        {
            // Setup
            var parameters = new[]
            {
                // Parameters of interest for test:
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword4,
                    NumericalValue = -1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.StandardDeviation // Expected Coefficient of Variation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword5,
                    NumericalValue = -1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword6,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword11,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword12,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword14,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword18,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.StandardDeviation // Expected Coefficient of Variation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword19,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.StandardDeviation // Expected Coefficient of Variation for normal distribution
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword22,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation // Expected Standard Deviation for normal distribution
                },

                // Remainder:
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword1,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword2,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword3,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword7,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword8,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword9,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword10,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword13,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword15,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword16,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword17,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword20,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword21,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword23,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword24,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword25,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword26,
                    NumericalValue = 1,
                    VarianceValue = 1,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    AlphanumericValue = "lagedrempel"
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(parameters);

            // Assert
            Assert.IsTrue(validationResult.IsValid,
                          "Expected to be valid, but found following errors: {0}",
                          string.Join(Environment.NewLine, validationResult.ErrorMessages));
        }
    }
}