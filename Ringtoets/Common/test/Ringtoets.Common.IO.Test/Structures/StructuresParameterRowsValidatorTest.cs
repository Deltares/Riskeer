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
        public void ValidateHeightStructuresParameters_ParameterIdsMissingOrDuplicated_ExpectedValues()
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
        public void ValidateHeightStructuresParameters_ParametersAllInvalid_ExpectedValues()
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
                "De waarde op regel 1, kolom 18 valt buiten het bereik [0, 360].",
                "De waarde op regel 2, kolom 18 is ongeldig.",
                "De waarde op regel 2, kolom 20 is ongeldig.",
                "De waarde op regel 2, kolom 19 is ongeldig.",
                "De waarde op regel 3, kolom 18 is ongeldig.",
                "De waarde op regel 3, kolom 20 is ongeldig.",
                "De waarde op regel 3, kolom 19 is ongeldig.",
                "De waarde op regel 4, kolom 18 is ongeldig.",
                "De waarde op regel 4, kolom 20 is ongeldig.",
                "De waarde op regel 4, kolom 19 is ongeldig.",
                "De waarde op regel 5, kolom 18 is ongeldig.",
                "De waarde op regel 5, kolom 20 is ongeldig.",
                "De waarde op regel 6, kolom 18 valt buiten het bereik [0, 1].",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 7, kolom numeriekewaarde) niet te dicht op 0 zijn.",
                "De waarde op regel 8, kolom 18 is ongeldig.",
                "Voor een betrouwbare conversie tussen standaard deviatie en variatiecoëfficiënt mag de gemiddelde waarde (op regel 8, kolom numeriekewaarde) niet te dicht op 0 zijn."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
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
        public void ValidateClosingStructuresParameters_ParameterIdsMissingOrDuplicated_ExpectedValues()
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
        public void ValidateClosingStructuresParameters_ParametersAllInvalid_ExpectedValues()
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
                "De waarde op regel 2, kolom 18 is ongeldig.",
                "De waarde op regel 3, kolom 18 valt buiten het bereik [0, 360].",
                "De waarde op regel 4, kolom 18 is ongeldig.",
                "De waarde op regel 5, kolom 18 is ongeldig.",
                "De waarde op regel 5, kolom 20 is ongeldig.",
                "De waarde op regel 6, kolom 18 is ongeldig.",
                "De waarde op regel 6, kolom 20 is ongeldig.",
                "De waarde op regel 7, kolom 18 is ongeldig.",
                "De waarde op regel 7, kolom 20 is ongeldig.",
                "De waarde op regel 8, kolom 18 is ongeldig.",
                "De waarde op regel 8, kolom 20 is ongeldig.",
                "De waarde op regel 8, kolom 19 is ongeldig.",
                "De waarde op regel 9, kolom 18 is ongeldig.",
                "De waarde op regel 9, kolom 20 is ongeldig.",
                "De waarde op regel 9, kolom 19 is ongeldig.",
                "De waarde op regel 10, kolom 18 is ongeldig.",
                "De waarde op regel 10, kolom 20 is ongeldig.",
                "De waarde op regel 10, kolom 19 is ongeldig.",
                "De waarde op regel 11, kolom 18 valt buiten het bereik [0, 1].",
                "De waarde op regel 12, kolom 18 valt buiten het bereik [0, 1].",
                "De waarde op regel 13, kolom 18 mag niet kleiner dan nul zijn.",
                "De waarde op regel 14, kolom 18 valt buiten het bereik [0, 1].",
                "De waarde op regel 15, kolom 17 is ongeldig."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }
    }
}