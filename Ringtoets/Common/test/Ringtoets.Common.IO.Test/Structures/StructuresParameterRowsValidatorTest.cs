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
        public void ValidateHeightStructuresParameters_ParameterIdsMissingOrDuplicated_ExpectedValues()
        {
            // Setup
            var structuresParameterRow = new StructuresParameterRow
            {
                ParameterId = "KW_HOOGTE1",
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
                    ParameterId = "KW_HOOGTE1",
                    NumericalValue = double.NaN
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_HOOGTE2",
                    NumericalValue = double.NaN,
                    VarianceValue = -10.0
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_HOOGTE3",
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_HOOGTE4",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = double.PositiveInfinity
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_HOOGTE5",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_HOOGTE6",
                    NumericalValue = double.NegativeInfinity
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_HOOGTE7",
                    NumericalValue = double.NegativeInfinity,
                    VarianceValue = -10.0
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_HOOGTE8",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.StandardDeviation
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateHeightStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            List<string> expectedErrorMessages = new List<string>
            {
                "De oriëntatie van het kunstwerk valt buiten het bereik [0, 360].",
                "De kerende hoogte van het kunstwerk heeft een ongeldige waarde.",
                "De standaard afwijking van de kerende hoogte normaalverdeling heeft een ongeldige waarde.",
                "De stroomvoerende breedte bij bodembescherming van het kunstwerk heeft een ongeldige waarde.",
                "De variantie van de stroomvoerende breedte bij bodembescherming lognormaalverdeling heeft een ongeldige waarde.",
                "Het kritieke overslagdebiet per strekkende meter van het kunstwerk heeft een ongeldige waarde.",
                "De variantie van de kritieke overslagdebiet per strekkende meter lognormaalverdeling heeft een ongeldige waarde.",
                "De breedte van de kruin van het kunstwerk heeft een ongeldige waarde.",
                "De standaard afwijking van de breedte van de kruin normaalverdeling heeft een ongeldige waarde.",
                "De waarde voor de faalkans van het kunstwerk valt buiten het bereik [0, 1].",
                "Het kombergend oppervlak van het kunstwerk heeft een ongeldige waarde.",
                "De variantie van de kombergend oppervlak lognormaalverdeling heeft een ongeldige waarde.",
                "De toegestane peilverhoging op het kombergend oppervlak van het kunstwerk heeft een ongeldige waarde.",
                "De variantie van de toegestane peilverhoging op het kombergend oppervlak lognormaalverdeling heeft een ongeldige waarde."
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
                    NumericalValue = double.NegativeInfinity,
                    VarianceValue = -10.0
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT2",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.StandardDeviation
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT3",
                    NumericalValue = double.NaN
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT4",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT5",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT6",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT7",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = 10.0,
                    VarianceType = VarianceType.NotSpecified
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT8",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = double.PositiveInfinity
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT9",
                    NumericalValue = double.PositiveInfinity,
                    VarianceValue = double.PositiveInfinity
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT10",
                    NumericalValue = double.NaN,
                    VarianceValue = double.NaN
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT11",
                    NumericalValue = double.NegativeInfinity
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT12",
                    NumericalValue = double.NegativeInfinity
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT13",
                    NumericalValue = -11
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT14",
                    NumericalValue = double.NegativeInfinity
                },
                new StructuresParameterRow
                {
                    ParameterId = "KW_BETSLUIT15",
                    NumericalValue = -11
                }
            };

            // Call
            ValidationResult validationResult = StructuresParameterRowsValidator.ValidateClosingStructuresParameters(structureParameterRows);

            // Assert
            Assert.IsFalse(validationResult.IsValid);
            List<string> expectedErrorMessages = new List<string>
            {
                "Het kombergend oppervlak van het kunstwerk heeft een ongeldige waarde.",
                "De variantie van de kombergend oppervlak lognormaalverdeling heeft een ongeldige waarde.",
                "De toegestane peilverhoging op het kombergend oppervlak van het kunstwerk heeft een ongeldige waarde.",
                "De variantie van de toegestane peilverhoging op het kombergend oppervlak lognormaalverdeling heeft een ongeldige waarde.",
                "De oriëntatie van het kunstwerk valt buiten het bereik [0, 360].",
                "De breedte van de kruin van het kunstwerk heeft een ongeldige waarde.",
                "De standaard afwijking van de breedte van de kruin normaalverdeling heeft een ongeldige waarde.",
                "De kruinhoogte niet gesloten kering van het kunstwerk heeft een ongeldige waarde.",
                "De standaard afwijking van de kruinhoogte niet gesloten kering normaalverdeling heeft een ongeldige waarde.",
                "De binnenwaterstand van het kunstwerk heeft een ongeldige waarde.",
                "De standaard afwijking van de binnenwaterstand normaalverdeling heeft een ongeldige waarde.",
                "De drempelhoogte van het kunstwerk heeft een ongeldige waarde.",
                "De standaard afwijking van de drempelhoogte normaalverdeling heeft een ongeldige waarde.",
                "Het doorstroomoppervlak van het kunstwerk heeft een ongeldige waarde.",
                "De variantie van het doorstroomoppervlak lognormaalverdeling heeft een ongeldige waarde.",
                "Het kritieke overslagdebiet per strekkende meter van het kunstwerk heeft een ongeldige waarde.",
                "De variantie van de kritieke overslagdebiet per strekkende meter lognormaalverdeling heeft een ongeldige waarde.",
                "De stroomvoerende breedte bij bodembescherming van het kunstwerk heeft een ongeldige waarde.",
                "De variantie van de stroomvoerende breedte bij bodembescherming lognormaalverdeling heeft een ongeldige waarde.",
                "De kans op open staan bij naderend hoogwater heeft een ongeldige waarde.",
                "De kans op mislukken sluiting van geopend kunstwerk heeft een ongeldige waarde.",
                "Het aantal identieke doorstroomopeningen heeft een ongeldige waarde.",
                "De faalkans herstel van gefaalde situatie heeft een ongeldige waarde.",
                "Het instroommodel heeft een ongeldige waarde."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
        }
    }
}