using System;
using System.Linq;

using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Test.Properties;
using Core.Common.TestUtil;
using Core.Common.Utils.Events;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    [TestFixture]
    public class ViewListTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();
            
            // Call
            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                // Assert
                CollectionAssert.IsEmpty(viewList);
                Assert.AreEqual(0, viewList.Count);
                Assert.IsNull(viewList.ActiveView);
                CollectionAssert.IsEmpty(viewList.AllViews);
                Assert.IsFalse(viewList.IgnoreActivation);
                Assert.IsFalse(viewList.IsReadOnly);
                Assert.IsNull(viewList.UpdateViewNameAction);
            }
            
            mocks.VerifyAll();
        }

        [Test]
        public void ActiveView_SetViewNotInViewList_MakeActiveViewAndFireActiveViewChangingEventsAndLogThatViewIsNotInCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            using (var view = new ToolWindowTestControl())
            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var changingEventCount = 0;
                viewList.ActiveViewChanging += (sender, args) =>
                {
                    changingEventCount++;
                    Assert.AreSame(viewList, sender);
                    Assert.AreSame(view, args.View);
                    Assert.IsNull(args.OldView);
                };
                var changedEventCount = 0;
                viewList.ActiveViewChanged += (sender, args) =>
                {
                    changingEventCount++;
                    Assert.AreSame(viewList, sender);
                    Assert.AreSame(view, args.View);
                    Assert.IsNull(args.OldView);
                };

                // Precondition
                CollectionAssert.DoesNotContain(viewList, view);

                // Call
                Action call = () => viewList.ActiveView = view;

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, "Documentvenster is niet gevonden tussen alle mogelijke documentvensters.", 1);

                Assert.AreEqual(1, changingEventCount);
                Assert.AreEqual(0, changedEventCount);

                Assert.AreSame(view, viewList.ActiveView);
                Assert.IsFalse(view.Focused);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ActiveView_SetViewInList_SwitchActiveViewWithChangeEvents()
        {
            // Setup
            using (var view1 = new ToolWindowTestControl())
            using (var view2 = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, ViewLocation.Floating))
                {
                    viewList.Add(view1);
                    viewList.Add(view2);

                    var changingEventCount = 0;
                    viewList.ActiveViewChanging += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view1, args.View);
                        Assert.AreSame(view2, args.OldView);
                        changingEventCount++;
                    };

                    var changedEventCount = 0;
                    viewList.ActiveViewChanged += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view1, args.View);
                        Assert.AreSame(view2, args.OldView);
                        changedEventCount++;
                    };

                    // Precondition
                    Assert.False(viewList.IgnoreActivation);
                    Assert.AreSame(view2, viewList.ActiveView);

                    // Call
                    viewList.ActiveView = view1;

                    // Assert
                    Assert.AreSame(view1, viewList.ActiveView);

                    Assert.AreEqual(1, changingEventCount);
                    Assert.AreEqual(1, changedEventCount);
                }
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ActiveView_IgnoreActivation_DoNotChangeActiveView()
        {
            // Setup
            using (var view1 = new ToolWindowTestControl())
            using (var view2 = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, ViewLocation.Floating))
                {
                    viewList.Add(view1);
                    viewList.Add(view2);

                    viewList.IgnoreActivation = true;

                    viewList.ActiveViewChanging += (sender, args) =>
                    {
                        Assert.Fail("Should ignore view activation, therefore no events should be fired.");
                    };

                    viewList.ActiveViewChanged += (sender, args) =>
                    {
                        Assert.Fail("Should ignore view activation, therefore no events should be fired.");
                    };

                    // Precondition
                    Assert.AreSame(view2, viewList.ActiveView);

                    // Call
                    viewList.ActiveView = view1;

                    // Assert
                    Assert.AreSame(view2, viewList.ActiveView);
                }
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ActiveView_ActivatingDisposedView_DoNotChangeActiveView()
        {
            // Setup
            using (var view1 = new ToolWindowTestControl())
            using (var view2 = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, ViewLocation.Floating))
                {
                    viewList.Add(view1);
                    viewList.Add(view2);

                    view1.Dispose();

                    viewList.ActiveViewChanging += (sender, args) =>
                    {
                        Assert.Fail("Should ignore view activation, therefore no events should be fired.");
                    };

                    viewList.ActiveViewChanged += (sender, args) =>
                    {
                        Assert.Fail("Should ignore view activation, therefore no events should be fired.");
                    };

                    // Precondition
                    Assert.AreSame(view2, viewList.ActiveView);
                    Assert.IsTrue(view1.IsDisposed);

                    // Call
                    viewList.ActiveView = view1;

                    // Assert
                    Assert.AreSame(view2, viewList.ActiveView);
                }
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(ViewLocation.Top)]
        [TestCase(ViewLocation.Bottom)]
        [TestCase(ViewLocation.Document)]
        [TestCase(ViewLocation.Floating)]
        [TestCase(ViewLocation.Left)]
        [TestCase(ViewLocation.Right)]
        public void Add_ViewInAnyLocation_AppendToListAndMakeActiveWithEventsAndFireCollectionChangedEvent(ViewLocation location)
        {
            // Setup
            using (var view = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                dockingManager.Stub(dm => dm.ViewBarClosing += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewBarClosing -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.Dispose());
                dockingManager.Expect(dm => dm.Add(view, location));
                dockingManager.Expect(dm => dm.ActivateView(view));
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
                {
                    var updateViewNameActionCallCount = 0;
                    viewList.UpdateViewNameAction = v => updateViewNameActionCallCount++;

                    var changingEventCount = 0;
                    viewList.ActiveViewChanging += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view, args.View);
                        Assert.IsNull(args.OldView);
                        changingEventCount++;
                    };
                    var changedEventCount = 0;
                    viewList.ActiveViewChanged += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view, args.View);
                        Assert.IsNull(args.OldView);
                        changedEventCount++;
                    };
                    var collectionChangedEventCount = 0;
                    viewList.CollectionChanged += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreEqual(NotifyCollectionChangeAction.Add, args.Action);
                        Assert.AreEqual(0, args.Index);
                        Assert.AreSame(view, args.Item);
                        Assert.AreEqual(-1, args.OldIndex);
                        Assert.IsNull(args.OldItem);
                        Assert.IsFalse(args.Cancel);
                        collectionChangedEventCount++;
                    };

                    // Call
                    viewList.Add(view, location);

                    // Assert
                    Assert.AreEqual(1, viewList.Count);
                    CollectionAssert.AreEqual(new[] { view }, viewList);
                    CollectionAssert.AreEqual(new[] { view }, viewList.AllViews);
                    Assert.AreSame(view, viewList[0]);

                    Assert.AreEqual(1, updateViewNameActionCallCount);

                    Assert.AreEqual(1, changingEventCount);
                    Assert.AreEqual(1, changedEventCount);
                    Assert.AreEqual(1, collectionChangedEventCount);

                    Assert.AreSame(view, viewList.ActiveView);
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void Add_ViewWithViewLocation_FireAddCollectionChange(int alreadyAddedViews)
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var view = mocks.Stub<IView>();
            var viewsToPreadd = new[]
            {
                mocks.Stub<IView>(),
                mocks.Stub<IView>()
            };
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Bottom))
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
        [TestCase(ViewLocation.Top)]
        [TestCase(ViewLocation.Bottom)]
        [TestCase(ViewLocation.Document)]
        [TestCase(ViewLocation.Floating)]
        [TestCase(ViewLocation.Left)]
        [TestCase(ViewLocation.Right)]
        public void Add_ViewInAlreadyAddedButNotActive_MakeActiveWithEvents(ViewLocation location)
        {
            // Setup
            using (var view1 = new ToolWindowTestControl())
            using (var view2 = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                dockingManager.Stub(dm => dm.ViewBarClosing += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewBarClosing -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.Dispose());
                dockingManager.Expect(dm => dm.Add(view2, location));
                dockingManager.Expect(dm => dm.ActivateView(view2)).Repeat.Twice();
                dockingManager.Expect(dm => dm.Add(view1, ViewLocation.Document));
                dockingManager.Expect(dm => dm.ActivateView(view1));
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
                {
                    viewList.Add(view2, location);
                    viewList.Add(view1, ViewLocation.Document);

                    var updateViewNameActionCallCount = 0;
                    viewList.UpdateViewNameAction = v => updateViewNameActionCallCount++;

                    var changingEventCount = 0;
                    viewList.ActiveViewChanging += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view2, args.View);
                        Assert.AreSame(view1, args.OldView);
                        changingEventCount++;
                    };
                    var changedEventCount = 0;
                    viewList.ActiveViewChanged += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view2, args.View);
                        Assert.AreSame(view1, args.OldView);
                        changedEventCount++;
                    };
                    viewList.CollectionChanged += (sender, args) =>
                    {
                        Assert.Fail("View is already added so no collection changes should be sent.");
                    };

                    // Call
                    viewList.Add(view2, location);

                    // Assert
                    Assert.AreEqual(2, viewList.Count);
                    CollectionAssert.AreEqual(new[] { view2, view1 }, viewList);
                    CollectionAssert.AreEqual(new[] { view2, view1 }, viewList.AllViews);
                    Assert.AreSame(view2, viewList[0]);
                    Assert.AreSame(view1, viewList[1]);

                    Assert.AreEqual(0, updateViewNameActionCallCount,
                        "View is already added, therefore no name update should occur.");

                    Assert.AreEqual(1, changingEventCount);
                    Assert.AreEqual(1, changedEventCount);

                    Assert.AreSame(view2, viewList.ActiveView);
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(ViewLocation.Top)]
        [TestCase(ViewLocation.Bottom)]
        [TestCase(ViewLocation.Document)]
        [TestCase(ViewLocation.Floating)]
        [TestCase(ViewLocation.Left)]
        [TestCase(ViewLocation.Right)]
        public void Add_ViewAlreadyAddedAndAlreadyActive_ActivateView(ViewLocation location)
        {
            // Setup
            using (var view = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                dockingManager.Stub(dm => dm.ViewBarClosing += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewBarClosing -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.Dispose());
                dockingManager.Expect(dm => dm.Add(view, ViewLocation.Document));
                dockingManager.Expect(dm => dm.ActivateView(view));
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
                {
                    viewList.Add(view);

                    viewList.UpdateViewNameAction = v => Assert.Fail("No 'UpdateViewNameAction' should be called.");

                    viewList.ActiveViewChanging += (sender, args) =>
                    {
                        Assert.Fail("View is already active, therefore no changes should be broadcasted.");
                    };
                    viewList.ActiveViewChanged += (sender, args) =>
                    {
                        Assert.Fail("View is already active, therefore no changes should be broadcasted.");
                    };
                    viewList.CollectionChanged += (sender, args) =>
                    {
                        Assert.Fail("No collection change event should be fired.");
                    };

                    // Call
                    viewList.Add(view, location);

                    // Assert
                    Assert.AreEqual(1, viewList.Count);
                    CollectionAssert.AreEqual(new[] { view }, viewList);
                    CollectionAssert.AreEqual(new[] { view }, viewList.AllViews);
                    Assert.AreSame(view, viewList[0]);

                    Assert.AreSame(view, viewList.ActiveView);
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void Add_ViewWithoutViewLocation_FireAddCollectionChange(int alreadyAddedViews)
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var view = mocks.Stub<IView>();
            var viewsToPreadd = new[]
            {
                mocks.Stub<IView>(),
                mocks.Stub<IView>()
            };
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
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
                Assert.AreEqual(alreadyAddedViews + 1, viewList.Count);
                Assert.AreSame(view, viewList.ActiveView);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(ViewLocation.Top)]
        [TestCase(ViewLocation.Bottom)]
        [TestCase(ViewLocation.Document)]
        [TestCase(ViewLocation.Floating)]
        [TestCase(ViewLocation.Left)]
        [TestCase(ViewLocation.Right)]
        public void Add_WithoutViewLocation_AppendToAtDefaultLocationListAndMakeActiveWithEventsAndFireCollectionChangedEvent(ViewLocation defaultLocation)
        {
            // Setup
            using (var view1 = new ToolWindowTestControl())
            using (var view2 = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                dockingManager.Stub(dm => dm.ViewBarClosing += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewBarClosing -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.Dispose());
                dockingManager.Stub(dm => dm.Add(view1, defaultLocation));
                dockingManager.Stub(dm => dm.ActivateView(view1));
                dockingManager.Expect(dm => dm.Add(view2, defaultLocation));
                dockingManager.Expect(dm => dm.ActivateView(view2));
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, defaultLocation))
                {
                    viewList.Add(view1);

                    var updateViewNameActionCallCount = 0;
                    viewList.UpdateViewNameAction = v => updateViewNameActionCallCount++;

                    var changingEventCount = 0;
                    viewList.ActiveViewChanging += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view2, args.View);
                        Assert.AreSame(view1, args.OldView);
                        changingEventCount++;
                    };
                    var changedEventCount = 0;
                    viewList.ActiveViewChanged += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view2, args.View);
                        Assert.AreSame(view1, args.OldView);
                        changedEventCount++;
                    };
                    var collectionChangedEventCount = 0;
                    viewList.CollectionChanged += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreEqual(NotifyCollectionChangeAction.Add, args.Action);
                        Assert.AreEqual(1, args.Index);
                        Assert.AreSame(view2, args.Item);
                        Assert.AreEqual(-1, args.OldIndex);
                        Assert.IsNull(args.OldItem);
                        Assert.IsFalse(args.Cancel);
                        collectionChangedEventCount++;
                    };

                    // Call
                    viewList.Add(view2);

                    // Assert
                    Assert.AreEqual(2, viewList.Count);
                    CollectionAssert.AreEqual(new[] { view1, view2 }, viewList);
                    CollectionAssert.AreEqual(new[] { view1, view2 }, viewList.AllViews);
                    Assert.AreSame(view2, viewList[1]);

                    Assert.AreEqual(1, updateViewNameActionCallCount);

                    Assert.AreEqual(1, changingEventCount);
                    Assert.AreEqual(1, changedEventCount);
                    Assert.AreEqual(1, collectionChangedEventCount);

                    Assert.AreSame(view2, viewList.ActiveView);
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        public void Add_WithoutLocationAndListConstructedWithoutDefaultLocation_ThrowInvalidOperationException()
        {
            // Setup
            using (var view = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, null))
                {
                    // Call
                    TestDelegate call = () => viewList.Add(view);

                    // Assert
                    var message = Assert.Throws<InvalidOperationException>(call).Message;
                    Assert.AreEqual("Er is geen standaardlocatie opgegeven. Kan geen documentvenster toevoegen zonder de locatie parameter.", message);
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        public void Contains_ViewNotInList_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            using (var view = new ToolWindowTestControl())
            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                // Call
                var result = viewList.Contains(view);

                // Assert
                Assert.False(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Contains_ViewNotInList_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            using (var view = new ToolWindowTestControl())
            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                viewList.Add(view);

                // Call
                var result = viewList.Contains(view);

                // Assert
                Assert.True(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetEnumerator_NoViewsAdded_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                // Call
                var result = viewList.GetEnumerator();

                // Assert
                Assert.False(result.MoveNext());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetEnumerator_ListWithViews_ReturnEnumeratorAcrossAllViews()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            using (var view1 = new ToolWindowTestControl())
            using (var view2 = new ToolWindowTestControl())
            using (var view3 = new ToolWindowTestControl())
            using (var view4 = new ToolWindowTestControl())
            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                viewList.Add(view1);
                viewList.Add(view2);
                viewList.Add(view3);
                viewList.Add(view4);

                // Call
                var result = viewList.GetEnumerator();

                // Assert
                Assert.True(result.MoveNext());
                Assert.AreSame(view1, result.Current);
                Assert.True(result.MoveNext());
                Assert.AreSame(view2, result.Current);
                Assert.True(result.MoveNext());
                Assert.AreSame(view3, result.Current);
                Assert.True(result.MoveNext());
                Assert.AreSame(view4, result.Current);
                Assert.False(result.MoveNext());
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(ViewLocation.Top)]
        [TestCase(ViewLocation.Bottom)]
        [TestCase(ViewLocation.Document)]
        [TestCase(ViewLocation.Floating)]
        [TestCase(ViewLocation.Left)]
        [TestCase(ViewLocation.Right)]
        public void Insert_WithoutLocation_InsertAtDefaultLocationAndMakeActiveWithEventsAndFireCollectionChange(ViewLocation defaultLocation)
        {
            // Setup
            using (var view1 = new ToolWindowTestControl())
            using (var view2 = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                dockingManager.Stub(dm => dm.ViewBarClosing += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewBarClosing -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.Dispose());
                dockingManager.Stub(dm => dm.Add(view1, defaultLocation));
                dockingManager.Stub(dm => dm.ActivateView(view1));
                dockingManager.Expect(dm => dm.Add(view2, defaultLocation));
                dockingManager.Expect(dm => dm.ActivateView(view2));
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, defaultLocation))
                {
                    viewList.Add(view1);

                    var updateViewNameActionCallCount = 0;
                    viewList.UpdateViewNameAction = v => updateViewNameActionCallCount++;

                    var changingEventCount = 0;
                    viewList.ActiveViewChanging += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view2, args.View);
                        Assert.AreSame(view1, args.OldView);
                        changingEventCount++;
                    };
                    var changedEventCount = 0;
                    viewList.ActiveViewChanged += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreSame(view2, args.View);
                        Assert.AreSame(view1, args.OldView);
                        changedEventCount++;
                    };
                    var collectionChangedEventCount = 0;
                    viewList.CollectionChanged += (sender, args) =>
                    {
                        Assert.AreSame(viewList, sender);
                        Assert.AreEqual(NotifyCollectionChangeAction.Add, args.Action);
                        Assert.AreEqual(0, args.Index);
                        Assert.AreSame(view2, args.Item);
                        Assert.AreEqual(-1, args.OldIndex);
                        Assert.IsNull(args.OldItem);
                        Assert.IsFalse(args.Cancel);
                        collectionChangedEventCount++;
                    };

                    // Call
                    viewList.Insert(0, view2);

                    // Assert
                    Assert.AreEqual(2, viewList.Count);
                    CollectionAssert.AreEqual(new[] { view2, view1 }, viewList);
                    CollectionAssert.AreEqual(new[] { view2, view1 }, viewList.AllViews);
                    Assert.AreSame(view2, viewList[0]);

                    Assert.AreEqual(1, updateViewNameActionCallCount);

                    Assert.AreEqual(1, changingEventCount);
                    Assert.AreEqual(1, changedEventCount);
                    Assert.AreEqual(1, collectionChangedEventCount);

                    Assert.AreSame(view2, viewList.ActiveView);
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        public void Insert_ListConstructedWithoutDefaultLocation_ThrowInvalidOperationException()
        {
            // Setup
            using (var view = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, null))
                {
                    // Call
                    TestDelegate call = () => viewList.Insert(0, view);

                    // Assert
                    var message = Assert.Throws<InvalidOperationException>(call).Message;
                    Assert.AreEqual("Er is geen standaardlocatie opgegeven. Kan geen documentvenster toevoegen zonder de locatie parameter.", message);
                }

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void Insert_ViewWithoutViewLocation_FireAddCollectionChange(int alreadyAddedViews)
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var view = mocks.Stub<IView>();
            var viewsToPreadd = new[]
            {
                mocks.Stub<IView>(),
                mocks.Stub<IView>()
            };
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
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
        public void Clear_WithoutViews_FireResetCollectionChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
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
            mocks.VerifyAll();
        }

        [Test]
        public void Clear_WithViews_FireResetCollectionChangedAndRemoveCollectionChangePerView()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var view1 = mocks.Stub<IView>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                viewList.Add(view1);

                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.AreSame(viewList, sender);

                    if (args.Action == NotifyCollectionChangeAction.Remove)
                    {
                        Assert.AreEqual(0, args.Index);
                        Assert.AreEqual(-1, args.OldIndex);
                        Assert.AreSame(view1, args.Item);
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
            var dockingManager = mocks.Stub<IDockingManager>();
            var view = mocks.Stub<IView>();
            var viewToKeep = mocks.Stub<IView>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
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
        public void Remove_RemovingAddedView_FireRemoveCollectionChange()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var view = mocks.Stub<IView>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
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
        public void Remove_RemovingViewNotInList_ReturnFalseAndNoCollectionChangedEvents()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var view1 = mocks.Stub<IView>();
            var view2 = mocks.Stub<IView>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                viewList.Add(view1);

                viewList.CollectionChanged += (sender, args) =>
                {
                    Assert.Fail("Shouldn't fire event for removing view not part of list.");
                };

                // Call
                var removeResult = viewList.Remove(view2);

                // Assert
                Assert.False(removeResult);
                Assert.AreSame(view1, viewList[0]);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveAt_RemovingAddedView_FireRemoveCollectionChange()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var view = mocks.Stub<IView>();
            var anotherView = mocks.Stub<IView>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
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

        [Test]
        public void IndexOf_ViewNotInList_ReturnMinusOne()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            using (var view = new ToolWindowTestControl())
            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                // Call
                var index = viewList.IndexOf(view);

                // Assert
                Assert.AreEqual(-1, index);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void IndexOf_ViewInList_ReturnMinusOne(int expectedViewIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            using (var view1 = new ToolWindowTestControl())
            using (var view2 = new ToolWindowTestControl())
            using (var view3 = new ToolWindowTestControl())
            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var views = new[]
                {
                    view1,
                    view2,
                    view3
                };
                foreach (var view in views)
                {
                    viewList.Add(view);
                }

                // Call
                var index = viewList.IndexOf(views[expectedViewIndex]);

                // Assert
                Assert.AreEqual(expectedViewIndex, index);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void SetToolTip_ViewInList_SetTooltip()
        {
            // Setup
            const string expectedText = "<Some tooltip text>";
            using (var view = new ToolWindowTestControl())
            {
                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                dockingManager.Stub(dm => dm.ViewBarClosing += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewBarClosing -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.Dispose());
                dockingManager.Expect(dm => dm.SetToolTip(view, expectedText));
                dockingManager.Stub(dm => dm.Add(view, ViewLocation.Bottom));
                dockingManager.Stub(dm => dm.ActivateView(view));
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, ViewLocation.Bottom))
                {
                    viewList.Add(view);

                    // Call
                    viewList.SetTooltip(view, expectedText);
                }
                // Assert
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CopyTo_ListWithViews_CopyElementsToTargetArray()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            mocks.ReplayAll();

            using (var view1 = new ToolWindowTestControl())
            using (var view2 = new ToolWindowTestControl())
            using (var view3 = new ToolWindowTestControl())
            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var views = new[]
                {
                    view1,
                    view2,
                    view3
                };
                foreach (var view in views)
                {
                    viewList.Add(view);
                }

                var targetArray = new IView[5];

                // Call
                viewList.CopyTo(targetArray, 2);

                // Assert
                CollectionAssert.AreEqual(Enumerable.Repeat<IView>(null, 2).Concat(views), targetArray);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void SetImage_ViewInList_DockingManagerSetImage()
        {
            // Setup
            using (var view = new ToolWindowTestControl())
            {
                var image = Resources.abacus;

                var mocks = new MockRepository();
                var dockingManager = mocks.Stub<IDockingManager>();
                dockingManager.Stub(dm => dm.ViewBarClosing += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated += null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewBarClosing -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.ViewActivated -= null).IgnoreArguments();
                dockingManager.Stub(dm => dm.Dispose());
                dockingManager.Stub(dm => dm.Add(view, ViewLocation.Document));
                dockingManager.Stub(dm => dm.ActivateView(view));
                dockingManager.Expect(dm => dm.SetImage(view, image));
                mocks.ReplayAll();

                using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
                {
                    viewList.Add(view);

                    // Call
                    viewList.SetImage(view, image);

                    
                }
                // Assert
                mocks.VerifyAll(); // Expect calls on IDockingManager
            }
        }
    }
}