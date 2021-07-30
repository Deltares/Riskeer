// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class WaterLevelCalculationsForNormTargetProbabilityContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>();
            double GetNormFunc() => 0.01;

            // Call
            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(calculations,
                                                                                    assessmentSection,
                                                                                    GetNormFunc);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IObservableEnumerable<HydraulicBoundaryLocationCalculation>>>(context);
            Assert.AreSame(calculations, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame((Func<double>) GetNormFunc, context.GetNormFunc);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new WaterLevelCalculationsForNormTargetProbabilityContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                     null,
                                                                                     () => 0.01);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_GetNormFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new WaterLevelCalculationsForNormTargetProbabilityContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                     assessmentSection,
                                                                                     null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getNormFunc", exception.ParamName);
        }
    }
}