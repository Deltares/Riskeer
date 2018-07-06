using NUnit.Framework;
using Ringtoets.Integration.Data.Merge;

namespace Ringtoets.Integration.Data.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionsOwnerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var owner = new AssessmentSectionsOwner();

            // Assert
            Assert.IsNull(owner.AssessmentSections);
        }
    }
}