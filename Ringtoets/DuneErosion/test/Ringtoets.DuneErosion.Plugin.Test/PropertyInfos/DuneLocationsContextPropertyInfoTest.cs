using System.Linq;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.PropertyClasses;

namespace Ringtoets.DuneErosion.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class DuneLocationsContextPropertyInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                PropertyInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(DuneLocationsContext), info.DataType);
                Assert.AreEqual(typeof(DuneLocationsContextProperties), info.PropertyObjectType);
            }
        }

        [Test]
        public void CreateInstance_Always_SetsHydraulicBoundaryLocationsAsData()
        {
            // Setup
            MockRepository mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new DuneLocationsContext(failureMechanism.DuneLocations,
                                                   failureMechanism, assessmentSection);

            using (DuneErosionPlugin plugin = new DuneErosionPlugin())
            {
                PropertyInfo info = GetInfo(plugin);

                // Call
                var objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<DuneLocationsContextProperties>(objectProperties);
                Assert.AreSame(context, objectProperties.Data);
            }
            mockRepository.VerifyAll();
        }

        private static PropertyInfo GetInfo(DuneErosionPlugin plugin)
        {
            return plugin.GetPropertyInfos().First(pi => pi.DataType == typeof(DuneLocationsContext));
        }
    }
}