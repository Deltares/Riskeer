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

using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationContextPropertiesTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionInwardsCalculationContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsCalculationContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewCalculationContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>();
            mockRepository.ReplayAll();

            var properties = new GrassCoverErosionInwardsCalculationContextProperties();

            // Call
            var grassCoverErosionInwardsCalculationContext = new GrassCoverErosionInwardsCalculationContext(calculationMock, failureMechanismMock, assessmentSectionMock);
            properties.Data = grassCoverErosionInwardsCalculationContext;

            // Assert
            var dikeGeometryProperties = new DikeGeometryProperties
            {
                Data = grassCoverErosionInwardsCalculationContext
            };
            Assert.AreEqual(dikeGeometryProperties.Coordinates, properties.DikeGeometry.Coordinates);
            Assert.AreEqual(dikeGeometryProperties.Roughness, properties.DikeGeometry.Roughness);

            var dikeHeight = new RoundedDouble(2, calculationMock.InputParameters.DikeHeight).Value.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual(dikeHeight, properties.DikeHeight);

            var foreshoreProperties = new ForeshoreProperties
            {
                Data = grassCoverErosionInwardsCalculationContext
            };
            Assert.AreEqual(foreshoreProperties.ForeshorePresent, properties.Foreshore.ForeshorePresent);
            Assert.AreEqual(foreshoreProperties.NumberOfCoordinates, properties.Foreshore.NumberOfCoordinates);

            var orientation = new RoundedDouble(2, calculationMock.InputParameters.Orientation).Value.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual(orientation, properties.Orientation);

            var breakWaterProperties = new BreakWaterProperties
            {
                Data = grassCoverErosionInwardsCalculationContext
            };
            Assert.AreEqual(breakWaterProperties.BreakWaterPresent, properties.BreakWater.BreakWaterPresent);
            Assert.AreEqual(breakWaterProperties.BreakWaterHeight, properties.BreakWater.BreakWaterHeight);
            Assert.AreEqual(breakWaterProperties.BreakWaterType, properties.BreakWater.BreakWaterType);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 2;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            calculation.Attach(observerMock);
            var properties = new GrassCoverErosionInwardsCalculationContextProperties
            {
                Data = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanismMock, assessmentSectionMock)
            };

            const double newDikeHeight = 9;
            var newOrientation = new RoundedDouble(2, 5);

            // Call
            properties.DikeHeight = newDikeHeight.ToString(CultureInfo.InvariantCulture);
            properties.Orientation = newOrientation.Value.ToString(CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual(newDikeHeight, calculation.InputParameters.DikeHeight);
            Assert.AreEqual(newOrientation, calculation.InputParameters.Orientation);
            mockRepository.VerifyAll();
        }
    }
}