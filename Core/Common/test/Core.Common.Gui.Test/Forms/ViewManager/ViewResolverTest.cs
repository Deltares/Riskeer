using System;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Plugin;
using Core.Common.Utils;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ViewManager
{
    [TestFixture]
    public class ViewResolverTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Bottom))
            {
                // Call
                var viewResolver = new ViewResolver(viewList, Enumerable.Empty<ViewInfo>(), dialogParent);

                // Assert
                Assert.IsInstanceOf<IViewResolver>(viewResolver);
                CollectionAssert.IsEmpty(viewResolver.DefaultViewTypes);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OpenViewForData_DataIsNull_ReturnFalse(bool forceShowDialog)
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestView>()
            };

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                // Call
                var result = viewResolver.OpenViewForData(null, forceShowDialog);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void OpenViewForData_DataHasSingleMatch_ReturnTrueAndAddToViewList()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var data = new object();
            const string viewData = "<I'm a piece of view data>";
            const string viewName = "<name of the view>";
            bool afterCreateCalled = false;
            bool activateViewCalled = false;

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<ViewResolverTest, TestView>(), 
                new ViewInfo<object, TestView>
                {
                    GetViewData = o =>
                    {
                        Assert.AreSame(data, o);
                        return viewData;
                    },
                    AfterCreate = (view, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(view);
                        Assert.AreSame(data, o);
                        afterCreateCalled = true;
                    },
                    GetViewName = (view, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(view);
                        Assert.AreSame(viewData, o);
                        return viewName;
                    },
                    OnActivateView = (view, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(view);
                        Assert.AreSame(data, o);
                        activateViewCalled = true;
                    }
                },
                new ViewInfo<int, TestView>()
            };

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);
                

                // Call
                var result = viewResolver.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(1, viewList.Count);
                var view = (TestView)viewList[0];
                Assert.AreSame(view, viewList.ActiveView);
                Assert.AreEqual(viewData, view.Data);
                Assert.AreEqual(viewName, view.Text);

                Assert.IsTrue(afterCreateCalled);
                Assert.IsTrue(activateViewCalled);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OpenViewForData_NoViewInfoRegistered_ReturnFalse(bool forceShowDialog)
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var viewInfos = new ViewInfo[0];

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                // Call
                var result = viewResolver.OpenViewForData(new object(), forceShowDialog);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void OpenViewForData_DataHasSingleMatchOnBaseType_ReturnTrueAndAddToViewList()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var data = new InheritedFromA();
            const string viewName = "<cool view name>";
            bool afterCreateCalled = false;
            bool activateViewCalled = false;

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<ViewResolverTest, TestView>(), 
                new ViewInfo<A, TestView>
                {
                    AfterCreate = (view, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(view);
                        Assert.AreSame(data, o);
                        afterCreateCalled = true;
                    },
                    GetViewName = (view, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(view);
                        Assert.AreSame(data, o);
                        return viewName;
                    },
                    OnActivateView = (view, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(view);
                        Assert.AreSame(data, o);
                        activateViewCalled = true;
                    }
                },
                new ViewInfo<int, TestView>()
            };

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);


                // Call
                var result = viewResolver.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(1, viewList.Count);
                var view = (TestView)viewList[0];
                Assert.AreSame(view, viewList.ActiveView);
                Assert.AreEqual(data, view.Data);
                Assert.AreEqual(viewName, view.Text);

                Assert.IsTrue(afterCreateCalled);
                Assert.IsTrue(activateViewCalled);
            }
        }

        [Test]
        public void OpenViewForData_DataHasMultipleMatchesOnType_ResolveToMostSpecializedAndReturnTrueAndAddToViewList()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var data = new InheritedFromA();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<InheritedFromA, TestViewDerivative>(), 
                new ViewInfo<A, TestView>()
            };

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                // Call
                var result = viewResolver.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(1, viewList.Count);
                var view = (TestViewDerivative)viewList[0];
                Assert.AreSame(view, viewList.ActiveView);
                Assert.AreEqual(data, view.Data);
                Assert.AreEqual(string.Empty, view.Text);
            }
        }

        [Test]
        public void OpenViewForData_ViewInfosForInheritedData_ResolveToMostSpecializedForDataAndReturnTrueAndAddToViewList()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var data = new A();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<InheritedFromA, TestViewDerivative>(), // Should not be matched as A does not inherit from InheritedFromA!
                new ViewInfo<A, TestView>()
            };

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                // Call
                var result = viewResolver.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(1, viewList.Count);
                var view = (TestView)viewList[0];
                Assert.AreSame(view, viewList.ActiveView);
                Assert.AreEqual(data, view.Data);
                Assert.AreEqual(string.Empty, view.Text);
            }
        }

        [Test]
        public void OpenViewForData_DataHasMultipleSingleMatches_UseAdditionalDataCheckAndReturnTrueAndAddToViewList()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var data = new object();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestViewDerivative>
                {
                    AdditionalDataCheck = o => true
                }, 
                new ViewInfo<object, TestView>
                {
                    AdditionalDataCheck = o => false
                }
            };

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                // Call
                var result = viewResolver.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(1, viewList.Count);
                var view = (TestViewDerivative)viewList[0];
                Assert.AreSame(view, viewList.ActiveView);
                Assert.AreSame(data, view.Data);
            }
        }

        [Test]
        public void OpenViewForData_DataHasMultipleMatchesAndRegisteredDefaultView_ReturnTrueAndAddDefaultViewToViewList()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var data = new object();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestViewDerivative>(), 
                new ViewInfo<object, TestView>()
            };

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);
                viewResolver.DefaultViewTypes[typeof(object)] = typeof(TestViewDerivative);

                // Call
                var result = viewResolver.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(1, viewList.Count);
                var view = (TestViewDerivative)viewList[0];
                Assert.AreSame(view, viewList.ActiveView);
                Assert.AreEqual(data, view.Data);
                Assert.AreEqual(string.Empty, view.Text);
            }
        }

        [Test]
        public void OpenViewForData_OpenSameViewForTwoDifferentDataInstances_OpenTwoViews()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Left))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<object, TestView>()
                };

                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                var data1 = new object();
                var data2 = new object();

                // Call
                viewResolver.OpenViewForData(data1);
                viewResolver.OpenViewForData(data2);

                // Assert
                CollectionAssert.AllItemsAreInstancesOfType(viewList, typeof(TestView));
                Assert.AreEqual(2, viewList.Count,
                    "Should have opened 2 views for 2 different data instances.");

                Assert.AreSame(data1, viewList[0].Data);
                Assert.AreSame(data2, viewList[1].Data);
            }
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void OpeningViewForDataTwiceShouldOnlySetActiveView()
        {
            // Setup
            var url = new WebLink("Deltares", new Uri("http://www.deltares.nl"));

            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<WebLink, HtmlPageView>()
                };

                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                // Open view...
                viewResolver.OpenViewForData(url);

                var originallyActiveView = viewList.ActiveView;

                // ... then make it inactive.
                viewList.ActiveView = null;

                // Call
                viewResolver.OpenViewForData(url);

                // Assert
                Assert.AreEqual(1, viewList.Count);
                Assert.AreSame(originallyActiveView, viewList[0]);
                Assert.AreEqual(originallyActiveView, viewList.ActiveView);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfosFor_NoViewInfosRegistered_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Bottom))
            {
                var viewResolver = new ViewResolver(viewList, Enumerable.Empty<ViewInfo>(), dialogParent);

                var data = new object();

                // Call
                var matchedViewInfos = viewResolver.GetViewInfosFor(data);

                // Assert
                CollectionAssert.IsEmpty(matchedViewInfos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfosFor_SingleDirectMatch_ReturnSingleMatchingViewInfo()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Bottom))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<A, TestView>(),
                    new ViewInfo<int, TestView>(),
                    new ViewInfo<string, TestView>()
                };

                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                var data = default(int);

                // Call
                var matchedViewInfos = viewResolver.GetViewInfosFor(data).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[]{viewInfos[1]}, matchedViewInfos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfosFor_ViewInfosWithInheritance_ReturnMatchesBasedOnInheritaceDataType()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Bottom))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<A, TestView>(),
                    new ViewInfo<InheritedFromA, TestView>(),
                    new ViewInfo<object, TestView>()
                };

                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                var data = new A();

                // Call
                var matchedViewInfos = viewResolver.GetViewInfosFor(data).ToArray();

                // Assert
                var expected = new[]
                {
                    viewInfos[0],
                    viewInfos[2]
                };
                CollectionAssert.AreEqual(expected, matchedViewInfos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfosFor_ViewInfosWithAdditionalDataCheck_ReturnMatchesWithAdditionalDataCheckTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Bottom))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<A, TestView>
                    {
                        AdditionalDataCheck = a => true
                    },
                    new ViewInfo<InheritedFromA, TestView>(),
                    new ViewInfo<object, TestView>
                    {
                        AdditionalDataCheck = o => false
                    }
                };

                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                var data = new InheritedFromA();

                // Call
                var matchedViewInfos = viewResolver.GetViewInfosFor(data).ToArray();

                // Assert
                var expected = new[]
                {
                    viewInfos[0],
                    viewInfos[1]
                };
                CollectionAssert.AreEqual(expected, matchedViewInfos);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewName_ViewNotRegistered_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var view = new TestView())
            using (var viewList = new ViewList(dockingManager, ViewLocation.Floating))
            {
                var viewResolver = new ViewResolver(viewList, Enumerable.Empty<ViewInfo>(), dialogParent);

                // Call
                var name = viewResolver.GetViewName(view);

                // Assert
                Assert.AreEqual(string.Empty, name);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewName_RegisteredViewHasNoGetViewNameImplementation_ReturnNull()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var view = new TestView())
            using (var viewList = new ViewList(dockingManager, ViewLocation.Floating))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<object, TestView>()
                };
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                // Call
                var name = viewResolver.GetViewName(view);

                // Assert
                Assert.IsNull(name);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewName_RegisteredViewWithGetViewNameImplementation_ReturnName()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            var data = new A();
            const string viewName = "<view name>";

            using (var view = new TestView { Data = data })
            using (var viewList = new ViewList(dockingManager, ViewLocation.Floating))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<object, TestView>
                    {
                        GetViewName = (testView, o) =>
                        {
                            Assert.AreSame(view, testView);
                            Assert.AreSame(data, o);
                            return viewName;
                        }
                    }
                };
                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);

                // Call
                var name = viewResolver.GetViewName(view);

                // Assert
                Assert.AreEqual(viewName, name);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseAllViewsFor_DataDoesNotCorrespondToOpenedViews_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<A, TestView>(),
                    new ViewInfo<InheritedFromA, TestViewDerivative>(),
                };

                var data1 = new A();
                var data2 = new InheritedFromA();
                var unusedViewData = new object();

                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);
                viewResolver.OpenViewForData(data1);
                viewResolver.OpenViewForData(data2);

                // Precondition
                Assert.AreEqual(2, viewList.Count);

                // Call
                viewResolver.CloseAllViewsFor(unusedViewData);

                // Assert
                Assert.AreEqual(2, viewList.Count,
                    "No elements should have been removed.");
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseAllViewsFor_DataDoesNotCorrespondToOpenedViewsButCloseForDataReturnsTrue_RemoveViews()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var data1 = new A();
                var data2 = new InheritedFromA();
                var unusedViewData = new object();

                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<A, TestView>
                    {
                        CloseForData = (view, o) =>
                        {
                            Assert.IsInstanceOf<TestView>(view);
                            Assert.AreSame(data1, view.Data);
                            Assert.AreSame(unusedViewData, o);
                            return true;
                        }
                    },
                    new ViewInfo<InheritedFromA, TestViewDerivative>
                    {
                        CloseForData = (view, o) =>
                        {
                            Assert.IsInstanceOf<TestView>(view);
                            Assert.AreSame(data2, view.Data);
                            Assert.AreSame(unusedViewData, o);
                            return true;
                        }
                    }
                };

                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);
                viewResolver.OpenViewForData(data1);
                viewResolver.OpenViewForData(data2);

                // Precondition
                Assert.AreEqual(2, viewList.Count);

                // Call
                viewResolver.CloseAllViewsFor(unusedViewData);

                // Assert
                Assert.AreEqual(0, viewList.Count,
                    "Views should be closed due to CloseForData returning true.");
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseAllViewsFor_DataIsNull_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<A, TestView>(),
                    new ViewInfo<InheritedFromA, TestViewDerivative>(),
                };

                var data1 = new A();
                var data2 = new InheritedFromA();

                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);
                viewResolver.OpenViewForData(data1);
                viewResolver.OpenViewForData(data2);

                // Precondition
                Assert.AreEqual(2, viewList.Count);

                // Call
                viewResolver.CloseAllViewsFor(null);

                // Assert
                Assert.AreEqual(2, viewList.Count,
                    "No elements should have been removed.");
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseAllViewsFor_DataCorrespondsToOpenedView_RemoveThatView()
        {
            // Setup
            var mocks = new MockRepository();
            var dockingManager = mocks.Stub<IDockingManager>();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            using (var viewList = new ViewList(dockingManager, ViewLocation.Document))
            {
                var viewInfos = new ViewInfo[]
                {
                    new ViewInfo<A, TestView>(),
                    new ViewInfo<InheritedFromA, TestViewDerivative>(),
                };

                var data1 = new A();
                var data2 = new InheritedFromA();

                var viewResolver = new ViewResolver(viewList, viewInfos, dialogParent);
                viewResolver.OpenViewForData(data1);
                viewResolver.OpenViewForData(data2);

                // Precondition
                Assert.AreEqual(2, viewList.Count);

                // Call
                viewResolver.CloseAllViewsFor(data1);

                // Assert
                Assert.AreEqual(1, viewList.Count);
                Assert.IsInstanceOf<TestViewDerivative>(viewList[0]);
                Assert.AreSame(data2, viewList[0].Data);
            }
            mocks.VerifyAll();
        }

        private class A
        {
            
        }

        private class InheritedFromA : A
        {
            
        }
    }
}