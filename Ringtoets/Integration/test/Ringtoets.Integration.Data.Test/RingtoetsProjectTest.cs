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

using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;

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
        public void Equals_NewProject_ReturnsTrue()
        {
            // Setup
            var newProjectA = new RingtoetsProject();
            var newProjectB = new RingtoetsProject();

            // Call
            bool result = newProjectA.Equals(newProjectB);
            bool result2 = newProjectB.Equals(newProjectA);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(result2);
        }

        [Test]
        public void Equals_ProjectNameChanged_ReturnsFalse()
        {
            // Setup
            var newProject = new RingtoetsProject();
            var changedProject = new RingtoetsProject
            {
                Name = "<some name>"
            };

            // Call
            bool result = newProject.Equals(changedProject);
            bool result2 = changedProject.Equals(newProject);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(result2);
        }

        [Test]
        public void Equals_ProjectDescriptionChanged_ReturnsFalse()
        {
            // Setup
            var newProject = new RingtoetsProject();
            var changedProject = new RingtoetsProject
            {
                Description = "<some description>"
            };

            // Call
            bool result = newProject.Equals(changedProject);
            bool result2 = changedProject.Equals(newProject);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(result2);
        }

        [Test]
        public void Equals_ProjectAssessmentSectionsChanged_ReturnsFalse()
        {
            // Setup
            var newProject = new RingtoetsProject();
            var changedProject = new RingtoetsProject();
            newProject.AssessmentSections.Add(new AssessmentSection(AssessmentSectionComposition.Dike));

            // Call
            bool result = newProject.Equals(changedProject);
            bool result2 = changedProject.Equals(newProject);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(result2);
        }

        [Test]
        public void GetHashCode_ProjectsAreEqual_ReturnsEqualHashes()
        {
            // Setup
            const string name = "Some name";
            const string desctiption = "Some desctiption";
            var project = new RingtoetsProject(name)
            {
                Description = desctiption
            };
            var otherProject = new RingtoetsProject(name)
            {
                Description = desctiption
            };

            // Call
            int result = project.GetHashCode();
            int otherResult = otherProject.GetHashCode();

            // Assert
            Assert.AreEqual(result, otherResult);
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var project = new RingtoetsProject();
            project.Attach(observerMock);

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
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var project = new RingtoetsProject();
            project.Attach(observerMock);
            project.NotifyObservers();

            // Call
            project.Detach(observerMock);
            project.NotifyObservers();

            // Assert
            mockRepository.VerifyAll();
        }
    }
}