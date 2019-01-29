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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.StabilityStoneCover;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil.Hydraulics;

namespace Riskeer.Storage.Core.Test.Read.StabilityStoneCover
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((StabilityStoneCoverWaveConditionsCalculationEntity) null).Read(new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_ValidEntity_ReturnCalculation()
        {
            // Setup
            const string name = "Calculation Name";
            const string comments = "Calculation Comment";

            var random = new Random(21);
            double orientation = random.NextDouble();
            bool useBreakWater = random.NextBoolean();
            var breakWaterType = random.NextEnumValue<BreakWaterType>();
            double breakWaterHeight = random.NextDouble();
            bool useForeshore = random.NextBoolean();
            const double lowerBoundaryRevetment = 3.58;
            const double upperBoundaryRevetment = 6.10;
            const double lowerBoundaryWaterLevels = 3.40;
            const double upperBoundaryWaterLevels = 5.88;
            var stepSize = random.NextEnumValue<WaveConditionsInputStepSize>();
            var categoryType = random.NextEnumValue<AssessmentSectionCategoryType>();

            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity
            {
                Name = name,
                Comments = comments,
                UseBreakWater = Convert.ToByte(useBreakWater),
                BreakWaterType = Convert.ToByte(breakWaterType),
                BreakWaterHeight = breakWaterHeight,
                UseForeshore = Convert.ToByte(useForeshore),
                Orientation = orientation,
                UpperBoundaryRevetment = upperBoundaryRevetment,
                LowerBoundaryRevetment = lowerBoundaryRevetment,
                UpperBoundaryWaterLevels = upperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = lowerBoundaryWaterLevels,
                StepSize = Convert.ToByte(stepSize),
                CategoryType = Convert.ToByte(categoryType)
            };

            var collector = new ReadConversionCollector();

            // Call
            StabilityStoneCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, calculation.Name);
            Assert.AreEqual(comments, calculation.Comments.Body);

            AssessmentSectionCategoryWaveConditionsInput calculationInput = calculation.InputParameters;
            Assert.AreEqual(useBreakWater, calculationInput.UseBreakWater);
            Assert.AreEqual(breakWaterType, calculationInput.BreakWater.Type);
            AssertRoundedDouble(breakWaterHeight, calculationInput.BreakWater.Height);
            Assert.AreEqual(useForeshore, calculationInput.UseForeshore);
            AssertRoundedDouble(orientation, calculationInput.Orientation);
            AssertRoundedDouble(upperBoundaryRevetment, calculationInput.UpperBoundaryRevetment);
            AssertRoundedDouble(lowerBoundaryRevetment, calculationInput.LowerBoundaryRevetment);
            AssertRoundedDouble(upperBoundaryWaterLevels, calculationInput.UpperBoundaryWaterLevels);
            AssertRoundedDouble(lowerBoundaryWaterLevels, calculationInput.LowerBoundaryWaterLevels);
            Assert.AreEqual(stepSize, calculationInput.StepSize);
            Assert.AreEqual(categoryType, calculationInput.CategoryType);

            Assert.IsNull(calculationInput.HydraulicBoundaryLocation);
            Assert.IsNull(calculationInput.ForeshoreProfile);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnCalculationWithNaNValues()
        {
            // Setup
            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity();
            var collector = new ReadConversionCollector();

            // Call
            StabilityStoneCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNull(calculation.Name);
            Assert.IsNull(calculation.Comments.Body);

            AssessmentSectionCategoryWaveConditionsInput calculationInput = calculation.InputParameters;
            Assert.IsNaN(calculationInput.BreakWater.Height);
            Assert.IsNaN(calculationInput.Orientation);
            Assert.IsNaN(calculationInput.UpperBoundaryRevetment);
            Assert.IsNaN(calculationInput.LowerBoundaryRevetment);
            Assert.IsNaN(calculationInput.UpperBoundaryWaterLevels);
            Assert.IsNaN(calculationInput.LowerBoundaryWaterLevels);

            Assert.IsNull(calculationInput.HydraulicBoundaryLocation);
            Assert.IsNull(calculationInput.ForeshoreProfile);
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Read_EntityWithForeshoreProfileInCollector_CalculationHasAlreadyReadForeshoreProfile()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();
            var foreshoreProfileEntity = new ForeshoreProfileEntity
            {
                GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
            };
            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity
            {
                ForeshoreProfileEntity = foreshoreProfileEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(foreshoreProfileEntity, foreshoreProfile);

            // Call
            StabilityStoneCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(foreshoreProfile, calculation.InputParameters.ForeshoreProfile);
        }

        [Test]
        public void Read_EntityWithForeshoreProfileNotYetInCollector_CalculationWithCreatedForeshoreProfileAndRegisteredNewEntities()
        {
            // Setup
            const string id = "profile";
            var foreshoreProfileEntity = new ForeshoreProfileEntity
            {
                Id = id,
                GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
            };

            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity
            {
                ForeshoreProfileEntity = foreshoreProfileEntity
            };

            var collector = new ReadConversionCollector();

            // Call
            StabilityStoneCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(foreshoreProfileEntity));
            CollectionAssert.AreEqual(id, calculation.InputParameters.ForeshoreProfile.Id);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationInCollector_CalculationHasAlreadyReadHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 1.1, 2.2);
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            StabilityStoneCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithHydraulicBoundaryLocationNotYetInCollector_CalculationWithCreatedHydraulicBoundaryLocationAndRegisteredNewEntities()
        {
            // Setup
            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity
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
            const double outputALevel = 5.4;
            const double outputBLevel = 2.3;
            const double outputCLevel = 13.2;
            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity
            {
                StabilityStoneCoverWaveConditionsOutputEntities =
                {
                    new StabilityStoneCoverWaveConditionsOutputEntity
                    {
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        WaterLevel = outputBLevel,
                        Order = 1,
                        OutputType = Convert.ToByte(WaveConditionsOutputType.Columns)
                    },
                    new StabilityStoneCoverWaveConditionsOutputEntity
                    {
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        WaterLevel = outputCLevel,
                        Order = 2,
                        OutputType = Convert.ToByte(WaveConditionsOutputType.Blocks)
                    },
                    new StabilityStoneCoverWaveConditionsOutputEntity
                    {
                        CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                        WaterLevel = outputALevel,
                        Order = 0,
                        OutputType = Convert.ToByte(WaveConditionsOutputType.Columns)
                    }
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            StabilityStoneCoverWaveConditionsCalculation calculation = entity.Read(collector);

            // Assert
            Assert.IsNotNull(calculation.Output);
            double accuracy = calculation.Output.ColumnsOutput.ElementAt(0).WaterLevel.GetAccuracy();

            Assert.AreEqual(2, calculation.Output.ColumnsOutput.Count());
            Assert.AreEqual(outputALevel, calculation.Output.ColumnsOutput.ElementAt(0).WaterLevel, accuracy);
            Assert.AreEqual(outputBLevel, calculation.Output.ColumnsOutput.ElementAt(1).WaterLevel, accuracy);

            Assert.AreEqual(1, calculation.Output.BlocksOutput.Count());
            Assert.AreEqual(outputCLevel, calculation.Output.BlocksOutput.ElementAt(0).WaterLevel, accuracy);
        }

        private static void AssertRoundedDouble(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }
    }
}