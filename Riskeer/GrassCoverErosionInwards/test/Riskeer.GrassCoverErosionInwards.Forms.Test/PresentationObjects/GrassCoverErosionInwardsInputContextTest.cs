﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.PresentationObjects
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
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            var context = new GrassCoverErosionInwardsInputContext(input, calculation, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionInwardsContext<GrassCoverErosionInwardsInput>>(context);
            Assert.AreSame(input, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            void Call() => new GrassCoverErosionInwardsInputContext(input, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocksRepository.VerifyAll();
        }
    }
}