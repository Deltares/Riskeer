using System.Linq;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.ClosingStructures.Plugin.Test.PropertyInfos
{
    public class ClosingStructuresFailureMechanismContextPropertyInfoTest
    {
        private ClosingStructuresPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(ClosingStructuresFailureMechanismProperties));
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
            Assert.AreEqual(typeof(ClosingStructuresFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(ClosingStructuresFailureMechanismProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_Always_NewPropertiesWithFailureMechanismContextAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var context = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            var objectProperties = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<ClosingStructuresFailureMechanismProperties>(objectProperties);
            Assert.AreSame(failureMechanism, objectProperties.Data);

            mocks.VerifyAll();
        }
    }
}