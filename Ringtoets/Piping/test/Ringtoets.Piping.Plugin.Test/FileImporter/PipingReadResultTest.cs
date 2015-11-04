using NUnit.Framework;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingReadResultTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_CriticalErrorOccuredOrNot_InitializesCollectionAndSetsCriticalErrorOccuredProperty(bool errorOccurred)
        {
            // Call
            var readResult = new PipingReadResult<object>(errorOccurred);

            // Assert
            CollectionAssert.IsEmpty(readResult.ImportedItems);
            Assert.AreEqual(errorOccurred, readResult.CriticalErrorOccurred);
        } 
    }
}