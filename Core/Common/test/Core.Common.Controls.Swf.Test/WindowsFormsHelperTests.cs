using System.Windows.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.Swf.Test
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