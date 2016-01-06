using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Plugin;
using Core.Common.Test.TestObjects;
using Core.Common.Utils;
using Core.Common.Utils.Events;

using NUnit.Framework;

namespace Core.Common.Test.Gui
{
    [TestFixture]
    public class ViewListTest
    {
        [Test]
        public void CollectionChangedEvent()
        {
            var dockingManager = new TestDockingManager();

            var toolWindowViewManager = new ViewList(dockingManager, ViewLocation.Left);

            var view = new ToolWindowTestControl
            {
                Text = @"text"
            };

            var senders = new List<object>();
            var actions = new List<NotifyCollectionChangeAction>();
            var items = new List<object>();
            var indexes = new List<int>();
            toolWindowViewManager.CollectionChanged += (s, e) =>
            {
                senders.Add(s);
                actions.Add(e.Action);
                items.Add(e.Item);
                indexes.Add(e.Index);
            };

            toolWindowViewManager.Add(view);
            toolWindowViewManager.Remove(view);

            // asserts
            Assert.AreEqual(new[]
            {
                toolWindowViewManager,
                toolWindowViewManager
            }, senders);
            Assert.AreEqual(new[]
            {
                NotifyCollectionChangeAction.Add,
                NotifyCollectionChangeAction.Remove
            }, actions);
            Assert.AreEqual(new[]
            {
                view,
                view
            }, items);
            Assert.AreEqual(new[]
            {
                0,
                0
            }, indexes);
        }

        [Test]
        public void OpeningAViewForAObjectShouldUseNewView()
        {
            var viewList = new ViewList(new TestDockingManager(), ViewLocation.Left);
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
        }

        [Test]
        public void ReplacingAViewGivesAddAndRemoveEvents()
        {
            // get a view manager
            var viewList = new ViewList(new TestDockingManager(), ViewLocation.Left);

            // add a context aware view
            var view = new TestView();

            viewList.Add(view);

            // now replace the view with another one
            var newView = new TestView();

            var senders = new List<object>();
            var actions = new List<NotifyCollectionChangeAction>();
            var items = new List<IView>();

            viewList.CollectionChanged += (s, e) =>
            {
                senders.Add(s);
                actions.Add(e.Action);
                items.Add((IView) e.Item);
            };

            // action! replace a view
            viewList[0] = newView;

            // assert the right collection changes occures ..for now it should be remove / add...no replace yet
            Assert.AreEqual(new[]
            {
                viewList,
                viewList
            }, senders);
            Assert.AreEqual(new[]
            {
                NotifyCollectionChangeAction.Remove,
                NotifyCollectionChangeAction.Add
            }, actions);
            Assert.AreEqual(new[]
            {
                view,
                newView
            }, items);
        }

        [Test]
        public void OpeningViewForDataTwiceShouldOnlySetActiveView()
        {
            var url = new Url("Deltares", "www.deltares.nl");

            var viewList = new ViewList(new TestDockingManager(), ViewLocation.Document);
            var viewResolver = new ViewResolver(viewList, new ViewInfo[]
            {
                new ViewInfo<Url, HtmlPageView>()
            }, null);

            viewResolver.OpenViewForData(url);

            var activeView = viewList.ActiveView;

            viewList.ActiveView = null;
            viewResolver.OpenViewForData(url);

            Assert.AreEqual(activeView, viewList.ActiveView);
        }

        [Test]
        public void GetViewsForDataWithCustomData()
        {
            var viewList = new ViewList(new TestDockingManager(), ViewLocation.Left);
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
        }

        [Test]
        public void OpenViewForItemShouldNotReturnSubClassedViews()
        {
            var viewList = new ViewList(new TestDockingManager(), ViewLocation.Left);
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
        }
    }
}