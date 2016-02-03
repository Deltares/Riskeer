using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Components.DotSpatial;
using Core.Plugins.DotSpatial.Commands;
using Core.Plugins.DotSpatial.Legend;
using Fluent;
using NUnit.Framework;
using Rhino.Mocks;
using ToggleButton = Fluent.ToggleButton;

namespace Core.Plugins.DotSpatial.Test
{
    [TestFixture]
    public class MapRibbonTest
    {
        [Test]
        [RequiresSTA]
        public void DefaultContstructor_Always_CreatesControl()
        {
            // Setup
            var ribbon = new MapRibbon();

            // Call & Assert
            Assert.IsInstanceOf<Control>(ribbon.GetRibbonControl());
        }

        [Test]
        [RequiresSTA]
        public void Commands_NoCommandsAssigned_ReturnsEmptyCommandsCollection()
        {
            // Setup
            var ribbon = new MapRibbon();

            // Call & Assert
            CollectionAssert.IsEmpty(ribbon.Commands);
        }

        [Test]
        [RequiresSTA]
        public void Commands_CommandsAssigned_ReturnsAssignedCommands()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.Stub<IToolViewController>();
            mocks.ReplayAll();

            var toggleLegendViewCommand = new ToggleMapLegendViewCommand(new MapLegendController(toolViewController));

            var ribbon = new MapRibbon
            {
                ToggleLegendViewCommand = toggleLegendViewCommand
            };

            // Call
            var commands = ribbon.Commands.ToArray();

            // Assert
            CollectionAssert.AreEqual(new ICommand[] { toggleLegendViewCommand }, commands);
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void IsContextualTabVisible_Always_ReturnsFalse()
        {
            // Setup
            var ribbon = new MapRibbon();

            // Call & Assert
            Assert.IsFalse(ribbon.IsContextualTabVisible(null, null));
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_WithOrWithoutMap_UpdatesMapContextualVisiblity(bool mapVisible)
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.Stub<IMap>();

            mocks.ReplayAll();

            var ribbon = new MapRibbon();
            var contextualGroup = ribbon.GetRibbonControl().FindName("MapContextualGroup") as RibbonContextualTabGroup;

            // Precondition
            Assert.IsNotNull(contextualGroup, "Ribbon should have a map contextual group button");

            // Call
            ribbon.Map = mapVisible ? map : null;

            // Assert
            Assert.AreEqual(mapVisible ? Visibility.Visible : Visibility.Collapsed, contextualGroup.Visibility);
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ToggleLegendViewButton_OnClick_ExecutesToggleLegendViewCommand()
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Execute());

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                ToggleLegendViewCommand = command
            };
            var button = ribbon.GetRibbonControl().FindName("ToggleLegendViewButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle legend view button");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
    }
}
