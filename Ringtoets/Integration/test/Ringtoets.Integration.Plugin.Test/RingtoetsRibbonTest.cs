using System;

using Core.Common.Gui.Forms;

using Fluent;

using NUnit.Framework;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class RingtoetsRibbonTest
    {
        [Test]
        [STAThread] // Due to creating fluent ribbon
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var ribbon = new RingtoetsRibbon();

            // Assert
            Assert.IsInstanceOf<IRibbonCommandHandler>(ribbon);
            CollectionAssert.IsEmpty(ribbon.Commands);
        }

        [Test]
        [STAThread] // Due to creating fluent Ribbon
        public void GetRibbonControl_Always_ReturnRibbon()
        {
            // Setup
            var ribbon = new RingtoetsRibbon();

            // Call
            var control = ribbon.GetRibbonControl();

            // Assert
            Assert.IsInstanceOf<Ribbon>(control);
        }

        [Test]
        [STAThread] // Due to creating fluent Ribbon
        public void IsContextualTabVisible_Always_ReturnFalse()
        {
            // Setup
            var ribbon = new RingtoetsRibbon();

            // Call
            var isVisible = ribbon.IsContextualTabVisible(null, null);

            // Assert
            Assert.IsFalse(isVisible);
        }
    }
}