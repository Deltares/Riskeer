// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ViewHost
{
    [TestFixture]
    public class DocumentViewControllerTest : NUnitFormTestWithHiddenDesktop
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();

            mocks.ReplayAll();

            // Call
            using (var documentViewController = new DocumentViewController(viewHost,
                                                                           Enumerable.Empty<ViewInfo>(),
                                                                           dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<IDocumentViewController>(documentViewController);
                CollectionAssert.IsEmpty(documentViewController.DefaultViewTypes);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OpenViewForData_DataIsNull_ReturnFalse(bool forceShowDialog)
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();

            mocks.ReplayAll();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                bool result = documentViewController.OpenViewForData(null, forceShowDialog);

                // Assert
                Assert.IsFalse(result);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OpenViewForData_NoViewInfoRegistered_ReturnFalse(bool forceShowDialog)
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();

            mocks.ReplayAll();

            var viewInfos = new ViewInfo[0];

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                bool result = documentViewController.OpenViewForData(new object(), forceShowDialog);

                // Assert
                Assert.IsFalse(result);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_DataHasSingleMatch_ReturnTrueAndAddToViewHost()
        {
            // Setup
            TestView view = null;
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                view = invocation.Arguments[0] as TestView;
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var data = new object();
            const string viewData = "<I'm a piece of view data>";
            const string viewName = "<name of the view>";
            var afterCreateCalled = false;

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<DocumentViewControllerTest, TestView>(),
                new ViewInfo<object, TestView>
                {
                    GetViewData = o =>
                    {
                        Assert.AreSame(data, o);
                        return viewData;
                    },
                    AfterCreate = (v, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(v);
                        Assert.AreSame(data, o);
                        afterCreateCalled = true;
                    },
                    GetViewName = (v, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(v);
                        Assert.AreSame(data, o);
                        return viewName;
                    }
                },
                new ViewInfo<int, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                bool result = documentViewController.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(viewData, view.Data);
                Assert.AreEqual(viewName, view.Text);
                Assert.IsTrue(afterCreateCalled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_DataHasSingleMatchOnBaseType_ReturnTrueAndAddToViewHost()
        {
            // Setup
            TestView view = null;
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                view = invocation.Arguments[0] as TestView;
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var data = new InheritedFromA();
            const string viewName = "<cool view name>";
            var afterCreateCalled = false;

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<DocumentViewControllerTest, TestView>(),
                new ViewInfo<A, TestView>
                {
                    AfterCreate = (v, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(v);
                        Assert.AreSame(data, o);
                        afterCreateCalled = true;
                    },
                    GetViewName = (v, o) =>
                    {
                        Assert.IsInstanceOf<TestView>(v);
                        Assert.AreSame(data, o);
                        return viewName;
                    }
                },
                new ViewInfo<int, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                bool result = documentViewController.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(data, view.Data);
                Assert.AreEqual(viewName, view.Text);
                Assert.IsTrue(afterCreateCalled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_DataHasMultipleMatchesOnType_ResolveToMostSpecializedAndReturnTrueAndAddToViewHost()
        {
            // Setup
            TestViewDerivative view = null;
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestViewDerivative>.Is.NotNull)).WhenCalled(invocation =>
            {
                view = invocation.Arguments[0] as TestViewDerivative;
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var data = new InheritedFromA();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<InheritedFromA, TestViewDerivative>(),
                new ViewInfo<A, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                bool result = documentViewController.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(data, view.Data);
                Assert.IsEmpty(view.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_ViewInfosForInheritedData_ResolveToMostSpecializedForDataAndReturnTrueAndAddToViewHost()
        {
            // Setup
            TestView view = null;
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                view = invocation.Arguments[0] as TestView;
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var data = new A();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<InheritedFromA, TestViewDerivative>(), // Should not be matched as A does not inherit from InheritedFromA!
                new ViewInfo<A, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                bool result = documentViewController.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(data, view.Data);
                Assert.IsEmpty(view.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_DataHasMultipleSingleMatches_UseAdditionalDataCheckAndReturnTrueAndAddToViewHost()
        {
            // Setup
            TestViewDerivative view = null;
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestViewDerivative>.Is.NotNull)).WhenCalled(invocation =>
            {
                view = invocation.Arguments[0] as TestViewDerivative;
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();

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

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                bool result = documentViewController.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(data, view.Data);
                Assert.IsEmpty(view.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_ClickCancelInOpenedDialog_ReturnFalseAndNoViewAddedToViewHost()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);

            mocks.ReplayAll();

            var data = new object();
            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestViewDerivative>(),
                new ViewInfo<object, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                DialogBoxHandler = (name, wnd) =>
                {
                    var buttonCancel = new ControlTester("buttonCancel");

                    buttonCancel.Click();
                };

                // Call
                bool result = documentViewController.OpenViewForData(data);

                // Assert
                Assert.IsFalse(result);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_ClickOkInOpenedDialog_ReturnTrueAndViewAddedToViewHost()
        {
            // Setup
            TestView view = null;
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                view = invocation.Arguments[0] as TestView;
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var data = new object();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestViewDerivative>(),
                new ViewInfo<object, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                DialogBoxHandler = (name, wnd) =>
                {
                    var buttonOk = new ControlTester("buttonOk");

                    buttonOk.Click();
                };

                // Call
                bool result = documentViewController.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(data, view.Data);
                Assert.IsEmpty(view.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_MarkAsDefaultViewAndClickOkInOpenedDialog_ReturnTrueViewAddedToViewHostAndDefaultViewTypesUpdated()
        {
            // Setup
            TestView view = null;
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                view = invocation.Arguments[0] as TestView;
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var data = new object();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestViewDerivative>(),
                new ViewInfo<object, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                DialogBoxHandler = (name, wnd) =>
                {
                    var buttonOk = new ControlTester("buttonOk");
                    var checkbox = new CheckBoxTester("checkBoxDefault");

                    checkbox.Check();
                    buttonOk.Click();
                };

                // Precondition
                Assert.IsFalse(documentViewController.DefaultViewTypes.ContainsKey(typeof(object)));

                // Call
                bool result = documentViewController.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(data, view.Data);
                Assert.IsEmpty(view.Text);
                Assert.IsTrue(documentViewController.DefaultViewTypes.ContainsKey(typeof(object)));
                Assert.AreEqual(documentViewController.DefaultViewTypes[typeof(object)], typeof(TestView));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_SelectDifferentDefaultViewAndClickOkInOpenedDialog_ReturnTrueViewAddedToViewHostAndDefaultViewTypesUpdated()
        {
            // Setup
            TestView view = null;
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                view = invocation.Arguments[0] as TestView;
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var data = new object();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestViewDerivative>(),
                new ViewInfo<object, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                documentViewController.DefaultViewTypes[typeof(object)] = typeof(TestViewDerivative);

                DialogBoxHandler = (name, wnd) =>
                {
                    var buttonOk = new ControlTester("buttonOk");
                    var listBox = new ListBoxTester("listBox");
                    var checkBox = new CheckBoxTester("checkBoxDefault");

                    listBox.SetSelected(0, true);
                    checkBox.Check();
                    buttonOk.Click();
                };

                // Call
                bool result = documentViewController.OpenViewForData(data, true);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(data, view.Data);
                Assert.IsEmpty(view.Text);
                Assert.IsTrue(documentViewController.DefaultViewTypes.ContainsKey(typeof(object)));
                Assert.AreEqual(documentViewController.DefaultViewTypes[typeof(object)], typeof(TestView));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_DataHasMultipleMatchesAndRegisteredDefaultView_ReturnTrueAndAddDefaultViewToViewHost()
        {
            // Setup
            TestViewDerivative view = null;
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestViewDerivative>.Is.NotNull)).WhenCalled(invocation =>
            {
                view = invocation.Arguments[0] as TestViewDerivative;
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var data = new object();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestViewDerivative>(),
                new ViewInfo<object, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                documentViewController.DefaultViewTypes[typeof(object)] = typeof(TestViewDerivative);

                // Call
                bool result = documentViewController.OpenViewForData(data);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(data, view.Data);
                Assert.IsEmpty(view.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_OpenSameViewForTwoDifferentDataInstances_OpenTwoViews()
        {
            // Setup
            var data1 = new object();
            var data2 = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new IView[0]);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Matches(c => c.Data == data1)));
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Matches(c => c.Data == data2)));
            viewHost.Stub(vh => vh.SetImage(null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                documentViewController.OpenViewForData(data1);
                documentViewController.OpenViewForData(data2);
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void OpenViewForData_OpeningViewForAlreadyOpenedButInactiveView_ActivateDocumentView()
        {
            // Setup
            var viewList = new List<IView>();
            var data = new object();
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(viewList);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation => viewList.Add(invocation.Arguments[0] as TestView));
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();
            viewHost.Expect(vh => vh.BringToFront(Arg<TestView>.Matches(c => c == viewList.First())));

            mocks.ReplayAll();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<object, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Open view
                documentViewController.OpenViewForData(data);

                // Call
                documentViewController.OpenViewForData(data);
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfosFor_NoViewInfosRegistered_ReturnEmpty()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();

            mocks.ReplayAll();

            using (var documentViewController = new DocumentViewController(viewHost, Enumerable.Empty<ViewInfo>(), dialogParent))
            {
                var data = new object();

                // Call
                IEnumerable<ViewInfo> matchedViewInfos = documentViewController.GetViewInfosFor(data);

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
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();

            mocks.ReplayAll();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<A, TestView>(),
                new ViewInfo<int, TestView>(),
                new ViewInfo<string, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                const int data = default(int);

                // Call
                ViewInfo[] matchedViewInfos = documentViewController.GetViewInfosFor(data).ToArray();

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    viewInfos[1]
                }, matchedViewInfos);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfosFor_ViewInfosWithInheritance_ReturnMatchesBasedOnInheritaceDataType()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();

            mocks.ReplayAll();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<A, TestView>(),
                new ViewInfo<InheritedFromA, TestView>(),
                new ViewInfo<object, TestView>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                var data = new A();

                // Call
                ViewInfo[] matchedViewInfos = documentViewController.GetViewInfosFor(data).ToArray();

                // Assert
                ViewInfo[] expected =
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
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();

            mocks.ReplayAll();

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

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                var data = new InheritedFromA();

                // Call
                ViewInfo[] matchedViewInfos = documentViewController.GetViewInfosFor(data).ToArray();

                // Assert
                ViewInfo[] expected =
                {
                    viewInfos[0],
                    viewInfos[1]
                };
                CollectionAssert.AreEqual(expected, matchedViewInfos);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseAllViewsFor_DataIsNull_DoNothing()
        {
            // Setup
            var data1 = new A();
            var data2 = new InheritedFromA();
            var testView = new TestView
            {
                Data = data1
            };
            var testViewDerivative = new TestViewDerivative
            {
                Data = data2
            };

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(new[]
            {
                testView,
                testViewDerivative
            });

            mocks.ReplayAll();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<A, TestView>(),
                new ViewInfo<InheritedFromA, TestViewDerivative>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                documentViewController.CloseAllViewsFor(null);
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CloseAllViewsFor_DataDoesNotCorrespondToOpenedViews_DoNothing()
        {
            // Setup
            var data1 = new A();
            var data2 = new InheritedFromA();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();
            var documentViews = new List<IView>();
            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(documentViews);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                documentViews.Add(invocation.Arguments[0] as TestView);
            }).Repeat.Twice();
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments().Repeat.Twice();
            mocks.ReplayAll();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<A, TestView>(),
                new ViewInfo<InheritedFromA, TestViewDerivative>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                documentViewController.OpenViewForData(data1);
                documentViewController.OpenViewForData(data2);

                // Call
                documentViewController.CloseAllViewsFor(new object());
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CloseAllViewsFor_DataCorrespondsToOpenedView_RemoveThatView()
        {
            // Setup
            var data1 = new A();
            var data2 = new InheritedFromA();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();
            var documentViews = new List<IView>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(documentViews);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                documentViews.Add(invocation.Arguments[0] as TestView);
            }).Repeat.Twice();
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments().Repeat.Twice();
            viewHost.Expect(vh => vh.Remove(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                documentViews.Remove(invocation.Arguments[0] as TestView);
            });

            mocks.ReplayAll();

            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<A, TestView>(),
                new ViewInfo<InheritedFromA, TestViewDerivative>()
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                documentViewController.OpenViewForData(data1);
                documentViewController.OpenViewForData(data2);

                // Call
                documentViewController.CloseAllViewsFor(data1);
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CloseAllViewsFor_DataDoesNotCorrespondToOpenedViewsButCloseForDataReturnsTrue_RemoveViews()
        {
            // Setup
            var data1 = new A();
            var data2 = new InheritedFromA();
            var unusedViewData = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();
            var documentViews = new List<IView>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(documentViews);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                documentViews.Add(invocation.Arguments[0] as TestView);
            }).Repeat.Twice();
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments().Repeat.Twice();
            viewHost.Expect(vh => vh.Remove(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                documentViews.Remove(invocation.Arguments[0] as TestView);
            }).Repeat.Twice();
            mocks.ReplayAll();

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

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                documentViewController.OpenViewForData(data1);
                documentViewController.OpenViewForData(data2);

                // Call
                documentViewController.CloseAllViewsFor(unusedViewData);
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CloseAllViewsFor_DataCorrespondsToOpenedViewWithViewInfoThatBindsToSameViews_RemoveCorrectView()
        {
            // Setup
            var data = new A();
            var viewData = new object();

            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            var viewHost = mocks.StrictMock<IViewHost>();
            var documentViews = new List<IView>();

            viewHost.Stub(vh => vh.ViewClosed += null).IgnoreArguments();
            viewHost.Stub(vh => vh.ViewClosed -= null).IgnoreArguments();
            viewHost.Stub(vh => vh.DocumentViews).Return(documentViews);
            viewHost.Expect(vm => vm.AddDocumentView(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                documentViews.Add(invocation.Arguments[0] as TestView);
            });
            viewHost.Expect(vh => vh.SetImage(null, null)).IgnoreArguments();
            viewHost.Expect(vh => vh.Remove(Arg<TestView>.Is.NotNull)).WhenCalled(invocation =>
            {
                documentViews.Remove(invocation.Arguments[0] as TestView);
            });

            mocks.ReplayAll();

            var viewClosed = false;
            var viewInfos = new ViewInfo[]
            {
                new ViewInfo<B, object, TestView>
                {
                    CloseForData = (v, o) =>
                    {
                        Assert.Fail("Incorrect CloseForData called.");
                        return true;
                    }
                },
                new ViewInfo<A, object, TestView>
                {
                    CloseForData = (v, o) =>
                    {
                        if (o == viewData)
                        {
                            viewClosed = true;
                            return true;
                        }

                        return false;
                    }
                }
            };

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                documentViewController.OpenViewForData(data);

                // Call
                documentViewController.CloseAllViewsFor(viewData);
            }

            // Assert
            Assert.IsTrue(viewClosed);
            mocks.VerifyAll();
        }

        private class A {}

        private class B {}

        private class InheritedFromA : A {}
    }
}