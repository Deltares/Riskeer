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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.WaveImpactAsphaltCover;
using Application.Ringtoets.Storage.Serializers;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Application.Ringtoets.Storage.Test.Read.WaveImpactAsphaltCover
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationEntityReadExtensionsTest
    {
        private static IEnumerable<TestCaseData> ValidWaveConditionsInputs
        {
            get
            {
                yield return new TestCaseData("N", "C", 1.0, true, BreakWaterType.Caisson, 2.0, true,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Half);
                yield return new TestCaseData("N", "C", 1.0, true, BreakWaterType.Dam, 2.0, true,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.One);
                yield return new TestCaseData("N", "C", 1.0, true, BreakWaterType.Wall, 2.0, true,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Two);

                yield return new TestCaseData("N", "C", 1.0, true, BreakWaterType.Wall, 2.0, false,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Half);
                yield return new TestCaseData("N", "C", 1.0, true, BreakWaterType.Dam, 2.0, false,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.One);
                yield return new TestCaseData("N", "C", 1.0, true, BreakWaterType.Caisson, 2.0, false,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Two);

                yield return new TestCaseData("N", "C", 1.0, false, BreakWaterType.Caisson, 2.0, true,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Half);
                yield return new TestCaseData("N", "C", 1.0, false, BreakWaterType.Caisson, 2.0, true,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.One);
                yield return new TestCaseData("N", "C", 1.0, false, BreakWaterType.Caisson, 2.0, true,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Two);

                yield return new TestCaseData("N", "C", 1.0, false, BreakWaterType.Caisson, 2.0, false,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Half);
                yield return new TestCaseData("N", "C", 1.0, false, BreakWaterType.Caisson, 2.0, false,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.One);
                yield return new TestCaseData("N", "C", 1.0, false, BreakWaterType.Caisson, 2.0, false,
                                              3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Two);

                yield return new TestCaseData(null, null, double.NaN, true, BreakWaterType.Caisson, double.NaN, true,
                                              double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Half);
                yield return new TestCaseData(null, null, double.NaN, true, BreakWaterType.Caisson, double.NaN, true,
                                              double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.One);
                yield return new TestCaseData(null, null, double.NaN, true, BreakWaterType.Caisson, double.NaN, true,
                                              double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Two);

                yield return new TestCaseData(null, null, double.NaN, true, BreakWaterType.Caisson, double.NaN, false,
                                              double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Half);
                yield return new TestCaseData(null, null, double.NaN, true, BreakWaterType.Caisson, double.NaN, false,
                                              double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.One);
                yield return new TestCaseData(null, null, double.NaN, true, BreakWaterType.Caisson, double.NaN, false,
                                              double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Two);

                yield return new TestCaseData(null, null, double.NaN, false, BreakWaterType.Caisson, double.NaN, false,
                                              double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Half);
                yield return new TestCaseData(null, null, double.NaN, false, BreakWaterType.Caisson, double.NaN, false,
                                              double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.One);
                yield return new TestCaseData(null, null, double.NaN, false, BreakWaterType.Caisson, double.NaN, false,
                                              double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Two);
            }
        }

        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        [TestCaseSource("ValidWaveConditionsInputs")]
        public void Read_ValidEntity_ReturnWaveImpactAsphaltCoverWaveConditionsCalculation(
            string name, string comments,
            double orientation, bool useBreakWater, BreakWaterType breakWaterType, double breakWaterHeight, bool useForeshore, double lowerBoundaryRevetment,
            double upperBoundaryRevetment, double lowerBoundaryWaterLevels,
            double upperBoundaryWaterLevels, WaveConditionsInputStepSize stepSize)
        {
            // Setup
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                Name = name,
                Comments = comments,
                UseBreakWater = Convert.ToByte(useBreakWater),
                BreakWaterType = (byte) breakWaterType,
                BreakWaterHeight = breakWaterHeight,
                UseForeshore = Convert.ToByte(useForeshore),
                Orientation = orientation,
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = (byte) stepSize
            };

            var collector = new ReadConversionCollector();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, calculation.Name);
            Assert.AreEqual(comments, calculation.Comments);
            Assert.AreEqual(useBreakWater, calculation.InputParameters.UseBreakWater);
            Assert.AreEqual(breakWaterType, calculation.InputParameters.BreakWater.Type);
            AssertRoundedDouble(breakWaterHeight, calculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(useForeshore, calculation.InputParameters.UseForeshore);
            AssertRoundedDouble(orientation, calculation.InputParameters.Orientation);
            AssertRoundedDouble(upperBoundaryRevetment, calculation.InputParameters.UpperBoundaryRevetment);
            AssertRoundedDouble(lowerBoundaryRevetment, calculation.InputParameters.LowerBoundaryRevetment);
            AssertRoundedDouble(upperBoundaryWaterLevels, calculation.InputParameters.UpperBoundaryWaterLevels);
            AssertRoundedDouble(lowerBoundaryWaterLevels, calculation.InputParameters.LowerBoundaryWaterLevels);
            Assert.AreEqual(stepSize, calculation.InputParameters.StepSize);

            Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(calculation.InputParameters.ForeshoreProfile);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Read_EntityWithForeshoreProfileInCollector_CalculationHasAlreadyReadForeshoreProfile()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var foreshoreProfileEntity = new ForeshoreProfileEntity
            {
                GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
            };
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                ForeshoreProfileEntity = foreshoreProfileEntity,
            };

            var collector = new ReadConversionCollector();
            collector.Read(foreshoreProfileEntity, foreshoreProfile);

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(foreshoreProfile, calculation.InputParameters.ForeshoreProfile);
        }

        [Test]
        public void Read_EntityWithForeshoreProfileNotYetInCollector_CalculationWithCreatedForeshoreProfileAndRegisteredNewEntities()
        {
            // Setup
            string name = "foreshore profile";
            var foreshoreProfileEntity = new ForeshoreProfileEntity
            {
                Name = name,
                GeometryXml = new Point2DXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
            };

            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                ForeshoreProfileEntity = foreshoreProfileEntity,
            };

            var collector = new ReadConversionCollector();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(foreshoreProfileEntity));
            CollectionAssert.AreEqual(name, calculation.InputParameters.ForeshoreProfile.Name);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationInCollector_CalculationHasAlreadyReadHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1.1, 2.2);
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationNotYetInCollector_CalculationWithCreatedHydraulicBoundaryLocationAndRegisteredNewEntities()
        {
            // Setup
            var hydraulicLocationEntity = new HydraulicLocationEntity
            {
                Name = "A"
            };

            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity
            };

            var collector = new ReadConversionCollector();

            // Call
            entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(hydraulicLocationEntity));
        }

        [Test]
        public void Read_EntityWithCalculationOutputEntity_CalculationWithOutput()
        {
            // Setup
            double outputALevel = 5.4;
            double outputBLevel = 2.3;
            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
            {
                WaveImpactAsphaltCoverWaveConditionsOutputEntities =
                {
                    new WaveImpactAsphaltCoverWaveConditionsOutputEntity
                    {
                        WaterLevel = outputBLevel,
                        Order = 1
                    },
                    new WaveImpactAsphaltCoverWaveConditionsOutputEntity
                    {
                        WaterLevel = outputALevel,
                        Order = 0
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.Output);
            Assert.AreEqual(2, calculation.Output.Items.Count());
            var accuracy = calculation.Output.Items.ElementAt(0).WaterLevel.GetAccuracy();
            Assert.AreEqual(outputALevel, calculation.Output.Items.ElementAt(0).WaterLevel, accuracy);
            Assert.AreEqual(outputBLevel, calculation.Output.Items.ElementAt(1).WaterLevel, accuracy);
        }

        private static void AssertRoundedDouble(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }
    }
}