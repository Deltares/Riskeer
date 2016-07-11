using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class AssessmentSectionPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new AssessmentSectionProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<IAssessmentSection>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Id = "TestId",
                Name = "Test"
            };

            var properties = new AssessmentSectionProperties
            {
                Data = assessmentSection
            };

            // Call & Assert
            Assert.AreEqual(assessmentSection.Id, properties.Id);
            Assert.AreEqual(assessmentSection.Name, properties.Name);
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.Attach(projectObserver);

            var properties = new AssessmentSectionProperties
            {
                Data = assessmentSection
            };

            const string newName = "Test";

            // Call
            properties.Name = newName;

            // Assert
            Assert.AreEqual(newName, assessmentSection.Name);
            mocks.VerifyAll();
        }
    }
}