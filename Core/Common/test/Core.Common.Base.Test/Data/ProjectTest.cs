// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.Data
{
    [TestFixture]
    public class ProjectTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValue()
        {
            // Call
            var project = new Project();

            // Assert
            Assert.IsInstanceOf<IObservable>(project);
            Assert.AreEqual("Project", project.Name);
            Assert.AreEqual("", project.Description);
            CollectionAssert.IsEmpty(project.Items);
        }

        [Test]
        public void NameConstructor_SetNameAndInitializeOtherProperties()
        {
            // Setup
            const string someName = "<Some name>";

            // Call
            var project = new Project(someName);

            // Assert
            Assert.IsInstanceOf<IObservable>(project);
            Assert.AreEqual(someName, project.Name);
            Assert.AreEqual("", project.Description);
            CollectionAssert.IsEmpty(project.Items);
        }

        [Test]
        public void AutomaticProperties_SetAndGettingValue_ShouldReturnSetValue()
        {
            // Setup
            const string niceProjectName = "Nice project name";
            const string nicerDescription = "Nicer description";

            // Call
            var project = new Project
            {
                Name = niceProjectName,
                Description = nicerDescription,
            };

            // Assert
            Assert.AreEqual(niceProjectName, project.Name);
            Assert.AreEqual(nicerDescription, project.Description);
        }

        [Test]
        public void NotifyObservers_WithObserverAttached_ObserverIsNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var project = new Project();
            project.Attach(observerMock);

            // Call
            project.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_AttachedObserverHasBeenDetached_ObserverShouldNoLongerBeNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var project = new Project();
            project.Attach(observerMock);
            project.NotifyObservers();

            // Call
            project.Detach(observerMock);
            project.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}