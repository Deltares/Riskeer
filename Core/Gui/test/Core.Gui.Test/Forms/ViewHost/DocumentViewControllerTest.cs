// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using System.Windows.Media;
using Core.Common.Controls.Views;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;

namespace Core.Gui.Test.Forms.ViewHost
{
    [TestFixture]
    public class DocumentViewControllerTest : NUnitFormTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            // Call
            using (var documentViewController = new DocumentViewController(viewHost,
                                                                           Enumerable.Empty<ViewInfo>(),
                                                                           dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<IDocumentViewController>(documentViewController);
                CollectionAssert.IsEmpty(documentViewController.DefaultViewTypes);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OpenViewForData_DataIsNull_ReturnFalse(bool forceShowDialog)
        {
            // Setup
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OpenViewForData_NoViewInfoRegistered_ReturnFalse(bool forceShowDialog)
        {
            // Setup
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            var viewInfos = new ViewInfo[0];

            using (var documentViewController = new DocumentViewController(viewHost, viewInfos, dialogParent))
            {
                // Call
                bool result = documentViewController.OpenViewForData(new object(), forceShowDialog);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void OpenViewForData_DataHasSingleMatch_ReturnTrueAndAddToViewHost()
        {
            // Setup
            TestView view = null;
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            const string title = "<title>";
            const string symbol = "<symbol>";
            var fontFamily = new FontFamily();

            viewHost.DocumentViews.Returns(new IView[0]);
            viewHost.When(vh => vh.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Is(title),
                              Arg.Is(symbol),
                              Arg.Is(fontFamily)))
                    .Do(callInfo =>
                    {
                        view = callInfo.Arg<TestView>();
                    });
            var data = new object();
            const string viewData = "<I'm a piece of view data>";
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
                        return title;
                    },
                    GetSymbol = () => symbol,
                    GetFontFamily = () => fontFamily
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
                Assert.IsTrue(afterCreateCalled);
            }
        }

        [Test]
        public void OpenViewForData_DataHasSingleMatchOnBaseType_ReturnTrueAndAddToViewHost()
        {
            // Setup
            TestView view = null;
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            const string viewName = "<cool view name>";

            viewHost.DocumentViews.Returns(new IView[0]);

            viewHost.When(x => x.AddDocumentView(Arg.Any<TestView>(),
                                                 viewName, Arg.Any<string>(), Arg.Any<FontFamily>()))
                    .Do(invocation =>
                    {
                        view = invocation.Args()[0] as TestView;
                    });
            var data = new InheritedFromA();
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
                Assert.IsTrue(afterCreateCalled);
            }
        }

        [Test]
        public void OpenViewForData_DataHasMultipleMatchesOnType_ResolveToMostSpecializedAndReturnTrueAndAddToViewHost()
        {
            // Setup
            TestViewDerivative view = null;
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new IView[0]);
            viewHost.When(x => x.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(invocation =>
                    {
                        view = invocation.Args()[0] as TestViewDerivative;
                    });
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
        }

        [Test]
        public void OpenViewForData_ViewInfosForInheritedData_ResolveToMostSpecializedForDataAndReturnTrueAndAddToViewHost()
        {
            // Setup
            TestView view = null;
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new IView[0]);
            viewHost.When(x => x.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(invocation =>
                    {
                        view = invocation.Args()[0] as TestView;
                    });
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
        }

        [Test]
        public void OpenViewForData_DataHasMultipleSingleMatches_UseAdditionalDataCheckAndReturnTrueAndAddToViewHost()
        {
            // Setup
            TestViewDerivative view = null;
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new IView[0]);
            viewHost.When(x => x.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(invocation =>
                    {
                        view = invocation.Args()[0] as TestViewDerivative;
                    });
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
        }

        [Test]
        public void OpenViewForData_ClickCancelInOpenedDialog_ReturnFalseAndNoViewAddedToViewHost()
        {
            // Setup
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new IView[0]);
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
        }

