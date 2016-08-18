using System;
using NUnit.Framework;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class HydraulicBoundaryLocationFilterTest
    {
        [Test]
        [TestCase(-1)]
        [TestCase(long.MinValue)]
        [TestCase(303192)]
        [TestCase(303194)]
        [TestCase(603074)]
        [TestCase(603076)]
        [TestCase(long.MaxValue)]
        public void ShouldInclude_NameNotInFilterSet_ReturnsTrue(long id)
        {
            // Setup
            var filter = new HydraulicBoundaryLocationFilter();

            // Call
            bool shouldBeIncluded = filter.ShouldInclude(id);

            // Assert
            Assert.IsTrue(shouldBeIncluded);
        }

        [Test]
        [TestCase(303193)]
        [TestCase(603075)]
        public void ShouldInclude_NameInFilterSet_ReturnsFalse(long id)
        {
            // Setup
            var filter = new HydraulicBoundaryLocationFilter();

            // Call
            bool shouldBeIncluded = filter.ShouldInclude(id);

            // Assert
            Assert.IsFalse(shouldBeIncluded);
        }
    }
}