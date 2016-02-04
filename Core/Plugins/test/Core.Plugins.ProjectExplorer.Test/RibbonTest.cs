using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using ToggleButton = Fluent.ToggleButton;

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
        public void Commands_ToggleExplorerCommandSet_ReturnsToggleExplorerCommand()
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            mocks.ReplayAll();

            var ribbon = new Ribbon
            {
                ToggleExplorerCommand = command
            };

            // Call
            var result = ribbon.Commands;

            // Assert
            CollectionAssert.AreEqual(new[] { command }, result);
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

        [Test]
        [RequiresSTA]
        public void ToggleExplorerCommand_ButtonShowProjectExplorerToolWindowOnClick_ExecutesCommandAndUpdatesButtonState()
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Execute());
            command.Expect(c => c.Checked).Return(false);
            mocks.ReplayAll();

            var ribbon = new Ribbon
            {
                ToggleExplorerCommand = command
            };

            var toggleProjectExplorerButton = ribbon.GetRibbonControl().FindName("ToggleProjectExplorerButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(toggleProjectExplorerButton, "Ribbon should have a toggle project explorer button.");

            // Call
            toggleProjectExplorerButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
    }
}