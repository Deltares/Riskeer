using System;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test
{
    [TestFixture]
    public class TestContextMenuBuilderProviderTest
    {
        [Test]
        public void Create_NoMocks_ArgumentException()
        {
            // Call & Assert
            Assert.Throws<ArgumentNullException>(() => TestContextMenuBuilderProvider.Create(null, null));
        } 

        [Test]
        public void Create_NoTreeNode_ArgumentException()
        {
            // Call & Assert
            Assert.Throws<ArgumentNullException>(() => TestContextMenuBuilderProvider.Create(new MockRepository(), null));
        }

        [Test]
        public void Create_MocksAndTreeNode_SetExpectationDefaults()
        {
            // Setup
            var mockRepository = new MockRepository();

            // Call
            var result = TestContextMenuBuilderProvider.Create(mockRepository, mockRepository.StrictMock<ITreeNode>());

            mockRepository.ReplayAll();

            // Expect
            ContextMenuStrip menu = result.Get(null).AddExportItem().AddImportItem().AddPropertiesItem().AddOpenItem().Build();
            Assert.IsFalse(menu.Items[0].Enabled);
            Assert.IsFalse(menu.Items[1].Enabled);
            Assert.IsFalse(menu.Items[2].Enabled);
            Assert.IsFalse(menu.Items[3].Enabled);
        } 

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_MocksAndTreeNodeWithExpectations_SetExpectation(bool expectation)
        {
            // Setup
            var mockRepository = new MockRepository();

            // Call
            var result = TestContextMenuBuilderProvider.Create(mockRepository, mockRepository.StrictMock<ITreeNode>(), expectation);

            mockRepository.ReplayAll();

            // Expect
            ContextMenuStrip menu = result.Get(null).AddExportItem().AddImportItem().AddPropertiesItem().AddOpenItem().Build();
            Assert.AreEqual(expectation, menu.Items[0].Enabled);
            Assert.AreEqual(expectation, menu.Items[1].Enabled);
            Assert.AreEqual(expectation, menu.Items[2].Enabled);
            Assert.AreEqual(expectation, menu.Items[3].Enabled);
        } 
    }
}