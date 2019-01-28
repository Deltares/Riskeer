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
using System.Collections.Generic;
using System.Linq;
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
        public void ValidateHeightStructuresParameters_ParameterIdsDuplicated_IsValidFalseAndErrorMessages()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword1,
                NumericalValue = 180.0
            };

            var structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                structuresParameterRow
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            var expectedErrorMessages = new List<string>
            {
                "Parameter 'KW_HOOGTE1' komt meerdere keren voor."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        [TestCaseSource(nameof(HeightStructureParameters))]
        public void ValidateHeightStructuresParameters_OneParameterDefined_IsValidTrueAndMessagesEmpty(StructuresParameterRow structuresParameterRow)
        {
            // Setup
            var structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsTrue(validationResult.IsValid);
            CollectionAssert.IsEmpty(validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateHeightStructuresParameters_NoParameterDefined_IsValidFalseAndErrorMessages()
        {
            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(
                new List<StructuresParameterRow>());

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            var expectedErrorMessages = new List<string>
            {
                "Geen geldige parameter definities gevonden."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ValidateHeightStructuresParameters_ParametersAllInvalid_IsValidFalseAndErrorMessages()
        {
            // Setup
            var structureParameterRows = new List<StructuresParameterRow>
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
                    NumericalValue = 1e-6,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 8
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            var expectedErrorMessages = new List<string>
            {
                "De waarde voor parameter 'KW_HOOGTE1' op regel 1, kolom 'Numeriekewaarde', moet in het bereik [0,0, 360,0] liggen.",
                "De waarde voor parameter 'KW_HOOGTE2' op regel 2, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_HOOGTE2' op regel 2, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_HOOGTE2' op regel 2, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_HOOGTE3' op regel 3, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_HOOGTE3' op regel 3, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_HOOGTE3' op regel 3, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_HOOGTE4' op regel 4, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_HOOGTE4' op regel 4, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_HOOGTE4' op regel 4, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_HOOGTE5' op regel 5, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_HOOGTE5' op regel 5, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_HOOGTE6' op regel 6, kolom 'Numeriekewaarde', moet in het bereik [0,0, 1,0] liggen.",
                "De waarde voor parameter 'KW_HOOGTE7' op regel 7, kolom 'Numeriekewaarde', is te dicht op 0 waardoor een betrouwbare conversie tussen standaardafwijking en variatiecoëfficiënt niet mogelijk is.",
                "De waarde voor parameter 'KW_HOOGTE8' op regel 8, kolom 'Numeriekewaarde', is te dicht op 0 waardoor een betrouwbare conversie tussen standaardafwijking en variatiecoëfficiënt niet mogelijk is."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateHeightStructuresParameters_ParametersAllValid_IsValidTrueAndNoErrorMessage()
        {
            // Setup
            List<StructuresParameterRow> structureParameterRows = GetAllValidHeightStructuresParameterRows();

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsTrue(validationResult.IsValid);
            CollectionAssert.IsEmpty(validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateHeightStructuresParameters_VarianceValueConversionWithNegativeMean_NoErrorMessage()
        {
            // Setup
            var parameters = new[]
            {
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

                #region Remaining valid parameters irrelevant for test:

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

                #endregion
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(parameters);

            // Assert
            Assert.IsTrue(validationResult.IsValid,
                          "Expected to be valid, but found following errors: {0}",
                          string.Join(Environment.NewLine, validationResult.ErrorMessages));
        }

        [Test]
        public void GetRelevantHeightStructuresParameters_StructureParametersRowsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StructuresParameterRowsValidator.GetRelevantHeightStructuresParameters(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structureParameterRows", paramName);
        }

        [Test]
        public void GetRelevantHeightStructuresParameters_ParameterIdDuplicated_ThrowsArgumentException()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword1,
                NumericalValue = 180.0
            };

            var structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                structuresParameterRow
            };

            // Call
            TestDelegate call = () =>
                StructuresParameterRowsValidator.GetRelevantHeightStructuresParameters(structureParameterRows);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("Parameter 'KW_HOOGTE1' komt meerdere keren voor.", exception.Message);
        }

        [Test]
        public void GetRelevantHeightStructuresParameters_NoParametersPresent_ReturnsNoParameters()
        {
            // Setup
            var structuresParameterRows = new List<StructuresParameterRow>();

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantHeightStructuresParameters(structuresParameterRows);

            // Assert
            CollectionAssert.IsEmpty(parameters);
        }

        [Test]
        public void GetRelevantHeightStructuresParameters_OnlyIrrelevantParametersPresent_ReturnsNoParameters()
        {
            // Setup
            var irrelevantParameter = new StructuresParameterRow
            {
                ParameterId = "I am irrelevant :("
            };

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantHeightStructuresParameters(new[]
                {
                    irrelevantParameter
                });

            // Assert
            CollectionAssert.IsEmpty(parameters);
        }

        [Test]
        public void GetRelevantHeightStructuresParameters_AllParametersPresent_ReturnsAllParameters()
        {
            // Setup
            List<StructuresParameterRow> structureParameterRows = GetAllValidHeightStructuresParameterRows();

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantHeightStructuresParameters(structureParameterRows);

            // Assert
            CollectionAssert.AreEqual(structureParameterRows, parameters);
        }

        [Test]
        public void GetRelevantHeightStructuresParameters_AllParametersPresentAndExtraParameters_ReturnsOnlyRelevantParameters()
        {
            // Setup
            IEnumerable<StructuresParameterRow> expectedParameterRows = GetAllValidHeightStructuresParameterRows();

            const string extraParameterName = "Extra Parameter";
            var extraParameter = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };
            var extraParameter2 = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };

            List<StructuresParameterRow> readParameterRows = expectedParameterRows.ToList();
            readParameterRows.Add(extraParameter);
            readParameterRows.Add(extraParameter2);

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantHeightStructuresParameters(readParameterRows);

            // Assert
            CollectionAssert.AreEqual(expectedParameterRows, parameters);
        }

        [Test]
        [TestCaseSource(nameof(HeightStructureParameters))]
        public void GetRelevantHeightStructuresParameters_SingleParameterPresent_ReturnsParameters(StructuresParameterRow structuresParameterRow)
        {
            // Setup
            StructuresParameterRow[] expectedParameter =
            {
                structuresParameterRow
            };

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantHeightStructuresParameters(new[]
                {
                    structuresParameterRow
                });

            // Assert
            CollectionAssert.AreEqual(expectedParameter, parameters);
        }

        [Test]
        [TestCaseSource(nameof(HeightStructureParameters))]
        public void GetRelevantHeightStructuresParameters_SingleParameterPresentAndExtraParameters_ReturnsOnlyRelevantParameters(StructuresParameterRow structuresParameterRow)
        {
            // Setup
            StructuresParameterRow[] expectedParameterRows =
            {
                structuresParameterRow
            };

            const string extraParameterName = "Extra Parameter";
            var extraParameter = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };
            var extraParameter2 = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };
            var readParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                extraParameter,
                extraParameter2
            };

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantHeightStructuresParameters(readParameterRows);

            // Assert
            CollectionAssert.AreEqual(expectedParameterRows, parameters);
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
        public void ValidateClosingStructuresParameters_ParameterIdsDuplicated_IsValidFalseAndErrorMessages()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword3,
                NumericalValue = 180.0
            };

            var structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                structuresParameterRow
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            var expectedErrorMessages = new List<string>
            {
                "Parameter 'KW_BETSLUIT3' komt meerdere keren voor."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        [TestCaseSource(nameof(ClosingStructureParameters))]
        public void ValidateClosingStructuresParameters_OneParameterDefined_IsValidTrueMessagesEmpty(StructuresParameterRow structuresParameterRow)
        {
            // Setup
            var structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);

            // Assert;
            Assert.IsTrue(validationResult.IsValid);
            CollectionAssert.IsEmpty(validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateClosingStructuresParameters_NoParameterDefined_IsValidFalseAndErrorMessages()
        {
            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(
                new List<StructuresParameterRow>());

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            var expectedErrorMessages = new List<string>
            {
                "Geen geldige parameter definities gevonden."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ValidateClosingStructuresParameters_ParametersAllInvalid_IsValidFalseAndErrorMessages()
        {
            // Setup
            var structureParameterRows = new List<StructuresParameterRow>
            {
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword1,
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword2,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 2
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword3,
                    NumericalValue = double.NaN,
                    LineNumber = 3
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword4,
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 4
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword5,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 5
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword6,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 6
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword7,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 7
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword8,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = double.PositiveInfinity,
                    LineNumber = 8
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword9,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = double.PositiveInfinity,
                    LineNumber = 9
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword10,
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN,
                    LineNumber = 10
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword11,
                    NumericalValue = double.NegativeInfinity,
                    LineNumber = 11
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword12,
                    NumericalValue = double.NegativeInfinity,
                    LineNumber = 12
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword13,
                    NumericalValue = 0,
                    LineNumber = 13
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword14,
                    NumericalValue = double.NegativeInfinity,
                    LineNumber = 14
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword15,
                    AlphanumericValue = "oei",
                    LineNumber = 15
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            var expectedErrorMessages = new List<string>
            {
                "De waarde voor parameter 'KW_BETSLUIT1' op regel 1, kolom 'Numeriekewaarde', is te dicht op 0 waardoor een betrouwbare conversie tussen standaardafwijking en variatiecoëfficiënt niet mogelijk is.",
                "De waarde voor parameter 'KW_BETSLUIT2' op regel 2, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_BETSLUIT3' op regel 3, kolom 'Numeriekewaarde', moet in het bereik [0,0, 360,0] liggen.",
                "De waarde voor parameter 'KW_BETSLUIT5' op regel 5, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_BETSLUIT5' op regel 5, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_BETSLUIT6' op regel 6, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_BETSLUIT6' op regel 6, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_BETSLUIT7' op regel 7, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_BETSLUIT7' op regel 7, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_BETSLUIT8' op regel 8, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_BETSLUIT8' op regel 8, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_BETSLUIT8' op regel 8, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_BETSLUIT9' op regel 9, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_BETSLUIT9' op regel 9, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_BETSLUIT9' op regel 9, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_BETSLUIT10' op regel 10, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_BETSLUIT10' op regel 10, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_BETSLUIT10' op regel 10, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_BETSLUIT11' op regel 11, kolom 'Numeriekewaarde', moet in het bereik [0,0, 1,0] liggen.",
                "De waarde voor parameter 'KW_BETSLUIT12' op regel 12, kolom 'Numeriekewaarde', moet in het bereik [0,0, 1,0] liggen.",
                "De waarde voor parameter 'KW_BETSLUIT13' op regel 13, kolom 'Numeriekewaarde', moet een geheel getal zijn groter of gelijk aan 1.",
                "De waarde voor parameter 'KW_BETSLUIT14' op regel 14, kolom 'Numeriekewaarde', moet in het bereik [0,0, 1,0] liggen.",
                "De waarde voor parameter 'KW_BETSLUIT15' op regel 15, kolom 'Alfanumeriekewaarde', moet een geldig kunstwerk type zijn."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateClosingStructuresParameters_ParametersAllValid_IsValidTrueAndNoErrorMessages()
        {
            // Setup
            List<StructuresParameterRow> structureParameterRows = GetAllValidClosingStructuresParameterRows();

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsTrue(validationResult.IsValid);
            CollectionAssert.IsEmpty(validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateClosingStructuresParameters_VarianceValueConversionWithNegativeMean_NoErrorMessage()
        {
            // Setup
            var parameters = new[]
            {
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

                #region Remaining valid parameters irrelevant to test:

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

                #endregion
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(parameters);

            // Assert
            Assert.IsTrue(validationResult.IsValid,
                          "Expected to be valid, but found following errors: {0}",
                          string.Join(Environment.NewLine, validationResult.ErrorMessages));
        }

        [Test]
        public void GetRelevantClosingStructuresParameters_StructureParametersRowsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StructuresParameterRowsValidator.GetRelevantClosingStructuresParameters(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structureParameterRows", paramName);
        }

        [Test]
        public void GetRelevantClosingStructuresParameters_ParameterIdDuplicated_ThrowsArgumentException()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword1,
                NumericalValue = 180.0
            };

            var structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                structuresParameterRow
            };

            // Call
            TestDelegate call = () =>
                StructuresParameterRowsValidator.GetRelevantClosingStructuresParameters(structureParameterRows);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("Parameter 'KW_BETSLUIT1' komt meerdere keren voor.", exception.Message);
        }

        [Test]
        public void GetRelevantClosingStructuresParameters_NoParametersPresent_ReturnsNoParameters()
        {
            // Setup
            var structuresParameterRows = new List<StructuresParameterRow>();

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantClosingStructuresParameters(structuresParameterRows);

            // Assert
            CollectionAssert.IsEmpty(parameters);
        }

        [Test]
        public void GetRelevantClosingStructuresParameters_OnlyIrrelevantParametersPresent_ReturnsNoParameters()
        {
            // Setup
            var irrelevantParameter = new StructuresParameterRow
            {
                ParameterId = "I am irrelevant :("
            };

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantClosingStructuresParameters(new[]
                {
                    irrelevantParameter
                });

            // Assert
            CollectionAssert.IsEmpty(parameters);
        }

        [Test]
        public void GetRelevantClosingStructuresParameters_AllParametersPresent_ReturnsAllParameters()
        {
            // Setup
            List<StructuresParameterRow> structureParameterRows = GetAllValidClosingStructuresParameterRows();

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantClosingStructuresParameters(structureParameterRows);

            // Assert
            CollectionAssert.AreEqual(structureParameterRows, parameters);
        }

        [Test]
        public void GetRelevantClosingStructuresParameters_AllParametersPresentAndExtraParameters_ReturnsOnlyRelevantParameters()
        {
            // Setup
            IEnumerable<StructuresParameterRow> expectedParameterRows = GetAllValidClosingStructuresParameterRows();

            const string extraParameterName = "Extra Parameter";
            var extraParameter = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };
            var extraParameter2 = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };

            List<StructuresParameterRow> readParameterRows = expectedParameterRows.ToList();
            readParameterRows.Add(extraParameter);
            readParameterRows.Add(extraParameter2);

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantClosingStructuresParameters(readParameterRows);

            // Assert
            CollectionAssert.AreEqual(expectedParameterRows, parameters);
        }

        [Test]
        [TestCaseSource(nameof(ClosingStructureParameters))]
        public void GetRelevantClosingStructuresParameters_SingleParameterPresent_ReturnsParameters(StructuresParameterRow structuresParameterRow)
        {
            // Setup
            StructuresParameterRow[] expectedParameter =
            {
                structuresParameterRow
            };

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantClosingStructuresParameters(new[]
                {
                    structuresParameterRow
                });

            // Assert
            CollectionAssert.AreEqual(expectedParameter, parameters);
        }

        [Test]
        [TestCaseSource(nameof(ClosingStructureParameters))]
        public void GetRelevantClosingStructuresParameters_SingleParameterPresentAndExtraParameters_ReturnsOnlyRelevantParameters(StructuresParameterRow structuresParameterRow)
        {
            // Setup
            StructuresParameterRow[] expectedParameterRows =
            {
                structuresParameterRow
            };

            const string extraParameterName = "Extra Parameter";
            var extraParameter = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };
            var extraParameter2 = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };

            var readParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                extraParameter,
                extraParameter2
            };

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantClosingStructuresParameters(readParameterRows);

            // Assert
            CollectionAssert.AreEqual(expectedParameterRows, parameters);
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
        public void ValidateStabilityPointStructuresParameters_ParameterIdsDuplicated_IsValidFalseAndErrorMessages()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword1,
                NumericalValue = 180.0
            };

            var structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                structuresParameterRow
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            var expectedErrorMessages = new List<string>
            {
                "Parameter 'KW_STERSTAB1' komt meerdere keren voor."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        [TestCaseSource(nameof(StabilityPointStructureParameters))]
        public void ValidateStabilityPointStructuresParameters_OneParameterDefined_IsValidTrueAndMessagesEmpty(StructuresParameterRow structuresParameterRow)
        {
            // Setup
            var structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsTrue(validationResult.IsValid);
            CollectionAssert.IsEmpty(validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateStabilityPointStructuresParameters_NoParameterDefined_IsValidFalseAndErrorMessages()
        {
            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(
                new List<StructuresParameterRow>());

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            var expectedErrorMessages = new List<string>
            {
                "Geen geldige parameter definities gevonden."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ValidateStabilityPointStructuresParameters_ParametersAllInvalid_IsValidFalseAndErrorMessages()
        {
            // Setup
            var structureParameterRows = new List<StructuresParameterRow>
            {
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword1,
                    NumericalValue = double.NaN,
                    LineNumber = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword2,
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 2
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword3,
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN,
                    LineNumber = 3
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword4,
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 4
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword5,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 5
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword6,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 6
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword7,
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 7
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword8,
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN,
                    LineNumber = 8
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword9,
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 9
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword10,
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 10
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword11,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 11
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword12,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 12
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword13,
                    NumericalValue = double.NaN,
                    LineNumber = 13
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword14,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 14
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword15,
                    NumericalValue = -1.0,
                    LineNumber = 15
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword16,
                    NumericalValue = double.NegativeInfinity,
                    LineNumber = 16
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword17,
                    NumericalValue = 0.0,
                    LineNumber = 17
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword18,
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 18
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword19,
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation,
                    LineNumber = 19
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword20,
                    NumericalValue = -1,
                    LineNumber = 20
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword21,
                    NumericalValue = -1.0,
                    LineNumber = 21
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword22,
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified,
                    LineNumber = 22
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword23,
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 23
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword24,
                    NumericalValue = 1e-5,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation,
                    LineNumber = 24
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword25,
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN,
                    LineNumber = 25
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword26,
                    AlphanumericValue = "oei",
                    LineNumber = 26
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            var expectedErrorMessages = new List<string>
            {
                "De waarde voor parameter 'KW_STERSTAB1' op regel 1, kolom 'Numeriekewaarde', moet in het bereik [0,0, 360,0] liggen.",
                "De waarde voor parameter 'KW_STERSTAB2' op regel 2, kolom 'Numeriekewaarde', is te dicht op 0 waardoor een betrouwbare conversie tussen standaardafwijking en variatiecoëfficiënt niet mogelijk is.",
                "De waarde voor parameter 'KW_STERSTAB3' op regel 3, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_STERSTAB3' op regel 3, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_STERSTAB3' op regel 3, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_STERSTAB5' op regel 5, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_STERSTAB5' op regel 5, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_STERSTAB6' op regel 6, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_STERSTAB6' op regel 6, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_STERSTAB7' op regel 7, kolom 'Numeriekewaarde', is te dicht op 0 waardoor een betrouwbare conversie tussen standaardafwijking en variatiecoëfficiënt niet mogelijk is.",
                "De waarde voor parameter 'KW_STERSTAB8' op regel 8, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_STERSTAB8' op regel 8, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_STERSTAB8' op regel 8, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_STERSTAB9' op regel 9, kolom 'Numeriekewaarde', is te dicht op 0 waardoor een betrouwbare conversie tussen standaardafwijking en variatiecoëfficiënt niet mogelijk is.",
                "De waarde voor parameter 'KW_STERSTAB10' op regel 10, kolom 'Numeriekewaarde', is te dicht op 0 waardoor een betrouwbare conversie tussen standaardafwijking en variatiecoëfficiënt niet mogelijk is.",
                "De waarde voor parameter 'KW_STERSTAB11' op regel 11, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_STERSTAB11' op regel 11, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_STERSTAB12' op regel 12, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_STERSTAB12' op regel 12, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_STERSTAB13' op regel 13, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_STERSTAB14' op regel 14, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_STERSTAB14' op regel 14, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_STERSTAB15' op regel 15, kolom 'Numeriekewaarde', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_STERSTAB16' op regel 16, kolom 'Numeriekewaarde', moet in het bereik [0,0, 1,0] liggen.",
                "De waarde voor parameter 'KW_STERSTAB17' op regel 17, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_STERSTAB17' op regel 17, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_STERSTAB17' op regel 17, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_STERSTAB20' op regel 20, kolom 'Numeriekewaarde', moet een positief geheel getal zijn.",
                "De waarde voor parameter 'KW_STERSTAB21' op regel 21, kolom 'Numeriekewaarde', moet in het bereik [0,0, 1,0] liggen.",
                "De waarde voor parameter 'KW_STERSTAB22' op regel 22, kolom 'Numeriekewaarde', is geen getal.",
                "De waarde voor parameter 'KW_STERSTAB23' op regel 23, kolom 'Numeriekewaarde', is te dicht op 0 waardoor een betrouwbare conversie tussen standaardafwijking en variatiecoëfficiënt niet mogelijk is.",
                "De waarde voor parameter 'KW_STERSTAB24' op regel 24, kolom 'Numeriekewaarde', is te dicht op 0 waardoor een betrouwbare conversie tussen standaardafwijking en variatiecoëfficiënt niet mogelijk is.",
                "De waarde voor parameter 'KW_STERSTAB25' op regel 25, kolom 'Numeriekewaarde', moet een getal zijn groter dan 0.",
                "De waarde voor parameter 'KW_STERSTAB25' op regel 25, kolom 'Boolean', moet '0' (variatiecoëfficiënt) of '1' (standaardafwijking) zijn.",
                "De waarde voor parameter 'KW_STERSTAB25' op regel 25, kolom 'Standaardafwijking.variatie', moet een positief getal zijn.",
                "De waarde voor parameter 'KW_STERSTAB26' op regel 26, kolom 'Alfanumeriekewaarde', moet een geldig kunstwerk type zijn."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }

        [Test]
        public void ValidateStabilityPointStructuresParameters_ParametersAllValid_IsValidTrueAndNoErrorMessages()
        {
            // Setup
            List<StructuresParameterRow> structureParameterRows = GetAllValidStabilityPointStructuresParameterRows();

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsTrue(validationResult.IsValid);
            CollectionAssert.IsEmpty(validationResult.ErrorMessages);
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
                    VarianceType = VarianceType.CoefficientOfVariation // Type is irrelevant
                },

                #region Remaining valid parameters irrelevant to test:

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

                #endregion
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateStabilityPointStructuresParameters(parameters);

            // Assert
            Assert.IsTrue(validationResult.IsValid,
                          "Expected to be valid, but found following errors: {0}",
                          string.Join(Environment.NewLine, validationResult.ErrorMessages));
        }

        [Test]
        public void GetRelevantStabilityPointStructuresParameters_StructureParametersRowsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StructuresParameterRowsValidator.GetRelevantStabilityPointStructuresParameters(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("structureParameterRows", paramName);
        }

        [Test]
        public void GetRelevantStabilityPointStructuresParameters_ParameterIdDuplicated_ThrowsArgumentException()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword1,
                NumericalValue = 180.0
            };

            var structureParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                structuresParameterRow
            };

            // Call
            TestDelegate call = () =>
                StructuresParameterRowsValidator.GetRelevantStabilityPointStructuresParameters(structureParameterRows);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("Parameter 'KW_STERSTAB1' komt meerdere keren voor.", exception.Message);
        }

        [Test]
        public void GetRelevantStabilityPointStructuresParameters_NoParametersPresent_ReturnsNoParameters()
        {
            // Setup
            var structuresParameterRows = new List<StructuresParameterRow>();

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantStabilityPointStructuresParameters(structuresParameterRows);

            // Assert
            CollectionAssert.IsEmpty(parameters);
        }

        [Test]
        public void GetRelevantStabilityPointStructuresParameters_OnlyIrrelevantParametersPresent_ReturnsNoParameters()
        {
            // Setup
            var irrelevantParameter = new StructuresParameterRow
            {
                ParameterId = "I am irrelevant :("
            };

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantStabilityPointStructuresParameters(new[]
                {
                    irrelevantParameter
                });

            // Assert
            CollectionAssert.IsEmpty(parameters);
        }

        [Test]
        public void GetRelevantStabilityPointStructuresParameters_AllParametersPresent_ReturnsAllParameters()
        {
            // Setup
            List<StructuresParameterRow> structureParameterRows = GetAllValidStabilityPointStructuresParameterRows();

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantStabilityPointStructuresParameters(structureParameterRows);

            // Assert
            CollectionAssert.AreEqual(structureParameterRows, parameters);
        }

        [Test]
        public void GetRelevantStabilityPointStructuresParameters_AllParametersPresentAndExtraParameters_ReturnsOnlyRelevantParameters()
        {
            // Setup
            IEnumerable<StructuresParameterRow> expectedParameterRows = GetAllValidStabilityPointStructuresParameterRows();

            const string extraParameterName = "Extra Parameter";
            var extraParameter = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };
            var extraParameter2 = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };

            List<StructuresParameterRow> readParameterRows = expectedParameterRows.ToList();
            readParameterRows.Add(extraParameter);
            readParameterRows.Add(extraParameter2);

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantStabilityPointStructuresParameters(readParameterRows);

            // Assert
            CollectionAssert.AreEqual(expectedParameterRows, parameters);
        }

        [Test]
        [TestCaseSource(nameof(StabilityPointStructureParameters))]
        public void GetRelevantStabilityPointStructuresParameters_SingleParameterPresent_ReturnsParameters(StructuresParameterRow structuresParameterRow)
        {
            // Setup
            StructuresParameterRow[] expectedParameter =
            {
                structuresParameterRow
            };

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantStabilityPointStructuresParameters(new[]
                {
                    structuresParameterRow
                });

            // Assert
            CollectionAssert.AreEqual(expectedParameter, parameters);
        }

        [Test]
        [TestCaseSource(nameof(StabilityPointStructureParameters))]
        public void GetRelevantStabilityPointStructuresParameters_SingleParameterPresentAndExtraParameters_ReturnsOnlyRelevantParameters(StructuresParameterRow structuresParameterRow)
        {
            // Setup
            StructuresParameterRow[] expectedParameterRows =
            {
                structuresParameterRow
            };

            const string extraParameterName = "Extra Parameter";
            var extraParameter = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };
            var extraParameter2 = new StructuresParameterRow
            {
                ParameterId = extraParameterName
            };

            var readParameterRows = new List<StructuresParameterRow>
            {
                structuresParameterRow,
                extraParameter,
                extraParameter2
            };

            // Call
            IEnumerable<StructuresParameterRow> parameters =
                StructuresParameterRowsValidator.GetRelevantStabilityPointStructuresParameters(readParameterRows);

            // Assert
            CollectionAssert.AreEqual(expectedParameterRows, parameters);
        }

        #region Height structures

        private static List<StructuresParameterRow> GetAllValidHeightStructuresParameterRows()
        {
            return new List<StructuresParameterRow>
            {
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword1,
                    NumericalValue = 180.0
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword2,
                    NumericalValue = 5.9,
                    VarianceValue = 0.01,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword3,
                    NumericalValue = 18.5,
                    VarianceValue = 0.05,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword4,
                    NumericalValue = 0.1,
                    VarianceValue = 0.15,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword5,
                    NumericalValue = 4,
                    VarianceValue = 0.05,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword6,
                    NumericalValue = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword7,
                    NumericalValue = 500000,
                    VarianceValue = 0.1,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.HeightStructureParameterKeyword8,
                    NumericalValue = 6.5,
                    VarianceValue = 0.1,
                    VarianceType = VarianceType.StandardDeviation
                }
            };
        }

        private static IEnumerable<TestCaseData> HeightStructureParameters()
        {
            foreach (StructuresParameterRow parameterRow in GetAllValidHeightStructuresParameterRows())
            {
                yield return new TestCaseData(parameterRow).SetName(parameterRow.ParameterId);
            }
        }

        #endregion

        #region Closing structures

        private static List<StructuresParameterRow> GetAllValidClosingStructuresParameterRows()
        {
            return new List<StructuresParameterRow>
            {
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword1,
                    NumericalValue = 3,
                    VarianceValue = 1.0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword2,
                    NumericalValue = 1,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword3,
                    NumericalValue = 123
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword4,
                    NumericalValue = 0,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword5,
                    NumericalValue = 1,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword6,
                    NumericalValue = 9,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword7,
                    NumericalValue = 1,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword8,
                    NumericalValue = 1,
                    VarianceValue = 2,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword9,
                    NumericalValue = 2,
                    VarianceValue = 3,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword10,
                    NumericalValue = 9,
                    VarianceValue = 3,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword11,
                    NumericalValue = 0.4
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword12,
                    NumericalValue = 0.6
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword13,
                    NumericalValue = 14
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword14,
                    NumericalValue = 0.8
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.ClosingStructureParameterKeyword15,
                    AlphanumericValue = "verticalewand"
                }
            };
        }

        private static IEnumerable<TestCaseData> ClosingStructureParameters()
        {
            foreach (StructuresParameterRow parameterRow in GetAllValidClosingStructuresParameterRows())
            {
                yield return new TestCaseData(parameterRow).SetName(parameterRow.ParameterId);
            }
        }

        #endregion

        #region Stability point structures

        private static List<StructuresParameterRow> GetAllValidStabilityPointStructuresParameterRows()
        {
            return new List<StructuresParameterRow>
            {
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword1,
                    NumericalValue = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword2,
                    NumericalValue = 1,
                    VarianceValue = 0.01,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword3,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword4,
                    NumericalValue = 1,
                    VarianceValue = 0.01,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword5,
                    NumericalValue = 1,
                    VarianceValue = 0.01,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword6,
                    NumericalValue = 1,
                    VarianceValue = 0.01,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword7,
                    NumericalValue = 1,
                    VarianceValue = 0.01,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword8,
                    NumericalValue = 1,
                    VarianceValue = 0.01,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword9,
                    NumericalValue = 1,
                    VarianceValue = 0.01,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword10,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword11,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword12,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword13,
                    NumericalValue = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword14,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword15,
                    NumericalValue = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword16,
                    NumericalValue = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword17,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword18,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword19,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword20,
                    NumericalValue = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword21,
                    NumericalValue = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword22,
                    NumericalValue = 1
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword23,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword24,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.CoefficientOfVariation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword25,
                    NumericalValue = 1,
                    VarianceValue = 0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = StructureFilesKeywords.StabilityPointStructureParameterKeyword26,
                    AlphanumericValue = "LageDrempel"
                }
            };
        }

        private static IEnumerable<TestCaseData> StabilityPointStructureParameters()
        {
            foreach (StructuresParameterRow parameterRow in GetAllValidStabilityPointStructuresParameterRows())
            {
                yield return new TestCaseData(parameterRow).SetName(parameterRow.ParameterId);
            }
        }

        #endregion
    }
}