        [Test]
        public void OpenViewForData_ClickOkInOpenedDialog_ReturnTrueAndViewAddedToViewHost()
        {
            // Setup
            TestView view = null;
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new IView[0]);
            viewHost.When(x => x.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(invocation =>
                    {
                        view = invocation.Args()[0] as TestView;
                    });
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
        }

        [Test]
        public void OpenViewForData_MarkAsDefaultViewAndClickOkInOpenedDialog_ReturnTrueViewAddedToViewHostAndDefaultViewTypesUpdated()
        {
            // Setup
            TestView view = null;
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new IView[0]);
            viewHost.When(x => x.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(invocation =>
                    {
                        view = invocation.Args()[0] as TestView;
                    });
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
        }

        [Test]
        public void OpenViewForData_SelectDifferentDefaultViewAndClickOkInOpenedDialog_ReturnTrueViewAddedToViewHostAndDefaultViewTypesUpdated()
        {
            // Setup
            TestView view = null;
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new IView[0]);
            viewHost.When(x => x.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(invocation =>
                    {
                        view = invocation.Args()[0] as TestView;
                    });
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
        }

        [Test]
        public void OpenViewForData_DataHasMultipleMatchesAndRegisteredDefaultView_ReturnTrueAndAddDefaultViewToViewHost()
        {
            // Setup
            TestViewDerivative view = null;
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new IView[0]);
            viewHost.When(x => x.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(invocation =>
                    {
                        view = invocation.Args()[0] as TestViewDerivative;
                    });
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
        }

