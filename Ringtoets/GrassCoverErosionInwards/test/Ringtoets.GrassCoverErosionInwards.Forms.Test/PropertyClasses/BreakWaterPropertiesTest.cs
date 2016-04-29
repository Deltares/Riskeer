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

using System.Globalization;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class BreakWaterPropertiesTest
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
            // Setup & Call
            var properties = new BreakWaterProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionInwardsCalculationContext>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual(Resources.BreakWaterProperties_DisplayName, properties.ToString());
        }

        [Test]
        public void Data_SetNewCalculationContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var calculationMock = mockRepository.StrictMock<GrassCoverErosionInwardsCalculation>();
            mockRepository.ReplayAll();

            var properties = new BreakWaterProperties();

            // Call
            properties.Data = new GrassCoverErosionInwardsCalculationContext(calculationMock, failureMechanismMock, assessmentSectionMock);

            // Assert
            Assert.IsTrue(properties.BreakWaterPresent);
            Assert.AreEqual(BreakWaterType.Dam, properties.BreakWaterType);
            Assert.AreEqual("10", properties.BreakWaterHeight);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 3;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            calculation.Attach(observerMock);
            var properties = new BreakWaterProperties
            {
                Data = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanismMock, assessmentSectionMock)
            };

            const double newBreakWaterHeight = 9;
            const BreakWaterType newBreakWaterType = BreakWaterType.Wall;

            // Call
            properties.BreakWaterHeight = newBreakWaterHeight.ToString(CultureInfo.InvariantCulture);
            properties.BreakWaterType = newBreakWaterType;
            properties.BreakWaterPresent = false;

            // Assert
            Assert.IsFalse(calculation.InputParameters.UseBreakWater);
            Assert.AreEqual(newBreakWaterType, calculation.InputParameters.BreakWater.Type);
            Assert.AreEqual(newBreakWaterHeight, calculation.InputParameters.BreakWater.Height);
            mockRepository.VerifyAll();
        }
    }
}