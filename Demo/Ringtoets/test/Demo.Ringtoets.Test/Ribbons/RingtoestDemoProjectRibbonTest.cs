using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Components.DotSpatial.Data;
using Demo.Ringtoets.Commands;
using Demo.Ringtoets.Ribbons;
using NUnit.Framework;
using Rhino.Mocks;

namespace Demo.Ringtoets.Test.Ribbons
{
    [TestFixture]
    public class RingtoestDemoProjectRibbonTest
    {
        [Test]
        [RequiresSTA]
        public void DefaultContructor_CreatesCommands()
        {
            // Call
            var ribbon = new RingtoetsDemoProjectRibbon();
            var commands = ribbon.Commands.ToArray();

            // Assert
            CollectionAssert.IsNotEmpty(commands);
            CollectionAssert.AllItemsAreInstancesOfType(commands, typeof(ICommand));
            CollectionAssert.AllItemsAreUnique(commands);
        }

        [Test]
        [RequiresSTA]
        public void DefaultConstructor_Always_CreatesControl()
        {
            // Call
            var ribbon = new RingtoetsDemoProjectRibbon();

            // Assert
            Assert.IsNotNull(ribbon);
            Assert.IsInstanceOf<Control>(ribbon.GetRibbonControl());
        }

        [Test]
        [RequiresSTA]
        public void IsContextualTabVisible_Always_ReturnsFalse()
        {
            // Call
            var ribbon = new RingtoetsDemoProjectRibbon();

            // Assert
            Assert.IsFalse(ribbon.IsContextualTabVisible(null, null));
        }

        [Test]
        [RequiresSTA]
        public void OpenMapViewButton_OnClick_OpensView()
        {
            // Setup
            var mocks = new MockRepository();
            var viewResolver = mocks.StrictMock<IViewResolver>();
            viewResolver.Expect(vr => vr.OpenViewForData(Arg<MapData>.Matches(md => md.IsValid()), Arg<bool>.Matches(b => b == false))).Return(true);

            var gui = mocks.StrictMock<IGui>();
            gui.Expect(g => g.DocumentViewsResolver).Return(viewResolver);

            mocks.ReplayAll();

            var ribbon = new RingtoetsDemoProjectRibbon();
            ribbon.Commands.OfType<OpenMapViewCommand>().First().Gui = gui;

            var button = ribbon.GetRibbonControl().FindName("OpenMapViewButton") as Button;

            // Precondition
            Assert.IsNotNull((object) button, "Ribbon should have an open map view button");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
    }
}