        [Test]
        public void OpenViewForData_OpenSameViewForTwoDifferentDataInstances_OpenTwoViews()
        {
            // Setup
            var data1 = new object();
            var data2 = new object();
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new IView[0]);
            viewHost.AddDocumentView(
                Arg.Any<TestView>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<FontFamily>());
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
            viewHost.Received().AddDocumentView(
                Arg.Is<TestView>(c => c.Data == data1),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<FontFamily>());
            viewHost.Received().AddDocumentView(
                Arg.Is<TestView>(c => c.Data == data2),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<FontFamily>());
        }

        [Test]
        public void OpenViewForData_OpeningViewForAlreadyOpenedButInactiveView_ActivateDocumentView()
        {
            // Setup
            var viewList = new List<IView>();
            var data = new object();
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(viewList);
            viewHost.When(vh => vh.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(callInfo =>
                    {
                        viewList.Add(callInfo.Args()[0] as TestView);
                    });

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
            viewHost.Received().BringToFront(Arg.Is<TestView>(c => c == viewList.First()));
        }

        [Test]
        public void GetViewInfosFor_NoViewInfosRegistered_ReturnEmpty()
        {
            // Setup
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            using (var documentViewController = new DocumentViewController(viewHost, Enumerable.Empty<ViewInfo>(), dialogParent))
            {
                var data = new object();

                // Call
                IEnumerable<ViewInfo> matchedViewInfos = documentViewController.GetViewInfosFor(data);

                // Assert
                CollectionAssert.IsEmpty(matchedViewInfos);
            }
        }

        [Test]
        public void GetViewInfosFor_SingleDirectMatch_ReturnSingleMatchingViewInfo()
        {
            // Setup
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

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
        }

        [Test]
        public void GetViewInfosFor_ViewInfosWithInheritance_ReturnMatchesBasedOnInheritanceDataType()
        {
            // Setup
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

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
        }

        [Test]
        public void GetViewInfosFor_ViewInfosWithAdditionalDataCheck_ReturnMatchesWithAdditionalDataCheckTrue()
        {
            // Setup
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

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
        }

        [Test]
        public void CloseAllViews_Always_RemoveViews()
        {
            // Setup
            var data1 = new A();
            var data2 = new InheritedFromA();
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();
            var documentViews = new List<IView>();

            viewHost.DocumentViews.Returns(documentViews);
            viewHost.When(vh => vh.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(callInfo =>
                    {
                        documentViews.Add(callInfo.Args()[0] as TestView);
                    });

            viewHost.When(x => x.Remove(Arg.Any<TestView>()))
                    .Do(invocation =>
                    {
                        documentViews.Remove(invocation.Args()[0] as TestView);
                    });
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
                documentViewController.CloseAllViews();
            }

            // Assert
            CollectionAssert.IsEmpty(documentViews);
            viewHost.Received(2).AddDocumentView(Arg.Any<TestView>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontFamily>());
            viewHost.Received(2).Remove(Arg.Any<TestView>());
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
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();

            viewHost.DocumentViews.Returns(new[]
            {
                testView,
                testViewDerivative
            });
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
        }

        [Test]
        public void CloseAllViewsFor_DataDoesNotCorrespondToOpenedViews_DoNothing()
        {
            // Setup
            var data1 = new A();
            var data2 = new InheritedFromA();
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();
            var documentViews = new List<IView>();

            viewHost.DocumentViews.Returns(documentViews);
            viewHost.When(vh => vh.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(callInfo =>
                    {
                        documentViews.Add(callInfo.Args()[0] as TestView);
                    });
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
            viewHost.Received(2).AddDocumentView(Arg.Any<TestView>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontFamily>());
        }

        [Test]
        public void CloseAllViewsFor_DataCorrespondsToOpenedView_RemoveThatView()
        {
            // Setup
            var data1 = new A();
            var data2 = new InheritedFromA();
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();
            var documentViews = new List<IView>();

            viewHost.DocumentViews.Returns(documentViews);

            viewHost.When(vh => vh.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(callInfo =>
                    {
                        documentViews.Add(callInfo.Args()[0] as TestView);
                    });

            viewHost.When(x => x.Remove(Arg.Any<TestView>()))
                    .Do(invocation =>
                    {
                        documentViews.Remove(invocation.Args()[0] as TestView);
                    });

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
            viewHost.Received().AddDocumentView(Arg.Any<TestView>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontFamily>());
            viewHost.Received().Remove(Arg.Any<TestView>());
        }

        [Test]
        public void CloseAllViewsFor_DataDoesNotCorrespondToOpenedViewsButCloseForDataReturnsTrue_RemoveViews()
        {
            // Setup
            var data1 = new A();
            var data2 = new InheritedFromA();
            var unusedViewData = new object();
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();
            var documentViews = new List<IView>();

            viewHost.DocumentViews.Returns(documentViews);
            viewHost.When(vh => vh.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(callInfo =>
                    {
                        documentViews.Add(callInfo.Args()[0] as TestView);
                    });

            viewHost.When(x => x.Remove(Arg.Any<TestView>()))
                    .Do(invocation =>
                    {
                        documentViews.Remove(invocation.Args()[0] as TestView);
                    });
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
            viewHost.Received(2).AddDocumentView(Arg.Any<TestView>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontFamily>());
            viewHost.Received(2).Remove(Arg.Any<TestView>());
        }

        [Test]
        public void CloseAllViewsFor_DataCorrespondsToOpenedViewWithViewInfoThatBindsToSameViews_RemoveCorrectView()
        {
            // Setup
            var data = new A();
            var viewData = new object();
            var dialogParent = Substitute.For<IWin32Window>();
            var viewHost = Substitute.For<IViewHost>();
            var documentViews = new List<IView>();

            viewHost.DocumentViews.Returns(documentViews);

            viewHost.When(vh => vh.AddDocumentView(
                              Arg.Any<TestView>(),
                              Arg.Any<string>(),
                              Arg.Any<string>(),
                              Arg.Any<FontFamily>()))
                    .Do(callInfo =>
                    {
                        documentViews.Add(callInfo.Args()[0] as TestView);
                    });

            viewHost.When(x => x.Remove(Arg.Any<TestView>()))
                    .Do(invocation =>
                    {
                        documentViews.Remove(invocation.Args()[0] as TestView);
                    });

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
            viewHost.Received(1).AddDocumentView(Arg.Any<TestView>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<FontFamily>());
            viewHost.Received(1).Remove(Arg.Any<TestView>());
        }

        private class A {}

        private class B {}

        private class InheritedFromA : A {}
    }
}