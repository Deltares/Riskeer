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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputContextTest
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
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var inputMock = mocksRepository.StrictMock<GrassCoverErosionInwardsInput>(generalInput);
            var calculationMock = mocksRepository.StrictMock<ICalculation>();
            var failureMechanismMock = mocksRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            mocksRepository.ReplayAll();

            // Call
            var context = new GrassCoverErosionInwardsInputContext(inputMock, calculationMock, failureMechanismMock, assessmentSectionMock);

            // Assert
            Assert.AreEqual(inputMock, context.WrappedData);
            Assert.AreEqual(calculationMock, context.Calculation);
            Assert.AreEqual(failureMechanismMock, context.GrassCoverErosionInwardsFailureMechanism);
            Assert.AreEqual(assessmentSectionMock, context.AssessmentSection);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullInput_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMock = mocksRepository.StrictMock<ICalculation>();
            var failureMechanismMock = mocksRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            mocksRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsInputContext(null, calculationMock, failureMechanismMock, assessmentSectionMock);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("wrappedData", exception.ParamName);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var inputMock = mocksRepository.StrictMock<GrassCoverErosionInwardsInput>(generalInput);
            var failureMechanismMock = mocksRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            mocksRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsInputContext(inputMock, null, failureMechanismMock, assessmentSectionMock);

            // Assert
            var message = String.Format(Resources.GrassCoverErosionInwardsContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                        Resources.GrassCoverErosionInwardsInputContext_DataDescription_GrassCoverErosionInwardsInputCalculationItem);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, message);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var inputMock = mocksRepository.StrictMock<GrassCoverErosionInwardsInput>(generalInput);
            var calculationMock = mocksRepository.StrictMock<ICalculation>();
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            mocksRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsInputContext(inputMock, calculationMock, null, assessmentSectionMock);

            // Assert
            var message = String.Format(Resources.GrassCoverErosionInwardsContext_AssertInputsAreNotNull_DataDescription_0_cannot_be_null,
                                        Resources.GrassCoverErosionInwardsContext_DataDescription_GrassCoverErosionInwardsFailureMechanism);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, message);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var inputMock = mocksRepository.StrictMock<GrassCoverErosionInwardsInput>(generalInput);
            var calculationMock = mocksRepository.StrictMock<ICalculation>();
            var failureMechanismMock = mocksRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            mocksRepository.ReplayAll();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsInputContext(inputMock, calculationMock, failureMechanismMock, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mocksRepository.VerifyAll();
        }
    }
}