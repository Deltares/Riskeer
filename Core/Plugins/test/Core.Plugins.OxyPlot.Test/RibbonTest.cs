using System.Linq;
using Core.Common.Controls.Commands;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test
{
    [TestFixture]
    public class RibbonTest
    {
        [Test]
        [RequiresSTA]
        public void Commands_NoCommandsAssigned_ReturnsNullForCommands()
        {
            // Setup
            var ribbon = new ChartingRibbon();

            // Call
            var commands = ribbon.Commands.ToArray();

            // Assert
            CollectionAssert.AreEqual(Enumerable.Repeat<ICommand>(null,2),commands);
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
                    ToggleLegendViewCommand = toggleLegendViewCommand
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
    }
}