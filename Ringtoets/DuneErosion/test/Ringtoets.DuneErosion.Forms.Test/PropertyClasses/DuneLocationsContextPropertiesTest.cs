using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.PropertyClasses;

namespace Ringtoets.DuneErosion.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DuneLocationsContextPropertiesTest
    {
        private const int requiredLocationsPropertyIndex = 0;

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new DuneErosionFailureMechanism();

            var location = new TestDuneLocation();
            var items = new ObservableList<DuneLocation>
            {
                location
            };

            var context = new DuneLocationsContext(items, failureMechanism, assessmentSection);

            // Call
            var properties = new DuneLocationsContextProperties
            {
                Data = context
            };

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(properties.Locations, typeof(DuneLocationProperties));
            Assert.AreEqual(1, properties.Locations.Length);
            Assert.IsTrue(TypeUtils.HasTypeConverter<DuneLocationsContextProperties,
                              ExpandableArrayConverter>(p => p.Locations));

            DuneLocationProperties duneLocationProperties= properties.Locations.First();
            Assert.AreEqual(location.Id, duneLocationProperties.Id);
            Assert.AreEqual(location.Name, duneLocationProperties.Name);
            Assert.AreEqual(location.CoastalAreaId, duneLocationProperties.CoastalAreaId);
            Assert.AreEqual(location.Offset, duneLocationProperties.Offset);
            Assert.AreEqual(location.Location, duneLocationProperties.Location);

            Assert.IsNaN(duneLocationProperties.WaterLevel);
            Assert.IsNaN(duneLocationProperties.WaveHeight);
            Assert.IsNaN(duneLocationProperties.WavePeriod);

            Assert.IsNaN(duneLocationProperties.TargetProbability);
            Assert.IsNaN(duneLocationProperties.TargetReliability);
            
            Assert.IsNaN(duneLocationProperties.CalculatedProbability);
            Assert.IsNaN(duneLocationProperties.CalculatedReliability);

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(CalculationConvergence.NotCalculated).DisplayName;
            Assert.AreEqual(convergenceValue, duneLocationProperties.Convergence);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new DuneErosionFailureMechanism();

            var items = new ObservableList<DuneLocation>
            {
                new TestDuneLocation()
            };
            var context = new DuneLocationsContext(items, failureMechanism, assessmentSection);

            // Call
            var properties = new DuneLocationsContextProperties
            {
                Data = context
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            const string expectedLocationsDisplayName = "Locaties";
            const string expectedLocationsDescription = "Locaties uit de hydraulische duinendatabase.";
            const string expectedLocationsCategory = "Algemeen";

            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<TypeConverter>(classTypeConverter);

            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor locationsProperty = dynamicProperties[requiredLocationsPropertyIndex];
            Assert.IsNotNull(locationsProperty);
            Assert.IsInstanceOf<ExpandableArrayConverter>(locationsProperty.Converter);
            Assert.IsTrue(locationsProperty.IsReadOnly);
            Assert.IsTrue(locationsProperty.IsBrowsable);
            Assert.AreEqual(expectedLocationsDisplayName, locationsProperty.DisplayName);
            Assert.AreEqual(expectedLocationsDescription, locationsProperty.Description);
            Assert.AreEqual(expectedLocationsCategory, locationsProperty.Category);
        }
    }
}
