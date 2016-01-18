using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Core.Common.Controls.Commands;
using Core.Plugins.DotSpatial.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.DotSpatial.Test
{
    [TestFixture]
    public class MapRibbonTest
    {
        [Test]
        [RequiresSTA]
        public void Commands_NoCommandsAssigned_ReturnsEmptyCommandCollection()
        {
            // Setup
            var ribbon = new MapRibbon();

            // Call / Assert
            CollectionAssert.IsEmpty(ribbon.Commands);
        }

        [Test]
        [RequiresSTA]
        public void Commands_CommandsAssigned_ReturnsAssignedCommands()
        {
            // Setup
            using (var plugin = new DotSpatialGuiPlugin())
            {
                var openMapViewCommand = new OpenMapViewCommand();
                var ribbon = new MapRibbon
                {
                    OpenMapViewCommand = openMapViewCommand
                };

                // Call
                var commands = ribbon.Commands.ToArray();

                // Assert
                CollectionAssert.AreEqual(new ICommand[] {openMapViewCommand}, commands);
            }
        }

        [Test]
        [RequiresSTA]
        public void DefaultConstructor_Always_CreatesControl()
        {
            // Setup
            var ribbon = new MapRibbon();

            // Call / Assert
            Assert.IsInstanceOf<Control>(ribbon.GetRibbonControl());
        }

        [Test]
        [RequiresSTA]
        public void IsContextualTabVisible_Always_ReturnsFalse()
        {
            // Setup
            var ribbon = new MapRibbon();

            // Call / Assert
            Assert.IsFalse(ribbon.IsContextualTabVisible(null, null));
        }

        [Test]
        [RequiresSTA]
        public void OpenMapViewButton_OnClick_ExecutesOpenMapViewCommand()
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Execute());

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                OpenMapViewCommand = command
            };

            var button = ribbon.GetRibbonControl().FindName("OpenMapViewButton") as Button;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have an open map view button");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
    }
}
