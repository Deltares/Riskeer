using System.Windows;
using System.Windows.Controls;
using Core.Components.DotSpatial;
using Fluent;
using NUnit.Framework;
using Rhino.Mocks;

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
    }
}
