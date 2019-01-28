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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;

namespace Ringtoets.Integration.Data.Test
{
    [TestFixture]
    public class RingtoetsProjectTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Call
            var project = new RingtoetsProject();

            // Assert
            Assert.IsInstanceOf<IProject>(project);
            Assert.AreEqual("Project", project.Name);
            Assert.AreEqual("", project.Description);
            CollectionAssert.IsEmpty(project.AssessmentSections);
        }

        [Test]
        public void NameConstructor_SetNameAndInitializeOtherProperties()
        {
            // Setup
            const string someName = "<Some name>";

            // Call
            var project = new RingtoetsProject(someName);

            // Assert
            Assert.IsInstanceOf<IProject>(project);
            Assert.AreEqual(someName, project.Name);
            Assert.AreEqual("", project.Description);
            CollectionAssert.IsEmpty(project.AssessmentSections);
        }

        [Test]
        public void AutomaticProperties_SetAndGettingValue_ShouldReturnSetValue()
        {
            // Setup
            const string niceProjectName = "Nice project name";
            const string nicerDescription = "Nicer description";

            // Call
            var project = new RingtoetsProject
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

            var project = new RingtoetsProject();
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

            var project = new RingtoetsProject();
            project.Attach(observer);
            project.NotifyObservers();

            // Call
            project.Detach(observer);
            project.NotifyObservers();

            // Assert
            mockRepository.VerifyAll();
        }

        [TestFixture]
        private class RingtoetsProjectEqualsTest : EqualsTestFixture<RingtoetsProject, DerivedRingtoetsProject>
        {
            protected override RingtoetsProject CreateObject()
            {
                return CreateProject();
            }

            protected override DerivedRingtoetsProject CreateDerivedObject()
            {
                return new DerivedRingtoetsProject(CreateProject());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                RingtoetsProject baseProject = CreateProject();

                yield return new TestCaseData(new RingtoetsProject("Different name")
                {
                    Description = baseProject.Description
                }).SetName("Name");

                yield return new TestCaseData(new RingtoetsProject(baseProject.Name))
                    .SetName("Description");

                var random = new Random(21);
                RingtoetsProject differentAssessmentSections = CreateProject();
                differentAssessmentSections.AssessmentSections.Add(new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>()));
                yield return new TestCaseData(differentAssessmentSections)
                    .SetName("AssessmentSections");
            }

            private static RingtoetsProject CreateProject()
            {
                return new RingtoetsProject("Some name")
                {
                    Description = "Some desctiption"
                };
            }
        }

        private class DerivedRingtoetsProject : RingtoetsProject
        {
            public DerivedRingtoetsProject(RingtoetsProject project)
            {
                Name = project.Name;
                Description = project.Description;
            }
        }
    }
}