using System;

using Core.Common.Gui.Selection;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Selection
{
    [TestFixture]
    public class SelectedItemChangedEventArgsTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var selectedObject = new object();

            // Call
            var eventArgs = new SelectedItemChangedEventArgs(selectedObject);

            // Assert
            Assert.IsInstanceOf<EventArgs>(eventArgs);
            Assert.AreSame(selectedObject, eventArgs.Item);
        }
    }
}