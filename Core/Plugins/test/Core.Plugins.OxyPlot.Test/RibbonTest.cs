using System.Linq;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test
{
    [TestFixture]
    public class RibbonTest
    {
        [Test]
        [RequiresSTA]
        public void Commands_Always_ReturnsOpenChartViewCommand()
        {
            // Setup
            var ribbon = new Ribbon();

            // Call
            var commands = ribbon.Commands.ToArray();

            // Assert
            Assert.AreEqual(1, commands.Count());
            Assert.IsInstanceOf<OpenChartViewCommand>(commands.First());
        }

        [Test]
        [RequiresSTA]
        public void RibbonControl_Always_ReturnsRibbonControl()
        {
            // Setup
            var ribbon = new Ribbon();

            // Call & Assert
            Assert.IsInstanceOf<System.Windows.Controls.Control>(ribbon.GetRibbonControl());
        }
        
        [Test]
        [RequiresSTA]
        public void IsContextualTabVisible_Always_ReturnsFalse()
        {
            // Setup
            var ribbon = new Ribbon();

            // Call & Assert
            Assert.IsFalse(ribbon.IsContextualTabVisible(null,null));
        }
    }
}