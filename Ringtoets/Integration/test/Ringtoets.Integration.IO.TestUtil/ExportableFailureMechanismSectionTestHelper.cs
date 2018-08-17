using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert <see cref="ExportableFailureMechanismSection"/>
    /// </summary>
    public static class ExportableFailureMechanismSectionTestHelper
    {
        /// <summary>
        /// Asserts a collection of <see cref="ExportableFailureMechanismSection"/>
        /// against a collection of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="expectedSections">The expected <see cref="FailureMechanismSection"/>.</param>
        /// <param name="actualSections">The actual <see cref="ExportableFailureMechanismSection"/> to assert against.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The number of sections between <paramref name="expectedSections"/> and <paramref name="actualSections"/>
        /// do not match. </item>
        /// <item>The geometry of any the sections are not equal.</item>
        /// </list></exception>
        public static void AssertExportableFailureMechanismSections(IEnumerable<FailureMechanismSection> expectedSections,
                                                                    IEnumerable<ExportableFailureMechanismSection> actualSections)
        {
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, actualSections.Count());
            for (var i = 0; i < expectedNrOfSections; i++)
            {
                FailureMechanismSection expectedSection = expectedSections.ElementAt(i);
                ExportableFailureMechanismSection actualSection = actualSections.ElementAt(i);

                AssertExportableFailureMechanismSection(expectedSection, actualSection);
            }
        }

        private static void AssertExportableFailureMechanismSection(FailureMechanismSection expectedSection,
                                                                    ExportableFailureMechanismSection actualSection)
        {
            Assert.IsNaN(actualSection.StartDistance);
            Assert.IsNaN(actualSection.EndDistance);
            CollectionAssert.AreEqual(expectedSection.Points, actualSection.Geometry);
        }
    }
}