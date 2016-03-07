using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Components.Gis.Forms;
using Core.Plugins.DotSpatial.Commands;
using Core.Plugins.DotSpatial.Legend;
using Fluent;
using NUnit.Framework;
using Rhino.Mocks;
using Button = Fluent.Button;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using Control = System.Windows.Controls.Control;
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
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

            mocks.ReplayAll();

            var toggleLegendViewCommand = new ToggleMapLegendViewCommand(new MapLegendController(toolViewController, contextMenuBuilderProvider, parentWindow));

            var ribbon = new MapRibbon
            {
                ToggleLegendViewCommand = toggleLegendViewCommand
            };

            // Call
            var commands = ribbon.Commands.ToArray();

            // Assert
            CollectionAssert.AreEqual(new ICommand[]
            {
                toggleLegendViewCommand
            }, commands);
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
            var map = mocks.Stub<IMapControl>();

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
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_ToggleLegendViewIsCheckedEqualToCommandChecked(bool commandChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Checked).Return(commandChecked);

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                ToggleLegendViewCommand = command,
            };

            var toggleLegendViewButton = ribbon.GetRibbonControl().FindName("ToggleLegendViewButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(toggleLegendViewButton, "Ribbon should have a toggle legend view button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(commandChecked, toggleLegendViewButton.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_TogglePanningButtonIsCheckedEqualToPanningChecked(bool panningChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Stub(m => m.IsPanningEnabled).Return(panningChecked);
            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };

            var button = ribbon.GetRibbonControl().FindName("TogglePanningButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle panning button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(panningChecked, button.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_ToggleRectangleZoomingButtonIsCheckedEqualToRectangleZoomChecked(bool rectangleZoomChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Stub(m => m.IsRectangleZoomingEnabled).Return(rectangleZoomChecked);
            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };

            var button = ribbon.GetRibbonControl().FindName("ToggleRectangleZoomingButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle rectangle zooming button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(rectangleZoomChecked, button.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_ToggleMouseCoordinatesButtonIsCheckedEqualToMouseCoordinatesChecked(bool mouseCoordinatesChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Stub(m => m.IsMouseCoordinatesVisible).Return(mouseCoordinatesChecked);
            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };

            var button = ribbon.GetRibbonControl().FindName("ToggleMouseCoordinatesButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle mouse coordinate visibility button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(mouseCoordinatesChecked, button.IsChecked);
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

        [Test]
        [RequiresSTA]
        public void TogglePanning_OnClick_TogglePanning()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Expect(c => c.TogglePanning());

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };
            var button = ribbon.GetRibbonControl().FindName("TogglePanningButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle panning button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ToggleRectangleZooming_OnClick_ToggleRectangleZooming()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Expect(c => c.ToggleRectangleZooming());

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };
            var button = ribbon.GetRibbonControl().FindName("ToggleRectangleZoomingButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle rectangle zooming button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
        
        [Test]
        [RequiresSTA]
        public void ZoomToAll_OnClick_ZoomToAll()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Expect(c => c.ZoomToAllVisibleLayers());

            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };
            var button = ribbon.GetRibbonControl().FindName("ZoomToAllButton") as Button;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a zoom to all button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void ToggleMouseCoordinatesVisibility_OnClick_ToggleMouseCoordinates()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.DynamicMock<IMapControl>();
            map.Expect(c => c.ToggleMouseCoordinatesVisibility());
            mocks.ReplayAll();

            var ribbon = new MapRibbon
            {
                Map = map
            };
            var button = ribbon.GetRibbonControl().FindName("ToggleMouseCoordinatesButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle mouse coordinates button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
    }
}