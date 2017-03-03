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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.WaveImpactAsphaltCover;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.TestUtil;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Application.Ringtoets.Storage.Test.Create.WaveImpactAsphaltCover
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationCreateExtensionsTest
    {
        private static IEnumerable<TestCaseData> ValidWaveConditionsInputs
        {
            get
            {
                yield return new TestCaseData(1.0, true, 3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Half).SetName("ValuesSetBWTrueStepSizeHalf");
                yield return new TestCaseData(1.0, true, 3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.One).SetName("ValuesSetBWTrueStepSizeOne");
                yield return new TestCaseData(1.0, true, 3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Two).SetName("ValuesSetBWTrueStepSizeTwo");

                yield return new TestCaseData(1.0, false, 3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Half).SetName("ValuesSetBWFalseStepSizeHalf");
                yield return new TestCaseData(1.0, false, 3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.One).SetName("ValuesSetBWFalseStepSizeOne");
                yield return new TestCaseData(1.0, false, 3.58, 6.10, 3.40, 5.88, WaveConditionsInputStepSize.Two).SetName("ValuesSetBWFalseStepSizeTwo");

                yield return new TestCaseData(double.NaN, true, double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Half).SetName("NaNValuesBWTrueStepSizeHalf");
                yield return new TestCaseData(double.NaN, true, double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.One).SetName("NaNValuesBWTrueStepSizeOne");
                yield return new TestCaseData(double.NaN, true, double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Two).SetName("NaNValuesBWTrueStepSizeTwo");

                yield return new TestCaseData(double.NaN, false, double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Half).SetName("NaNValuesBWFalseStepSizeHalf");
                yield return new TestCaseData(double.NaN, false, double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.One).SetName("NaNValuesBWFalseStepSizeOne");
                yield return new TestCaseData(double.NaN, false, double.NaN, double.NaN, double.NaN, double.NaN, WaveConditionsInputStepSize.Two).SetName("NaNValuesBWFalseStepSizeTwo");
            }
        }

        [Test]
        public void Create_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            // Call
            TestDelegate call = () => calculation.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual(paramName, "registry");
        }

        [Test]
        [TestCaseSource("ValidWaveConditionsInputs")]
        public void Create_CalculationWithPropertiesSet_ReturnCalculationEntity(
            double orientation, bool useBreakWater, double lowerBoundaryRevetment,
            double upperBoundaryRevetment, double lowerBoundaryWaterLevels,
            double upperBoundaryWaterLevels, WaveConditionsInputStepSize stepSize)
        {
            // Setup
            const string name = "Name";
            const string comments = "comments";
            const int order = 1234;

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = name,
                Comments =
                {
                    Body = comments
                },
                InputParameters =
                {
                    Orientation = (RoundedDouble) orientation,
                    UseBreakWater = useBreakWater,
                    UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                    LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                    UpperBoundaryWaterLevels = (RoundedDouble) upperBoundaryWaterLevels,
                    LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels,
                    StepSize = stepSize
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, order);

            // Assert
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(comments, entity.Comments);

            WaveConditionsInput input = calculation.InputParameters;
            Assert.AreEqual(input.Orientation.Value, entity.Orientation);
            Assert.AreEqual(Convert.ToByte(useBreakWater), entity.UseBreakWater);
            Assert.AreEqual(Convert.ToByte(false), entity.UseForeshore);
            Assert.AreEqual(input.UpperBoundaryRevetment, entity.UpperBoundaryRevetment, input.UpperBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(input.LowerBoundaryRevetment, entity.LowerBoundaryRevetment, input.LowerBoundaryRevetment.GetAccuracy());
            Assert.AreEqual(input.UpperBoundaryWaterLevels, entity.UpperBoundaryWaterLevels, input.UpperBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(input.LowerBoundaryWaterLevels, entity.LowerBoundaryWaterLevels, input.LowerBoundaryWaterLevels.GetAccuracy());
            Assert.AreEqual(Convert.ToByte(input.StepSize), entity.StepSize);

            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(0, entity.WaveImpactAsphaltCoverWaveConditionsCalculationEntityId);
            Assert.IsNull(entity.CalculationGroupEntity);
            Assert.IsNull(entity.ForeshoreProfileEntity);
            Assert.IsNull(entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "A";
            const string comments = "B";
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation()
            {
                Name = name,
                Comments =
                {
                    Body = comments
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreNotSame(name, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(name, entity.Name);

            Assert.AreNotSame(comments, entity.Comments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(comments, entity.Comments);
        }

        [Test]
        public void Create_WaveImpactAsphaltCoverHydraulicLocationEntity_EntityHasWaveImpactAsphaltCoverHydraulicLocationEntity()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2.3, 4.5);

            var registry = new PersistenceRegistry();
            HydraulicLocationEntity hydraulicLocationEntity = hydraulicBoundaryLocation.Create(registry, 0);

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreSame(hydraulicLocationEntity, entity.HydraulicLocationEntity);
        }

        [Test]
        public void Create_HasForeshoreProfile_EntityHasForeshoreProfileEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity.ForeshoreProfileEntity);
        }

        [Test]
        public void Create_HasCalculationOutputs_EntityHasOrderedCalculationOutputEntity()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(new[]
                {
                    new TestWaveConditionsOutput(),
                    new TestWaveConditionsOutput()
                })
            };

            // Call
            WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.AreEqual(2, entity.WaveImpactAsphaltCoverWaveConditionsOutputEntities.Count);
            Assert.AreEqual(new[]
            {
                0,
                1
            }, entity.WaveImpactAsphaltCoverWaveConditionsOutputEntities.Select(oe => oe.Order));
        }
    }
}