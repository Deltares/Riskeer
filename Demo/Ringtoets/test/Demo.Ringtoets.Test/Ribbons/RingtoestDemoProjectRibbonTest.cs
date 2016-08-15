using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewHost;
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
        public void DefaultConstructor_Always_CreatesControl()
        {
            // Setup
            var mocks = new MockRepository();
            var projectOwner = mocks.Stub<IProjectOwner>();
            var viewController = mocks.Stub<IViewController>();
            mocks.ReplayAll();

            // Call
            var ribbon = new RingtoetsDemoProjectRibbon(projectOwner, viewController);

            // Assert
            Assert.IsNotNull(ribbon);
            Assert.IsInstanceOf<Control>(ribbon.GetRibbonControl());
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void OpenChartViewButton_OnClick_ExecutesOpenChartViewCommand()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            documentViewController.Expect(vr => vr.OpenViewForData(null)).IgnoreArguments().Return(true);

            var projectOwner = mocks.Stub<IProjectOwner>();
            var viewController = mocks.Stub<IViewController>();
            viewController.Expect(vc => vc.DocumentViewController).Return(documentViewController);

            mocks.ReplayAll();

            var ribbon = new RingtoetsDemoProjectRibbon(projectOwner, viewController);
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
        public void OpenMapViewButton_OnClick_OpensView()
        {
            // Setup
            var mocks = new MockRepository();
            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            documentViewController.Expect(vr => vr.OpenViewForData(null)).IgnoreArguments().Return(true);

            var projectOwner = mocks.Stub<IProjectOwner>();
            var viewController = mocks.Stub<IViewController>();
            viewController.Expect(vc => vc.DocumentViewController).Return(documentViewController);

            mocks.ReplayAll();

            var ribbon = new RingtoetsDemoProjectRibbon(projectOwner, viewController);

            var button = ribbon.GetRibbonControl().FindName("OpenMapViewButton") as Button;

            // Precondition
            Assert.IsNotNull(button, "Ribbon should have an open map view button");

            // Call
            button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            // Assert
            mocks.VerifyAll();
        }
    }
}