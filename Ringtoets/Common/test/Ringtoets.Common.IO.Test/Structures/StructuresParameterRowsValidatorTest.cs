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
                "Parameter 'KW_HOOGTE2' ontbreekt.",
                "Parameter 'KW_HOOGTE3' ontbreekt.",
                "Parameter 'KW_HOOGTE4' ontbreekt.",
                "Parameter 'KW_HOOGTE5' ontbreekt.",
                "Parameter 'KW_HOOGTE6' ontbreekt.",
                "Parameter 'KW_HOOGTE7' ontbreekt.",
                "Parameter 'KW_HOOGTE8' ontbreekt."
            };
            List<string> expectedWarningMessages = new List<string>
            {
                "Parameter 'KW_HOOGTE1' komt meermaals voor. De eerste specificatie wordt gebruikt."
            };
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
            CollectionAssert.AreEqual(expectedWarningMessages, validationResult.WarningMessages);
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
            List<string> expectedWarningMessages = new List<string>();
            CollectionAssert.AreEqual(expectedErrorMessages, validationResult.ErrorMessages);
            CollectionAssert.AreEqual(expectedWarningMessages, validationResult.WarningMessages);
        }
    }
}