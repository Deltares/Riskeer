using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableCombinedFailureMechanismSectionTest
    {
        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();
            double startDistance = random.NextDouble();
            double endDistance = random.NextDouble();

            // Call
            var section = new ExportableCombinedFailureMechanismSection(geometry, startDistance, endDistance, assemblyMethod);

            // Assert
            Assert.IsInstanceOf<ExportableFailureMechanismSection>(section);

            Assert.AreSame(geometry, section.Geometry);
            Assert.AreEqual(startDistance, section.StartDistance);
            Assert.AreEqual(endDistance, section.EndDistance);
            Assert.AreEqual(assemblyMethod, section.AssemblyMethod);
        }
    }
}