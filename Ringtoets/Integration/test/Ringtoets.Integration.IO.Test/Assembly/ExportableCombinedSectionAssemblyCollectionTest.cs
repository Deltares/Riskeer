using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableCombinedSectionAssemblyCollectionTest
    {
        [Test]
        public void Constructor_SectionsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssemblyCollection(null,
                                                                                      Enumerable.Empty<ExportableCombinedSectionAssembly>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void Constructor_CombinedSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableCombinedSectionAssemblyCollection(Enumerable.Empty<ExportableCombinedFailureMechanismSection>(),
                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("combinedSectionAssemblyResults", exception.ParamName);
        }


        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            IEnumerable<ExportableCombinedFailureMechanismSection> sections = Enumerable.Empty<ExportableCombinedFailureMechanismSection>();
            IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults = Enumerable.Empty<ExportableCombinedSectionAssembly>();

            // Call
            var assembly = new ExportableCombinedSectionAssemblyCollection(sections, combinedSectionAssemblyResults);

            // Assert
            Assert.AreSame(sections, assembly.Sections);
            Assert.AreSame(combinedSectionAssemblyResults, assembly.CombinedSectionAssemblyResults);
        }
    }
}