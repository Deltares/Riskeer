using System;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class SimpleFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithoutSectionResult_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SimpleFailureMechanismSectionResultRow(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Constructor_WithSectionResult_PropertiesFromSectionAndResult()
        {
            // Setup
            var section = CreateSection();
            var result = new SimpleFailureMechanismSectionResult(section);

            // Call
            var row = new SimpleFailureMechanismSectionResultRow(result);

            // Assert
            Assert.AreEqual(section.Name, row.Name);
            Assert.AreEqual(result.AssessmentLayerOne, row.AssessmentLayerOne);
            Assert.AreEqual(result.AssessmentLayerTwoA, row.AssessmentLayerTwoA);
            Assert.AreEqual(result.AssessmentLayerTwoB, row.AssessmentLayerTwoB);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_AlwaysOnChange_NotifyObserversOfResultAndResultPropertyChanged(bool newValue)
        {
            // Setup
            var section = CreateSection();
            var result = new SimpleFailureMechanismSectionResult(section);
            var row = new SimpleFailureMechanismSectionResultRow(result);

            int counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = result
            })
            {
                // Call
                row.AssessmentLayerOne = newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(newValue, result.AssessmentLayerOne);
            }
        }

        [Test]
        public void AssessmentLayerTwoA_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var newValue = AssessmentLayerTwoAResult.Successful;
            var section = CreateSection();
            var result = new SimpleFailureMechanismSectionResult(section);
            var row = new SimpleFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoA = newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerTwoA);
        }


        [Test]
        public void AssessmentLayerTwoB_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            var section = CreateSection();
            var result = new SimpleFailureMechanismSectionResult(section);
            var row = new SimpleFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerTwoB = (RoundedDouble)newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerTwoB, row.AssessmentLayerTwoB.GetAccuracy());
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            var section = CreateSection();
            var result = new SimpleFailureMechanismSectionResult(section);
            var row = new SimpleFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerThree = (RoundedDouble)newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerThree, row.AssessmentLayerThree.GetAccuracy());
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });
        }  
    }
}