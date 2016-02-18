using Core.Common.IO.Readers;
using NUnit.Framework;

namespace Core.Common.IO.Test.Readers
{
    [TestFixture]
    public class ReadResultTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_CriticalErrorOccuredOrNot_InitializesCollectionAndSetsCriticalErrorOccuredProperty(bool errorOccurred)
        {
            // Call
            var readResult = new ReadResult<object>(errorOccurred);

            // Assert
            CollectionAssert.IsEmpty(readResult.ImportedItems);
            Assert.AreEqual(errorOccurred, readResult.CriticalErrorOccurred);
        }
    }
}