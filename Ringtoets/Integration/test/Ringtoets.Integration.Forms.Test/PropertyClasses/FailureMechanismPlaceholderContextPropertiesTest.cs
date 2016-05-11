using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class FailureMechanismPlaceholderContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var properties = new FailureMechanismPlaceholderContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismPlaceholderContext>>(properties);
        }

        [Test]
        public void Data_SetNewPipingFailureMechanismContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var failureMechanism = new FailureMechanismPlaceholder("testName", "testCode");
            var properties = new FailureMechanismPlaceholderContextProperties();

            // Call
            properties.Data = new FailureMechanismPlaceholderContext(failureMechanism, new MockRepository().StrictMock<IAssessmentSection>());

            // Assert
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
        } 
    }
}