using System;
using System.Collections.Generic;

using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class BaseFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "<cool name>";

            // Call
            var failureMechanism = new SimpleBaseFailureMechanism(name);

            // Assert
            Assert.IsInstanceOf<Observable>(failureMechanism);
            Assert.IsInstanceOf<IFailureMechanism>(failureMechanism);
            Assert.AreEqual(0, failureMechanism.Contribution);
            Assert.AreEqual(name, failureMechanism.Name);
            Assert.AreEqual(0, failureMechanism.StorageId);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        [TestCase(101)]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-1)]
        [TestCase(100+1e-6)]
        public void Contribution_ValueOutsideValidRegion_ThrowsArgumentException(double value)
        {
            // Setup
            var failureMechanism = new SimpleBaseFailureMechanism("");

            // Call
            TestDelegate test = () => failureMechanism.Contribution = value;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, Resources.FailureMechanism_Contribution_Value_should_be_in_interval_0_100);
        }

        [Test]
        [TestCase(100)]
        [TestCase(50)]
        [TestCase(1e-9)]
        public void Contribution_ValueIntsideValidRegion_DoesNotThrow(double value)
        {
            // Setup
            var failureMechanism = new SimpleBaseFailureMechanism("");

            // Call
            TestDelegate test = () => failureMechanism.Contribution = value;

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AddSection_SectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new SimpleBaseFailureMechanism("");

            // Call
            TestDelegate call = () => failureMechanism.AddSection(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void AddSection_FirstSectionAdded_SectionAddedToSections()
        {
            // Setup
            var failureMechanism = new SimpleBaseFailureMechanism("");

            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });

            // Call
            failureMechanism.AddSection(section);

            // Assert
            CollectionAssert.AreEqual(new[]{section}, failureMechanism.Sections);
        }

        [Test]
        public void AddSection_SecondSectionEndConnectingToStartOfFirst_Section2InsertedBeforeSection1()
        {
            // Setup
            var failureMechanism = new SimpleBaseFailureMechanism("");

            const int matchingX = 1;
            const int matchingY = 2;

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(matchingX, matchingY),
                new Point2D(3, 4)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(-2, -1),
                new Point2D(matchingX, matchingY)
            });

            // Call
            failureMechanism.AddSection(section1);
            failureMechanism.AddSection(section2);

            // Assert
            CollectionAssert.AreEqual(new[] { section2, section1 }, failureMechanism.Sections);
        }

        [Test]
        public void AddSection_SecondSectionStartConnectingToEndOfFirst_Section2AddedAfterSection1()
        {
            // Setup
            var failureMechanism = new SimpleBaseFailureMechanism("");

            const int matchingX = 1;
            const int matchingY = 2;

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(3, 4),
                new Point2D(matchingX, matchingY)
                
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(matchingX, matchingY),
                new Point2D(-2, -1)
            });

            // Call
            failureMechanism.AddSection(section1);
            failureMechanism.AddSection(section2);

            // Assert
            CollectionAssert.AreEqual(new[] { section1, section2 }, failureMechanism.Sections);
        }

        [Test]
        public void AddSection_SecondSectionDoesNotConnectToFirst_ThrowArgumentException()
        {
            // Setup
            var failureMechanism = new SimpleBaseFailureMechanism("");

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
                
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(5, 6),
                new Point2D(7, 8)
            });

            failureMechanism.AddSection(section1);

            // Call
            TestDelegate call = () => failureMechanism.AddSection(section2);

            // Assert
            string expectedMessage = "Vak 'B' sluit niet aan op de al gedefinieerde vakken van het faalmechanisme.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ClearAllSections_HasSections_ClearSections()
        {
            // Setup
            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });

            var failureMechanism = new SimpleBaseFailureMechanism("A");
            failureMechanism.AddSection(section);

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        private class SimpleBaseFailureMechanism : BaseFailureMechanism
        {
            public override IEnumerable<ICalculationItem> CalculationItems
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public SimpleBaseFailureMechanism(string failureMechanismName) : base(failureMechanismName) {}
        }
    }
}