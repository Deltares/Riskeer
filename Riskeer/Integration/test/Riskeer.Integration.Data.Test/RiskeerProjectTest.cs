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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Integration.Data.Test
{
    [TestFixture]
    public class RiskeerProjectTest
    {
        [Test]
        public void DefaultConstructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new RiskeerProject(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Setup
            AssessmentSection assessmentSection = CreateAssessmentSection();

            // Call
            var project = new RiskeerProject(assessmentSection);

            // Assert
            Assert.IsInstanceOf<IProject>(project);
            Assert.AreEqual("Project", project.Name);
            Assert.AreEqual("", project.Description);
            Assert.AreSame(assessmentSection, project.AssessmentSection);
        }

        [Test]
        public void NameConstructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new RiskeerProject(string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void NameConstructor_SetNameAndInitializeOtherProperties()
        {
            // Setup
            const string someName = "<Some name>";
            AssessmentSection assessmentSection = CreateAssessmentSection();

            // Call
            var project = new RiskeerProject(someName, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IProject>(project);
            Assert.AreEqual(someName, project.Name);
            Assert.AreEqual("", project.Description);
            Assert.AreSame(assessmentSection, project.AssessmentSection);
        }

        [Test]
        public void AutomaticProperties_SetAndGettingValue_ShouldReturnSetValue()
        {
            // Setup
            const string niceProjectName = "Nice project name";
            const string nicerDescription = "Nicer description";

            // Call
            var project = new RiskeerProject(CreateAssessmentSection())
            {
                Name = niceProjectName,
                Description = nicerDescription
            };

            // Assert
            Assert.AreEqual(niceProjectName, project.Name);
            Assert.AreEqual(nicerDescription, project.Description);
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var project = new RiskeerProject(CreateAssessmentSection());
            project.Attach(observer);

            // Call
            project.NotifyObservers();

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void NotifyObservers_AttachedObserverHasBeenDetached_ObserverShouldNoLongerBeNotified()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var project = new RiskeerProject(CreateAssessmentSection());
            project.Attach(observer);
            project.NotifyObservers();

            // Call
            project.Detach(observer);
            project.NotifyObservers();

            // Assert
            mockRepository.VerifyAll();
        }

        private static AssessmentSection CreateAssessmentSection()
        {
            var random = new Random(21);
            return new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
        }

        [TestFixture]
        private class RiskeerProjectEqualsTest : EqualsTestFixture<RiskeerProject, DerivedRiskeerProject>
        {
            private static readonly AssessmentSection assessmentSection = CreateAssessmentSection();

            protected override RiskeerProject CreateObject()
            {
                return CreateProject();
            }

            protected override DerivedRiskeerProject CreateDerivedObject()
            {
                return new DerivedRiskeerProject(CreateProject());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                RiskeerProject baseProject = CreateProject();

                yield return new TestCaseData(new RiskeerProject("Different name", assessmentSection)
                {
                    Description = baseProject.Description
                }).SetName("Name");

                yield return new TestCaseData(new RiskeerProject(baseProject.Name, assessmentSection))
                    .SetName("Description");

                var differentAssessmentSections = new RiskeerProject("Some name", CreateAssessmentSection())
                {
                    Description = "Some description"
                };
                yield return new TestCaseData(differentAssessmentSections)
                    .SetName("AssessmentSections");
            }

            private static RiskeerProject CreateProject()
            {
                return new RiskeerProject("Some name", assessmentSection)
                {
                    Description = "Some description"
                };
            }
        }

        private class DerivedRiskeerProject : RiskeerProject
        {
            public DerivedRiskeerProject(RiskeerProject project) : base(project.AssessmentSection)
            {
                Name = project.Name;
                Description = project.Description;
            }
        }
    }
}