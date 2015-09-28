using DelftTools.Shell.Gui;

using NUnit.Framework;

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
            // call
            var properties = new WtiProjectProperties();

            // assert
            Assert.IsInstanceOf<ObjectProperties<WtiProject>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // setup
            var project = new WtiProject
            {
                Name = "Test"
            };

            var properties = new WtiProjectProperties { Data = project };

            // call & Assert
            Assert.AreEqual(project.Name, properties.Name);
        }

        [Test]
        public void SetProperties_WithData_UpdateData()
        {
            // setup
            var project = new WtiProject();

            var properties = new WtiProjectProperties { Data = project };

            // call & Assert
            const string newName = "Test";
            properties.Name = newName;
            Assert.AreEqual(newName, project.Name);
        }


    }
}