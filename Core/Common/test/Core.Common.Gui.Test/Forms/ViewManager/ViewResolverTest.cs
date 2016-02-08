using System;
using System.Linq;

using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Plugin;
using Core.Common.Test.TestObjects;
using Core.Common.Utils;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    public class ViewResolverTest
    {
        [Test]
        public void OpeningAViewForAObjectShouldUseNewView()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            var viewList = new ViewList(dockingManager, ViewLocation.Left);
            var viewResolver = new ViewResolver(viewList, new ViewInfo[]
            {
                new ViewInfo<object, TestView>()
            }, null);

            var testObject = new object();
            viewResolver.OpenViewForData(testObject);
            var viewForTestObject = viewList[0];
            Assert.AreEqual(testObject, viewForTestObject.Data);

            var otherObject = new object();
            viewResolver.OpenViewForData(otherObject);

            // extra views. the first view now renders the other object
            Assert.AreEqual(2, viewList.Count);

            // no change in original view
            Assert.AreEqual(testObject, viewForTestObject.Data);

            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void OpeningViewForDataTwiceShouldOnlySetActiveView()
        {
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            var url = new WebLink("Deltares", new Uri("http://www.deltares.nl"));

            var viewList = new ViewList(dockingManager, ViewLocation.Document);
            var viewResolver = new ViewResolver(viewList, new ViewInfo[]
            {
                new ViewInfo<WebLink, HtmlPageView>()
            }, null);

            viewResolver.OpenViewForData(url);

            var activeView = viewList.ActiveView;

            viewList.ActiveView = null;
            viewResolver.OpenViewForData(url);

            Assert.AreEqual(activeView, viewList.ActiveView);

            mocks.VerifyAll();
        }

        [Test]
        public void GetViewsForDataWithCustomData()
        {
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            var viewList = new ViewList(dockingManager, ViewLocation.Left);
            var viewResolver = new ViewResolver(viewList, new ViewInfo[]
            {
                new ViewInfo<TestWrapper, string, TestView>
                {
                    GetViewData = o => o.RealData
                }
            }, null);

            var someDataObject = "some data object";
            var wrapper = new TestWrapper
            {
                RealData = someDataObject
            };

            var view = new TestView
            {
                Data = someDataObject
            };

            viewList.Add(view);

            var returnedViews = viewResolver.GetViewsForData(wrapper); // <-- must trigger IViewProvider.IsViewForData()

            Assert.AreEqual(1, returnedViews.Count, "number of compatible views");

            Assert.AreSame(view, returnedViews.First(), "correct view is returned back");

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForItemShouldNotReturnSubClassedViews()
        {
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            var viewList = new ViewList(dockingManager, ViewLocation.Left);
            var viewResolver = new ViewResolver(viewList, new ViewInfo[]
            {
                new ViewInfo<object, TestView>
                {
                    Description = "Object view"
                },
                new ViewInfo<string, TestViewDerivative>
                {
                    Description = "String view"
                } // string inherits from object 
            }, null);

            var data = "string data";
            viewResolver.OpenViewForData(data);

            var view = viewList.ActiveView;

            Assert.AreEqual(data, view.Data);
            Assert.AreEqual(typeof(TestViewDerivative), view.GetType());

            mocks.VerifyAll();
        }
    }
}