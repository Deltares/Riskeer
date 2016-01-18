using System.Linq;
using System.Windows;
using Core.Components.Charting;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Legend;
using Fluent;
using NUnit.Framework;
using Rhino.Mocks;
using Button = Fluent.Button;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using ICommand = Core.Common.Controls.Commands.ICommand;
using ToggleButton = Fluent.ToggleButton;

namespace Core.Plugins.OxyPlot.Test
{
    [TestFixture]
    public class ChartingRibbonTest
    {
        [Test]
        [RequiresSTA]
        public void Commands_NoCommandsAssigned_ReturnsEmptyCommandsCollection()
        {
            // Setup
            var ribbon = new ChartingRibbon();

            // Call & Assert
            CollectionAssert.IsEmpty(ribbon.Commands);
        }

        [Test]
        [RequiresSTA]
        public void Commands_CommandsAssigned_ReturnsAssignedCommands()
        {
            // Setup
            using(var oxyPlotGuiPlugin = new OxyPlotGuiPlugin()) {
                var openChartViewCommand = new OpenChartViewCommand();
                var toggleLegendViewCommand = new ToggleLegendViewCommand(new LegendController(oxyPlotGuiPlugin));
                var ribbon = new ChartingRibbon
                {
                    OpenChartViewCommand = openChartViewCommand,
                    ToggleLegendViewCommand = toggleLegendViewCommand,
                };

                // Call
                var commands = ribbon.Commands.ToArray();

                // Assert
                CollectionAssert.AreEqual(new ICommand[]{openChartViewCommand, toggleLegendViewCommand}, commands);
            }
        }

        [Test]
        [RequiresSTA]
        public void DefaultConstructor_Always_CreatesControl()
        {
            // Setup
            var ribbon = new ChartingRibbon();

            // Call & Assert
            Assert.IsInstanceOf<System.Windows.Controls.Control>(ribbon.GetRibbonControl());
        }
        
        [Test]
        [RequiresSTA]
        public void IsContextualTabVisible_Always_ReturnsFalse()
        {
            // Setup
            var ribbon = new ChartingRibbon();

            // Call & Assert
            Assert.IsFalse(ribbon.IsContextualTabVisible(null,null));
        }

        [Test]
        [RequiresSTA]
        public void OpenChartViewButton_OnClick_ExecutesOpenChartViewCommand()
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Execute());

            mocks.ReplayAll();

            var ribbon = new ChartingRibbon
            {
                OpenChartViewCommand = command
            };
            var button = ribbon.GetRibbonControl().FindName("OpenChartViewButton") as Button;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have an open chart view button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
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

            var ribbon = new ChartingRibbon
            {
                ToggleLegendViewCommand = command
            };
            var button = ribbon.GetRibbonControl().FindName("ToggleLegendViewButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have a toggle legend view button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void Chart_Always_AttachesToChartAndUpdatesChartingCommands(bool buttonChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var chart = mocks.StrictMock<IChart>();
            var ribbon = new ChartingRibbon();

            chart.Expect(c => c.IsPanning).Return(buttonChecked);
            chart.Expect(c => c.IsRectangleZooming).Return(buttonChecked);
            chart.Expect(c => c.Attach(ribbon));

            mocks.ReplayAll();

            var togglePanningButton = ribbon.GetRibbonControl().FindName("TogglePanningButton") as ToggleButton;
            var toggleRectangleZoomingButton = ribbon.GetRibbonControl().FindName("ToggleRectangleZoomingButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(togglePanningButton, "Ribbon should have a toggle panning button.");
            Assert.IsNotNull(toggleRectangleZoomingButton, "Ribbon should have a rectangle zoom panning button.");

            // Call
            ribbon.Chart = chart;

            // Assert
            Assert.AreEqual(buttonChecked, togglePanningButton.IsChecked);
            Assert.AreEqual(buttonChecked, toggleRectangleZoomingButton.IsChecked);
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void Chart_WithOrWithoutChart_UpdatesChartingContextualVisibility(bool chartVisible)
        {
            // Setup
            var mocks = new MockRepository();
            var chart = mocks.Stub<IChart>();

            mocks.ReplayAll();

            var ribbon = new ChartingRibbon();

            var contextualGroup = ribbon.GetRibbonControl().FindName("ChartingContextualGroup") as RibbonContextualTabGroup;

            // Precondition
            Assert.IsNotNull(contextualGroup, "Ribbon should have a charting contextual group button.");

            // Call
            ribbon.Chart = chartVisible ? chart : null;

            // Assert
            Assert.AreEqual(chartVisible ? Visibility.Visible : Visibility.Collapsed, contextualGroup.Visibility);
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

            var ribbon = new ChartingRibbon
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
        public void ValidateItems_Always_TogglePanningIsCheckedEqualToChartIsPanning(bool buttonChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var chart = mocks.StrictMock<IChart>();

            var ribbon = new ChartingRibbon();

            chart.Expect(c => c.IsPanning).Return(buttonChecked).Repeat.Twice();
            chart.Expect(c => c.IsRectangleZooming).Return(buttonChecked).Repeat.Twice(); ;
            chart.Expect(c => c.Attach(ribbon));

            mocks.ReplayAll();

            ribbon.Chart = chart;

            var togglePanningButton = ribbon.GetRibbonControl().FindName("TogglePanningButton") as ToggleButton;
            var toggleRectangleZoomingButton = ribbon.GetRibbonControl().FindName("ToggleRectangleZoomingButton") as ToggleButton;

            // Precondition
            Assert.IsNotNull(togglePanningButton, "Ribbon should have a toggle panning button.");
            Assert.IsNotNull(toggleRectangleZoomingButton, "Ribbon should have a rectangle zoom panning button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(buttonChecked, togglePanningButton.IsChecked);
            Assert.AreEqual(buttonChecked, toggleRectangleZoomingButton.IsChecked);
            mocks.VerifyAll();
        }
    }
}