using Core.Common.Gui.Forms;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Forms
{
    [TestFixture]
    public class RichTextFileTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup

            // Call
            var file = new RichTextFile();

            // Assert
            Assert.IsNull(file.Name);
            Assert.IsNull(file.FilePath);
        }

        [Test]
        public void SimpleProperties_SetNewValue_GetNewlySetValue()
        {
            // Setup
            var file = new RichTextFile();

            const string newName = "<name>";
            const string newPath = "<path>";

            // Call
            file.Name = newName;
            file.FilePath = newPath;

            // Assert
            Assert.AreEqual(newName, file.Name);
            Assert.AreEqual(newPath, file.FilePath);
        }
    }
}