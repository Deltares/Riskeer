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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DesignWaterLevelCalculationsContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>();
            Func<double> getNormFunc = () => 0.01;
            const string categoryBoundaryName = "Test name";

            // Call
            var context = new DesignWaterLevelCalculationsContext(calculations,
                                                                  assessmentSection,
                                                                  getNormFunc,
                                                                  categoryBoundaryName);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ObservableList<HydraulicBoundaryLocationCalculation>>>(context);
            Assert.AreSame(calculations, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(getNormFunc, context.GetNormFunc);
            Assert.AreEqual(categoryBoundaryName, context.CategoryBoundaryName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                              null,
                                                                              () => 0.01,
                                                                              "Test name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            TestDelegate call = () => new DesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                              assessmentSection,
                                                                              null,
                                                                              "Test name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getNormFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_CategoryBoundaryNameNull_ThrowsArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                              assessmentSection,
                                                                              () => 0.01,
                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("'categoryBoundaryName' must have a value.", exception.Message);
        }

        [Test]
        public void Constructor_CategoryBoundaryNameEmpty_ThrowsArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                              assessmentSection,
                                                                              () => 0.01,
                                                                              string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("'categoryBoundaryName' must have a value.", exception.Message);
        }

        [TestFixture]
        private class DesignWaterLevelCalculationsContextEqualsTest
            : EqualsTestFixture<DesignWaterLevelCalculationsContext, DerivedDesignWaterLevelCalculationsContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly Func<double> getNormFunc = () => 0.01;
            private static readonly ObservableList<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();
            private static readonly string categoryBoundaryName = "Test name";

            [SetUp]
            public void SetUp()
            {
                mocks.ReplayAll();
            }

            [TearDown]
            public void TearDown()
            {
                mocks.VerifyAll();
            }

            protected override DesignWaterLevelCalculationsContext CreateObject()
            {
                return new DesignWaterLevelCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                               assessmentSection,
                                                               getNormFunc,
                                                               categoryBoundaryName);
            }

            protected override DerivedDesignWaterLevelCalculationsContext CreateDerivedObject()
            {
                return new DerivedDesignWaterLevelCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                                      assessmentSection,
                                                                      getNormFunc,
                                                                      categoryBoundaryName);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new DesignWaterLevelCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                                                      assessmentSection,
                                                                                      getNormFunc,
                                                                                      "Other"))
                    .SetName("CategoryBoundaryName");
            }
        }

        private class DerivedDesignWaterLevelCalculationsContext : DesignWaterLevelCalculationsContext
        {
            public DerivedDesignWaterLevelCalculationsContext(ObservableList<HydraulicBoundaryLocationCalculation> wrappedData,
                                                              IAssessmentSection assessmentSection,
                                                              Func<double> getNormFunc,
                                                              string categoryBoundaryName)
                : base(wrappedData,
                       assessmentSection,
                       getNormFunc,
                       categoryBoundaryName) {}
        }
    }
}