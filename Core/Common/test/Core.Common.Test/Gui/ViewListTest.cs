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

using Rhino.Mocks;

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

            toolWindowViewManager.Add(view);

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

            toolWindowViewManager.Remove(view);

            // asserts
            Assert.AreEqual(new[]
            {
                toolWindowViewManager
            }, senders);
            Assert.AreEqual(new[]
            {
                NotifyCollectionChangeAction.Remove
            }, actions);
            Assert.AreEqual(new[]
            {
                view
            }, items);
            Assert.AreEqual(new[]
            {
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
        [RequiresSTA]
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

        [Test]
        public void Clear_WithoutViews_FireResetCollectionChanged()
        {
            // Setup
            using (var viewList = new ViewList(new TestDockingManager(), ViewLocation.Document))
            {
                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.AreSame(viewList, sender);
                    Assert.AreEqual(NotifyCollectionChangeAction.Reset, args.Action);
                    Assert.AreEqual(-1, args.Index);
                    Assert.AreEqual(-1, args.OldIndex);
                    Assert.IsNull(args.Item);
                    Assert.IsNull(args.OldItem);
                    Assert.IsFalse(args.Cancel);
                };

                // Call
                viewList.Clear();

                // Assert
                Assert.IsNull(viewList.ActiveView);
                CollectionAssert.IsEmpty(viewList);
            }
        }

        [Test]
        public void Clear_WithViews_FireResetCollectionChangedAndRemoveCollectionChangePerView()
        {
            // Setup
            var mocks = new MockRepository();
            var view = mocks.Stub<IView>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(new TestDockingManager(), ViewLocation.Document))
            {
                viewList.Add(view);

                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.AreSame(viewList, sender);

                    if (args.Action == NotifyCollectionChangeAction.Remove)
                    {
                        Assert.AreEqual(0, args.Index);
                        Assert.AreEqual(-1, args.OldIndex);
                        Assert.AreSame(view, args.Item);
                        Assert.IsNull(args.OldItem);
                        Assert.IsFalse(args.Cancel);
                    }
                    else if (args.Action == NotifyCollectionChangeAction.Reset)
                    {
                        Assert.AreEqual(-1, args.Index);
                        Assert.AreEqual(-1, args.OldIndex);
                        Assert.IsNull(args.Item);
                        Assert.IsNull(args.OldItem);
                        Assert.IsFalse(args.Cancel);
                    }
                    else
                    {
                        Assert.Fail("Unexpected collection change event.");
                    }
                };

                // Call
                viewList.Clear();

                // Assert
                Assert.IsNull(viewList.ActiveView);
                CollectionAssert.IsEmpty(viewList);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Clear_WithViewsAndKeepingOne_FireResetCollectionChangedAndRemoveCollectionChangePerRemovedView()
        {
            // Setup
            var mocks = new MockRepository();
            var view = mocks.Stub<IView>();
            var viewToKeep = mocks.Stub<IView>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(new TestDockingManager(), ViewLocation.Document))
            {
                viewList.Add(viewToKeep);
                viewList.Add(view);

                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.AreSame(viewList, sender);

                    if (args.Action == NotifyCollectionChangeAction.Remove)
                    {
                        Assert.AreEqual(1, args.Index);
                        Assert.AreEqual(-1, args.OldIndex);
                        Assert.AreSame(view, args.Item);
                        Assert.IsNull(args.OldItem);
                        Assert.IsFalse(args.Cancel);
                    }
                    else if (args.Action == NotifyCollectionChangeAction.Reset)
                    {
                        Assert.AreEqual(-1, args.Index);
                        Assert.AreEqual(-1, args.OldIndex);
                        Assert.IsNull(args.Item);
                        Assert.IsNull(args.OldItem);
                        Assert.IsFalse(args.Cancel);
                    }
                    else
                    {
                        Assert.Fail("Unexpected collection change event.");
                    }
                };

                // Call
                viewList.Clear(viewToKeep);

                // Assert
                Assert.AreSame(viewToKeep, viewList.ActiveView);
                Assert.AreEqual(1, viewList.Count);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void Add_ViewWithoutViewLocation_FireAddCollectionChange(int alreadyAddedViews)
        {
            // Setup
            var mocks = new MockRepository();
            var view = mocks.Stub<IView>();
            var viewsToPreadd = new[]
            {
                mocks.Stub<IView>(),
                mocks.Stub<IView>()
            };
            mocks.ReplayAll();

            using (var viewList = new ViewList(new TestDockingManager(), ViewLocation.Document))
            {
                for (int i = 0; i < alreadyAddedViews; i++)
                {
                    viewList.Add(viewsToPreadd[i]);
                }

                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.AreSame(viewList, sender);
                    Assert.AreEqual(NotifyCollectionChangeAction.Add, args.Action);
                    Assert.AreEqual(alreadyAddedViews, args.Index);
                    Assert.AreEqual(-1, args.OldIndex);
                    Assert.AreSame(view, args.Item);
                    Assert.IsNull(args.OldItem);
                    Assert.IsFalse(args.Cancel);
                };
                // Call
                viewList.Add(view);

                // Assert
                Assert.AreEqual(alreadyAddedViews+1, viewList.Count);
                Assert.AreSame(view, viewList.ActiveView);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void Add_ViewWithViewLocation_FireAddCollectionChange(int alreadyAddedViews)
        {
            // Setup
            var mocks = new MockRepository();
            var view = mocks.Stub<IView>();
            var viewsToPreadd = new[]
            {
                mocks.Stub<IView>(),
                mocks.Stub<IView>()
            };
            mocks.ReplayAll();

            using (var viewList = new ViewList(new TestDockingManager(), ViewLocation.Bottom))
            {
                for (int i = 0; i < alreadyAddedViews; i++)
                {
                    viewList.Add(viewsToPreadd[i]);
                }

                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.AreSame(viewList, sender);
                    Assert.AreEqual(NotifyCollectionChangeAction.Add, args.Action);
                    Assert.AreEqual(alreadyAddedViews, args.Index);
                    Assert.AreEqual(-1, args.OldIndex);
                    Assert.AreSame(view, args.Item);
                    Assert.IsNull(args.OldItem);
                    Assert.IsFalse(args.Cancel);
                };
                // Call
                viewList.Add(view, ViewLocation.Document);

                // Assert
                Assert.AreEqual(alreadyAddedViews + 1, viewList.Count);
                Assert.AreSame(view, viewList.ActiveView);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void Insert_ViewWithoutViewLocation_FireAddCollectionChange(int alreadyAddedViews)
        {
            // Setup
            var mocks = new MockRepository();
            var view = mocks.Stub<IView>();
            var viewsToPreadd = new[]
            {
                mocks.Stub<IView>(),
                mocks.Stub<IView>()
            };
            mocks.ReplayAll();

            using (var viewList = new ViewList(new TestDockingManager(), ViewLocation.Document))
            {
                for (int i = 0; i < alreadyAddedViews; i++)
                {
                    viewList.Add(viewsToPreadd[i]);
                }

                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.AreSame(viewList, sender);
                    Assert.AreEqual(NotifyCollectionChangeAction.Add, args.Action);
                    Assert.AreEqual(0, args.Index);
                    Assert.AreEqual(-1, args.OldIndex);
                    Assert.AreSame(view, args.Item);
                    Assert.IsNull(args.OldItem);
                    Assert.IsFalse(args.Cancel);
                };
                // Call
                viewList.Insert(0, view);

                // Assert
                Assert.AreEqual(alreadyAddedViews + 1, viewList.Count);
                Assert.AreSame(view, viewList.ActiveView);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Remove_RemovingAddedView_FireRemoveCollectionChange()
        {
            // Setup
            var mocks = new MockRepository();
            var view = mocks.Stub<IView>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(new TestDockingManager(), ViewLocation.Document))
            {
                viewList.Add(view);

                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.AreSame(viewList, sender);
                    Assert.AreEqual(NotifyCollectionChangeAction.Remove, args.Action);
                    Assert.AreEqual(0, args.Index);
                    Assert.AreEqual(-1, args.OldIndex);
                    Assert.AreSame(view, args.Item);
                    Assert.IsNull(args.OldItem);
                    Assert.IsFalse(args.Cancel);
                };

                // Call
                var removeResult = viewList.Remove(view);

                // Assert
                Assert.IsTrue(removeResult);
                CollectionAssert.IsEmpty(viewList);
                Assert.IsNull(viewList.ActiveView);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveAt_RemovingAddedView_FireRemoveCollectionChange()
        {
            // Setup
            var mocks = new MockRepository();
            var view = mocks.Stub<IView>();
            var anotherView = mocks.Stub<IView>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(new TestDockingManager(), ViewLocation.Document))
            {
                viewList.Add(anotherView);
                viewList.Add(view);

                var index = 1;
                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.AreSame(viewList, sender);
                    Assert.AreEqual(NotifyCollectionChangeAction.Remove, args.Action);
                    Assert.AreEqual(index, args.Index);
                    Assert.AreEqual(-1, args.OldIndex);
                    Assert.AreSame(view, args.Item);
                    Assert.IsNull(args.OldItem);
                    Assert.IsFalse(args.Cancel);
                };

                // Call
                viewList.RemoveAt(index);

                // Assert
                Assert.AreEqual(1, viewList.Count);
                Assert.AreEqual(anotherView, viewList.ActiveView);
            }
            mocks.VerifyAll();
        }
    }
}