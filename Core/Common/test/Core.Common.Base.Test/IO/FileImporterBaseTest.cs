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

using System;
using System.Collections.Generic;
using Core.Common.Base.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Base.Test.IO
{
    [TestFixture]
    public class FileImporterBaseTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var simpleImporter = new SimpleFileImporter();

            // Assert
            Assert.IsInstanceOf<IFileImporter>(simpleImporter);
        }

        [Test]
        public void DoPostImportUpdates_TargetIsObservable_NotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observableInstance = mocks.Stub<IObservable>();
            observableInstance.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var simpleImporter = new SimpleFileImporter();

            // Call
            simpleImporter.DoPostImportUpdates(observableInstance);

            // Assert
            mocks.VerifyAll(); // Assert NotifyObservers is called
        }

        [Test]
        public void DoPostImportUpdates_HasAffectedOtherObjects_NotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observableInstance = mocks.Stub<IObservable>();
            observableInstance.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var simpleImporter = new SimpleFileImporter
            {
                AffectedNonTargetObservableInstancesOverride = new[]
                {
                    observableInstance
                }
            };

            // Call
            simpleImporter.DoPostImportUpdates(new object());

            // Assert
            mocks.VerifyAll(); // Assert NotifyObservers is called
        }

        [Test]
        public void DoPostImportUpdates_ImportCancelled_NoNotifyObserversCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var observableInstance = mocks.StrictMock<IObservable>();

            var observableTarget = mocks.StrictMock<IObservable>();
            mocks.ReplayAll();

            var simpleImporter = new SimpleFileImporter
            {
                AffectedNonTargetObservableInstancesOverride = new[]
                {
                    observableInstance
                }
            };
            simpleImporter.Cancel();

            // Call
            simpleImporter.DoPostImportUpdates(observableTarget);

            // Assert
            mocks.VerifyAll(); // Assert no NotifyObservers were called
        }

        [Test]
        public void NotifyProgress_NoDelegateSet_DoNothing()
        {
            // Setup
            var simpleImporter = new SimpleFileImporter
            {
                ProgressChanged = null
            };

            // Call
            TestDelegate call = () => simpleImporter.TestNotifyProgress("A", 1, 2);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void NotifyProgress_DelegateSet_CallProgressChanged()
        {
            // Setup
            var expectedDescription = "B";
            var expectedStep = 1;
            var expectedNumberOfSteps = 3;

            var simpleImporter = new SimpleFileImporter();
            int progressChangedCallCount = 0;
            simpleImporter.ProgressChanged = (description, step, steps) =>
            {
                Assert.AreEqual(expectedDescription, description);
                Assert.AreEqual(expectedStep, step);
                Assert.AreEqual(expectedNumberOfSteps, steps);
                progressChangedCallCount++;
            };

            // Call
            simpleImporter.TestNotifyProgress(expectedDescription, expectedStep, expectedNumberOfSteps);

            // Assert
            Assert.AreEqual(1, progressChangedCallCount);
        }

        private class SimpleFileImporter : FileImporterBase
        {
            public SimpleFileImporter() : base("") {}

            public override ProgressChangedDelegate ProgressChanged { protected get; set; }
            public IObservable[] AffectedNonTargetObservableInstancesOverride { private get; set; }

            public void TestNotifyProgress(string currentStepName, int currentStep, int totalNumberOfSteps)
            {
                NotifyProgress(currentStepName, currentStep, totalNumberOfSteps);
            }

            public override bool Import()
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<IObservable> AffectedNonTargetObservableInstances
            {
                get
                {
                    return AffectedNonTargetObservableInstancesOverride ?? base.AffectedNonTargetObservableInstances;
                }
            }
        }
    }
}