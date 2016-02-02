using System;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using Fluent;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class RibbonTest
    {
        [Test]
        [RequiresSTA]
        public void DefaultConstructor_CreatesNewInstance()
        {
            // Call
            var ribbon = new Ribbon();

            // Assert
            Assert.IsInstanceOf<IRibbonCommandHandler>(ribbon);
            Assert.IsInstanceOf<Fluent.Ribbon>(ribbon.GetRibbonControl());
            CollectionAssert.IsEmpty(ribbon.Commands);
        }

        [Test]
        [RequiresSTA]
        public void IsContextualTabVisible_Always_ReturnFalse()
        {
            // Setup
            var ribbon = new Ribbon();

            // Call
            var visible = ribbon.IsContextualTabVisible(null, null);

            // Assert
            Assert.IsFalse(visible);
        }

        [Test]
        [RequiresSTA]
        public void ValidateItems_ShowProjectExplorerCommandNotSet_ThrowsNullReferenceException()
        {
            // Setup
            var ribbon = new Ribbon();

            // Call
            TestDelegate test = () => ribbon.ValidateItems();

            // Assert
            Assert.Throws<NullReferenceException>(test);
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_ShowProjectExplorerCommandSet_ShowProjectButtonIsCheckedEqualToCommandChecked(bool isChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Checked).Return(isChecked);
            mocks.ReplayAll();

            var ribbon = new Ribbon
            {
                ToggleExplorerCommand = command
            };

            var toggleProjectExplorerButton = ribbon.GetRibbonControl().FindName("ToggleProjectExplorerButton") as ToggleButton;
            
            // Precondition
            Assert.IsNotNull(toggleProjectExplorerButton, "Ribbon should have a toggle project explorer button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(isChecked, toggleProjectExplorerButton.IsChecked);
        }
    }
}