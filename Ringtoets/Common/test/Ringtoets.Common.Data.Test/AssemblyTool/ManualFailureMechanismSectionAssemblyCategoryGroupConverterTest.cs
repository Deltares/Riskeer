using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class ManualFailureMechanismSectionAssemblyCategoryGroupConverterTest
    {
        [Test]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionAssemblyCategoryGroup.None)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.NotApplicable, FailureMechanismSectionAssemblyCategoryGroup.NotApplicable)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.Iv, FailureMechanismSectionAssemblyCategoryGroup.Iv)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionAssemblyCategoryGroup.IIv)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.Vv, FailureMechanismSectionAssemblyCategoryGroup.Vv)]
        [TestCase(ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv, FailureMechanismSectionAssemblyCategoryGroup.VIIv)]
        public void Convert_WithValidManualFailureMechanismSectionAssemblyCategoryGroup_ReturnsExpectedFailureMechanismSectionAssemblyCategoryGroup(
            ManualFailureMechanismSectionAssemblyCategoryGroup originalCategoryGroup,
            FailureMechanismSectionAssemblyCategoryGroup expectedCategoryGroup)
        {
            // Call
            FailureMechanismSectionAssemblyCategoryGroup result =
                ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(originalCategoryGroup);

            // Assert
            Assert.AreEqual(expectedCategoryGroup, result);
        }

        [Test]
        public void Convert_WithInvalidManualFailureMechanismSectionAssemblyCategoryGroup_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const ManualFailureMechanismSectionAssemblyCategoryGroup invalidCategoryGroup = (ManualFailureMechanismSectionAssemblyCategoryGroup) 99;

            // Call
            TestDelegate test = () => ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(invalidCategoryGroup);

            // Assert
            string expectedMessage = $"The value of argument 'categoryGroup' (99) is invalid for Enum type '{nameof(ManualFailureMechanismSectionAssemblyCategoryGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }
    }
}