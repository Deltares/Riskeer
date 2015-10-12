﻿using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Wti.Data;
using Wti.Forms.PropertyClasses;

namespace Wti.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WtiProjectPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new WtiProjectProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WtiProject>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var project = new WtiProject
            {
                Name = "Test"
            };

            var properties = new WtiProjectProperties
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

            var project = new WtiProject();
            project.Attach(projectObserver);

            var properties = new WtiProjectProperties
            {
                Data = project
            };

            // Call & Assert
            const string newName = "Test";
            properties.Name = newName;
            Assert.AreEqual(newName, project.Name);
            mocks.VerifyAll();
        }
    }
}