using Core.Common.Base;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingCalculationInputsPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingCalculationInputsProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingCalculationInputs>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string name = "<very cool name>";
            var pipingData = new PipingCalculationData
            {
                Name = name
            };

            var properties = new PipingCalculationInputsProperties
            {
                Data = new PipingCalculationInputs
                {
                    PipingData = pipingData
                }
            };

            // Call & Assert
            Assert.AreEqual(name, properties.Name);
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingData = new PipingCalculationData();
            pipingData.Attach(projectObserver);

            var properties = new PipingCalculationInputsProperties
            {
                Data = new PipingCalculationInputs
                {
                    PipingData = pipingData
                }
            };

            // Call & Assert
            const string newName = "haha";
            properties.Name = newName;
            Assert.AreEqual(newName, pipingData.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            int numberProperties = 1;
            projectObserver.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mocks.ReplayAll();

            var pipingData = new PipingCalculationData();
            pipingData.Attach(projectObserver);

            // Call
            new PipingCalculationInputsProperties
            {
                Data = new PipingCalculationInputs
                {
                    PipingData = pipingData
                },
                Name = string.Empty
            };

            // Assert
            Assert.AreEqual(string.Empty, pipingData.Name);

            mocks.VerifyAll();
        }
    }
}