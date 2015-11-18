using Core.Common.Base;
using Core.Common.Gui;

using NUnit.Framework;

using Rhino.Mocks;

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
            var properties = new DikeAssessmentSectionProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<DikeAssessmentSection>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var assessmentSection = new DikeAssessmentSection
            {
                Name = "Test"
            };

            var properties = new DikeAssessmentSectionProperties
            {
                Data = assessmentSection
            };

            // Call & Assert
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

            var assessmentSection = new DikeAssessmentSection();
            assessmentSection.Attach(projectObserver);

            var properties = new DikeAssessmentSectionProperties
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