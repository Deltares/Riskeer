using Core.Common.Base;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
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
            Assert.IsInstanceOf<ObjectProperties<AssessmentSection>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection
            {
                Name = "Test"
            };

            var properties = new AssessmentSectionProperties
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

            var assessmentSection = new AssessmentSection();
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