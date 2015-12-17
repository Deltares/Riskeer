using System.Linq;

using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

using PropertyInfo = System.Reflection.PropertyInfo;

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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Name_GroupHasEditableName_NameShouldNotBeReadonly(bool nameIsEditable)
        {
            // Setup
            var group = new PipingCalculationGroup("A", nameIsEditable);

            var properties = new PipingCalculationGroupContextProperties
            {
                Data = new PipingCalculationGroupContext(group,
                                                         Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                         Enumerable.Empty<PipingSoilProfile>())
            };

            string propertyName = TypeUtils.GetMemberName<PipingCalculationGroupContextProperties>(p => p.Name);
            PropertyInfo nameProperty = properties.GetType().GetProperty(propertyName);
            
            // Call
            object[] namePropertyAttributes = nameProperty.GetCustomAttributes(false);

            // Assert
            Assert.AreEqual(1, namePropertyAttributes.OfType<DynamicReadOnlyAttribute>().Count());
            Assert.AreEqual(!nameIsEditable, DynamicReadOnlyAttribute.IsReadOnly(properties, propertyName));
        }
    }
}