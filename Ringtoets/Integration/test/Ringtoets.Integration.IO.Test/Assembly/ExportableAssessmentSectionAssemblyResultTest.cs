using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableAssessmentSectionAssemblyResultTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();
            var category = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

            // Call
            var assembly = new ExportableAssessmentSectionAssemblyResult(assemblyMethod, category);

            // Assert
            Assert.AreEqual(assemblyMethod, assembly.AssemblyMethod);
            Assert.AreEqual(category, assembly.AssemblyCategory);
        }
    }
}