﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Util.TestUtil;
using Riskeer.Integration.IO.Factories;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class HydraulicBoundaryLocationMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateHydraulicBoundaryLocationCalculationFeature_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationCalculationFeature(
                null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationCalculationFeature_MetaDataHeaderNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationCalculationFeature(
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("metaDataHeader", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateHydraulicBoundaryLocationCalculationFeature_WithData_ReturnFeature(bool calculationHasOutput)
        {
            // Setup
            const string metaDataHeader = "header";
            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation("location 1"));

            if (calculationHasOutput)
            {
                calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            }

            // Call
            MapFeature feature = HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationCalculationFeature(
                calculation, metaDataHeader);

            // Assert
            RoundedDouble expectedMetaDataValue = calculationHasOutput
                                                      ? calculation.Output.Result
                                                      : RoundedDouble.NaN;
            MapFeaturesMetaDataTestHelper.AssertMetaData(expectedMetaDataValue.ToString(),
                                                         feature, metaDataHeader);
        }
    }
}