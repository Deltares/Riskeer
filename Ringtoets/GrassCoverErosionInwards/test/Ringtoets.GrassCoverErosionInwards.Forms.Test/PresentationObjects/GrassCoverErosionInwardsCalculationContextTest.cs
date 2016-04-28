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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationContextTest
    {
        private MockRepository mocksRepository;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
        }

        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var calculationMock = mocksRepository.StrictMock<GrassCoverErosionInwardsCalculation>();
            var failureMechanismMock = mocksRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            mocksRepository.ReplayAll();

            // Call
            var context = new GrassCoverErosionInwardsCalculationContext(calculationMock, failureMechanismMock, assessmentSectionMock);

            // Assert
            Assert.AreEqual(calculationMock, context.WrappedData);
            Assert.AreEqual(failureMechanismMock, context.GrassCoverErosionInwardsFailureMechanism);
            Assert.AreEqual(assessmentSectionMock, context.AssessmentSection);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanismMock = mocksRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            mocksRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationContext(null, failureMechanismMock, assessmentSectionMock);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("wrappedData", exception.ParamName);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMock = mocksRepository.StrictMock<GrassCoverErosionInwardsCalculation>();
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            mocksRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationContext(calculationMock, null, assessmentSectionMock);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMock = mocksRepository.StrictMock<GrassCoverErosionInwardsCalculation>();
            var failureMechanismMock = mocksRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            mocksRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculationContext(calculationMock, failureMechanismMock, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocksRepository.VerifyAll();
        }
    }
}