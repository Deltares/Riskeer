using NUnit.Framework;

using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class CategoryTreeFolderTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var contents = new[]
            {
                new object(), new object()
            };
            var category = TreeFolderCategory.Output;

            // Call
            var treeFolder = new CategoryTreeFolder("<name>", contents, category);

            // Assert
            Assert.AreEqual("<name>", treeFolder.Name);
            Assert.AreEqual(category, treeFolder.Category);
            Assert.AreNotSame(contents, treeFolder.Contents);
            CollectionAssert.AreEqual(contents, treeFolder.Contents);
        }

        [Test]
        public void ParameteredConstructor_NotSpecifyingCategory_ExpectedValues()
        {
            // Setup
            var contents = new[]
            {
                new object(), new object()
            };
            
            // Call
            var treeFolder = new CategoryTreeFolder("<name>", contents);

            // Assert
            Assert.AreEqual("<name>", treeFolder.Name);
            Assert.AreEqual(TreeFolderCategory.General, treeFolder.Category);
            Assert.AreNotSame(contents, treeFolder.Contents);
            CollectionAssert.AreEqual(contents, treeFolder.Contents);
        }
    }
}