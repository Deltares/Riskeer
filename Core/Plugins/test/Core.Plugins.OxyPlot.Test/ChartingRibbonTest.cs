using System.Linq;
using System.Windows;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Legend;
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
            Assert.IsNotNull(button, "Ribbon should have an open chart view button.");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateItems_Always_IsCheckedEqualToCommandChecked(bool commandChecked)
        {
            // Setup
            var mocks = new MockRepository();
            var command = mocks.StrictMock<ICommand>();
            command.Expect(c => c.Checked).Return(commandChecked);

            mocks.ReplayAll();

            var ribbon = new ChartingRibbon
            {
                ToggleLegendViewCommand = command
            };
            
            var button = ribbon.GetRibbonControl().FindName("ToggleLegendViewButton") as ToggleButton;
            
            // Precondition
            Assert.IsNotNull(button, "Ribbon should have an open chart view button.");

            // Call
            ribbon.ValidateItems();

            // Assert
            Assert.AreEqual(commandChecked, button.IsChecked);
            mocks.VerifyAll();
        }
    }
}