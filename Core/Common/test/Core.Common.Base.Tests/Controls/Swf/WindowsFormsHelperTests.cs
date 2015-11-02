using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Swf;
using NUnit.Framework;

namespace Core.Common.Base.Tests.Controls.Swf
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