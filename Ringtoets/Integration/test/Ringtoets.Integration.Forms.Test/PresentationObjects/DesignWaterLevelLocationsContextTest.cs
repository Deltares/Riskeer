﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DesignWaterLevelLocationsContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var locations = new ObservableList<HydraulicBoundaryLocation>();
            Func<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> calculationFunc = hbl => null;
            const string categoryBoundaryName = "Test name";

            // Call
            var presentationObject = new DesignWaterLevelLocationsContext(locations,
                                                                          assessmentSection,
                                                                          calculationFunc,
                                                                          categoryBoundaryName);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableList<HydraulicBoundaryLocation>>>(presentationObject);
            Assert.AreSame(locations, presentationObject.WrappedData);
            Assert.AreSame(assessmentSection, presentationObject.AssessmentSection);
            Assert.AreSame(calculationFunc, presentationObject.GetCalculationFunc);
            Assert.AreEqual(categoryBoundaryName, presentationObject.CategoryBoundaryName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DesignWaterLevelLocationsContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                           null,
                                                                           hbl => null,
                                                                           "Test name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_GetCalculationFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new DesignWaterLevelLocationsContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                           assessmentSection,
                                                                           null,
                                                                           "Test name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getCalculationFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_CategoryBoundaryNameNull_ThrowsArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new DesignWaterLevelLocationsContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                           assessmentSection,
                                                                           hbl => null,
                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("categoryBoundaryName", exception.ParamName);
        }

        [Test]
        public void Constructor_CategoryBoundaryNameEmpty_ThrowsArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new DesignWaterLevelLocationsContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                           assessmentSection,
                                                                           hbl => null,
                                                                           string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("categoryBoundaryName", exception.ParamName);
        }
    }
}