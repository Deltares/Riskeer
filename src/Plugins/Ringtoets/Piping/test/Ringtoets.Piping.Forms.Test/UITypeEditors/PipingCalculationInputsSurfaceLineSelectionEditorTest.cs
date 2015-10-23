using System.Drawing.Design;

using NUnit.Framework;

using Ringtoets.Piping.Forms.UITypeEditors;

namespace Ringtoets.Piping.Forms.Test.UITypeEditors
{
    [TestFixture]
    public class PipingCalculationInputsSurfaceLineSelectionEditorTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var editor = new PipingCalculationInputsSurfaceLineSelectionEditor();

            // Assert
            Assert.IsInstanceOf<UITypeEditor>(editor);
        }

        [Test]
        public void GetEditStyle_Always_ReturnDropDown()
        {
            // Setup
            var editor = new PipingCalculationInputsSurfaceLineSelectionEditor();

            // Call
            var editStyle = editor.GetEditStyle();

            // Assert
            Assert.AreEqual(UITypeEditorEditStyle.DropDown, editStyle);
        }
    }
}