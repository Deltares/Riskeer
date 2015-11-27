using System.Linq;

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
    public class PipingCalculationGroupContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingCalculationGroupContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingCalculationGroupContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnTheSameValueAsData()
        {
            // Setup
            var group = new PipingCalculationGroup();

            var properties = new PipingCalculationGroupContextProperties
            {
                Data = new PipingCalculationGroupContext(group,
                                                         Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                         Enumerable.Empty<PipingSoilProfile>())
            };

            // Call & Assert
            Assert.AreEqual(group.Name, properties.Name);
        }

        [Test]
        public void SetProperties_WithData_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var group = new PipingCalculationGroup();
            group.Attach(projectObserver);

            var properties = new PipingCalculationGroupContextProperties
            {
                Data = new PipingCalculationGroupContext(group,
                                                         Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                         Enumerable.Empty<PipingSoilProfile>())
            };

            // Call & Assert
            const string name = "cool new name!";
            properties.Name = name;
            Assert.AreEqual(name, group.Name);
            mocks.VerifyAll();
        }
    }
}