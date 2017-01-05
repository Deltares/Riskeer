using System.Linq;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.HeightStructures.Plugin.Test.PropertyInfos
{
    public class HeightStructuresFailureMechanismContextPropertyInfoTest
    {
        private HeightStructuresPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new HeightStructuresPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(HeightStructuresFailureMechanismProperties));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(HeightStructuresFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(HeightStructuresFailureMechanismProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_Always_NewPropertiesWithFailureMechanismContextAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var context = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            var objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<HeightStructuresFailureMechanismProperties>(objectProperties);
            Assert.AreSame(failureMechanism, objectProperties.Data);

            mocks.VerifyAll();
        }
    }
}