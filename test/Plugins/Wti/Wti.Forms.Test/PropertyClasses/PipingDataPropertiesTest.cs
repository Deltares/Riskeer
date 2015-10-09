using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Wti.Data;
using Wti.Forms.PropertyClasses;

namespace Wti.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingDataPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var properties = new PipingDataProperties();

            // assert
            Assert.IsInstanceOf<ObjectProperties<PipingData>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // setup
            var pipingData = new PipingData
            {
                AssessmentLevel = 0.13
            };

            var properties = new PipingDataProperties
            {
                Data = pipingData
            };

            // call & Assert
            Assert.AreEqual(pipingData.AssessmentLevel, properties.AssessmentLevel);
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingData = new PipingData();
            pipingData.Attach(projectObserver);

            var properties = new PipingDataProperties
            {
                Data = pipingData
            };

            // call & Assert
            const double assessmentLevel = 0.12;
            properties.AssessmentLevel = assessmentLevel;
            Assert.AreEqual(assessmentLevel, pipingData.AssessmentLevel);
            mocks.VerifyAll();
        }
    }
}