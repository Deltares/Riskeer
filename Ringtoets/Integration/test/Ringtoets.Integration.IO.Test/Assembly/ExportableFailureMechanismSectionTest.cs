using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismSectionTest
    {
        [Test]
        public void Constructor_GeometryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => new ExportableFailureMechanismSection(null,
                                                                            random.NextDouble(),
                                                                            random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("geometry", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<Point2D> geometry = Enumerable.Empty<Point2D>();
            double startDistance = random.NextDouble();
            double endDistance = random.NextDouble();

            // Call
            var section = new ExportableFailureMechanismSection(geometry, startDistance, endDistance);

            // Assert
            Assert.AreSame(geometry, section.Geometry);
            Assert.AreEqual(startDistance, section.StartDistance);
            Assert.AreEqual(endDistance, section.EndDistance);
        }
    }
}