using System.Linq;

using Core.Common.Base;
using Core.Common.Gui.PropertyBag;

using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingCalculationContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingCalculationContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingCalculationContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string name = "<very cool name>";
            var calculation = new PipingCalculation
            {
                Name = name
            };

            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var properties = new PipingCalculationContextProperties
            {
                Data = new PipingCalculationContext(calculation,
                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                    Enumerable.Empty<PipingSoilProfile>(),
                                                    pipingFailureMechanismMock)
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
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var calculation = new PipingCalculation();
            calculation.Attach(projectObserver);

            var properties = new PipingCalculationContextProperties
            {
                Data = new PipingCalculationContext(calculation,
                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                    Enumerable.Empty<PipingSoilProfile>(),
                                                    pipingFailureMechanismMock)
            };

            // Call & Assert
            const string newName = "haha";
            properties.Name = newName;
            Assert.AreEqual(newName, calculation.Name);
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
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var calculation = new PipingCalculation();
            calculation.Attach(projectObserver);

            var properties = new PipingCalculationContextProperties
            {
                Data = new PipingCalculationContext(calculation,
                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                    Enumerable.Empty<PipingSoilProfile>(),
                                                    pipingFailureMechanismMock)
            };

            // Call
            const string newName = "Some new cool pretty name";
            properties.Name = newName;

            // Assert
            Assert.AreEqual(newName, calculation.Name);

            mocks.VerifyAll();
        }
    }
}