using System;
using System.Collections.Generic;
using System.Drawing;

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
            // Setup

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

            var simpleImporter = new SimpleFileImporter();
            simpleImporter.GetAffectedNonTargetObservableInstancesOverride = new[]
            {
                observableInstance
            };

            // Call
            simpleImporter.DoPostImportUpdates(new object());

            // Assert
            mocks.VerifyAll(); // Assert NotifyObservers is called
        }

        private class SimpleFileImporter : FileImporterBase
        {
            public override string Name
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string Category
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override Bitmap Image
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override Type SupportedItemType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override string FileFilter
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override ProgressChangedDelegate ProgressChanged { protected get; set; }
            public IObservable[] GetAffectedNonTargetObservableInstancesOverride { get; set; }

            protected override IEnumerable<IObservable> GetAffectedNonTargetObservableInstances()
            {
                return GetAffectedNonTargetObservableInstancesOverride ?? base.GetAffectedNonTargetObservableInstances();
            }

            public override bool Import(object targetItem, string filePath)
            {
                throw new NotImplementedException();
            }

            public override void Cancel()
            {
                throw new NotImplementedException();
            }
        }
    }
}