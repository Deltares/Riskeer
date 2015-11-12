using Core.Common.Base;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class RingtoetsProjectPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new RingtoetsProjectProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<RingtoetsProject>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var project = new RingtoetsProject
            {
                Name = "Test"
            };

            var properties = new RingtoetsProjectProperties
            {
                Data = project
            };

            // Call & Assert
            Assert.AreEqual(project.Name, properties.Name);
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var project = new RingtoetsProject();
            project.Attach(projectObserver);

            var properties = new RingtoetsProjectProperties
            {
                Data = project
            };

            const string newName = "Test";

            // Call
            properties.Name = newName;

            // Assert
            Assert.AreEqual(newName, project.Name);
            mocks.VerifyAll();
        }
    }
}