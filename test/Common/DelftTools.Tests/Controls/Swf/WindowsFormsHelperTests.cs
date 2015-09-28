using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class WindowsFormsHelperTests
    {
        [Test]
        public void DragDropEffectsFromDragOperation()
        {
            Assert.AreEqual(DragDropEffects.Move, WindowsFormsHelper.ToDragDropEffects(DragOperations.Move));
            Assert.AreEqual(DragDropEffects.None, WindowsFormsHelper.ToDragDropEffects(DragOperations.None));
        }
    }
}