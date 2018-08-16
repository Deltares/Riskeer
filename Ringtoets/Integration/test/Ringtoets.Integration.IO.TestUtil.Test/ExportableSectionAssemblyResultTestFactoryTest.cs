using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil.Test
{
    [TestFixture]
    public class ExportableSectionAssemblyResultTestFactoryTest
    {
        [Test]
        public void CreateSectionAssemblyResult_Always_ReturnsSectionAssemblyResult()
        {
            // Call
            ExportableSectionAssemblyResult result = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResult();

            // Assert
            Assert.AreEqual(ExportableAssemblyMethod.WBI0T1, result.AssemblyMethod);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIIv, result.AssemblyCategory);
        }

        [Test]
        public void CreateSectionAssemblyResultWithProbability_Always_ReturnsSectionAssemblyResult()
        {
            // Call
            ExportableSectionAssemblyResultWithProbability result = ExportableSectionAssemblyResultTestFactory.CreateSectionAssemblyResultWithProbability();

            // Assert
            Assert.AreEqual(ExportableAssemblyMethod.WBI0T1, result.AssemblyMethod);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIIv, result.AssemblyCategory);
            Assert.AreEqual(0.75, result.Probability);
        }
    }
}