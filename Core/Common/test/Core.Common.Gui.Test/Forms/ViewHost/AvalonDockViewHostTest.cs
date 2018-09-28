// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Common.Gui.Test.Forms.ViewHost
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class AvalonDockViewHostTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                // Assert
                Assert.IsInstanceOf<IViewHost>(avalonDockViewHost);
                CollectionAssert.IsEmpty(avalonDockViewHost.DocumentViews);
                CollectionAssert.IsEmpty(avalonDockViewHost.ToolViews);
                Assert.IsNull(avalonDockViewHost.ActiveDocumentView);
                Assert.IsFalse(IsAnyViewActive(avalonDockViewHost));
            }
        }

        [Test]
        public void Dispose_ViewHostWithMultipleViews_ViewsClearedActiveDocumentViewSetToNullAndActiveViewEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var testView3 = new TestView();
            var testView4 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var activeViewChangedCounter = 0;

            var avalonDockViewHost = new AvalonDockViewHost();

            avalonDockViewHost.AddDocumentView(testView1);
            avalonDockViewHost.AddDocumentView(testView2);
            avalonDockViewHost.AddToolView(testView3, ToolViewLocation.Left);
            avalonDockViewHost.AddToolView(testView4, ToolViewLocation.Left);

            SetActiveView(avalonDockViewHost, testView1);

            CollectionAssert.AreEqual(
                new[]
                {
                    testView3,
                    testView4
                },
                avalonDockViewHost.ToolViews);

            avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => activeDocumentViewChangingCounter++;
            avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => activeDocumentViewChangedCounter++;
            avalonDockViewHost.ActiveViewChanged += (sender, args) => activeViewChangedCounter++;

            // Call
            avalonDockViewHost.Dispose();

            // Assert
            CollectionAssert.IsEmpty(avalonDockViewHost.DocumentViews);
            CollectionAssert.IsEmpty(avalonDockViewHost.ToolViews);
            Assert.AreNotEqual(0, activeDocumentViewChangingCounter);
            Assert.AreNotEqual(0, activeDocumentViewChangedCounter);
            Assert.AreNotEqual(0, activeViewChangedCounter);
            Assert.IsNull(avalonDockViewHost.ActiveDocumentView);
            Assert.IsFalse(IsAnyViewActive(avalonDockViewHost));
        }

        private class TestView : UserControl, IView
        {
            public object Data { get; set; }
        }

        #region Document views

        [Test]
        public void AddDocumentView_NonControlView_ViewNotAddedAndNoViewOpenedEventFired()
        {
            // Setup
            var mocks = new MockRepository();
            var testView = mocks.StrictMock<IView>();
            mocks.ReplayAll();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                var viewOpenedCounter = 0;
                avalonDockViewHost.ViewOpened += (sender, args) => viewOpenedCounter++;

                // Call
                avalonDockViewHost.AddDocumentView(testView);

                // Assert
                CollectionAssert.IsEmpty(avalonDockViewHost.DocumentViews);
                Assert.IsFalse(IsDocumentViewPresent(avalonDockViewHost, testView));
                Assert.AreEqual(0, viewOpenedCounter);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void AddDocumentView_MultipleTestViews_ViewsAddedAndViewOpenedEventsFired(int numberOfViewsToAdd)
        {
            // Setup
            var viewList = new List<IView>();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                var viewOpenedCounter = 0;
                avalonDockViewHost.ViewOpened += (sender, args) =>
                {
                    Assert.AreSame(viewList.Last(), args.View);

                    viewOpenedCounter++;
                };

                for (var i = 0; i < numberOfViewsToAdd; i++)
                {
                    var testView = new TestView();

                    viewList.Add(testView);

                    // Call
                    avalonDockViewHost.AddDocumentView(testView);
                }

                // Assert
                CollectionAssert.AreEqual(viewList, avalonDockViewHost.DocumentViews);
                Assert.IsTrue(viewList.All(v => IsDocumentViewPresent(avalonDockViewHost, v)));
                Assert.AreEqual(numberOfViewsToAdd, viewOpenedCounter);
                Assert.IsNull(avalonDockViewHost.ActiveDocumentView);
            }
        }

        [Test]
        public void AddDocumentView_DocumentViewWasAlreadyAdded_NoDuplicationNoViewOpenedEventFired()
        {
            // Setup
            var testView = new TestView();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView);

                var viewOpenedCounter = 0;
                avalonDockViewHost.ViewOpened += (sender, args) => viewOpenedCounter++;

                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView
                    },
                    avalonDockViewHost.DocumentViews);

                // Call
                avalonDockViewHost.AddDocumentView(testView);

                // Assert
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView
                    },
                    avalonDockViewHost.DocumentViews);
                Assert.AreEqual(0, viewOpenedCounter);
            }
        }

        [Test]
        public void Remove_DocumentViewInViewHostWithMultipleViews_ViewRemoved()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);

                // Precondition
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView1,
                        testView2
                    },
                    avalonDockViewHost.DocumentViews);
                Assert.IsTrue(IsDocumentViewPresent(avalonDockViewHost, testView1));

                // Call
                avalonDockViewHost.Remove(testView1);

                // Assert
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView2
                    },
                    avalonDockViewHost.DocumentViews);
                Assert.IsFalse(IsDocumentViewPresent(avalonDockViewHost, testView1));
            }
        }

        [Test]
        public void Remove_DocumentView_ViewClosedEventsFired()
        {
            // Setup
            var testView = new TestView();
            var viewClosedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView);

                avalonDockViewHost.ViewClosed += (sender, args) =>
                {
                    Assert.AreSame(testView, args.View);

                    viewClosedCounter++;
                };

                // Call
                avalonDockViewHost.Remove(testView);

                // Assert
                Assert.AreEqual(1, viewClosedCounter);
            }
        }

        [Test]
        public void Remove_ActiveDocumentViewInViewHostWithNoOtherDocumentViews_ActiveDocumentViewSetToNullAndActiveDocumentViewEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var activeViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddToolView(testView2, ToolViewLocation.Left);
                SetActiveView(avalonDockViewHost, testView1);

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => activeDocumentViewChangingCounter++;
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => activeDocumentViewChangedCounter++;
                avalonDockViewHost.ActiveViewChanged += (sender, args) => activeViewChangedCounter++;

                // Call
                avalonDockViewHost.Remove(testView1);

                // Assert
                Assert.AreEqual(1, activeDocumentViewChangingCounter);
                Assert.AreEqual(1, activeDocumentViewChangedCounter);
                Assert.AreEqual(0, activeViewChangedCounter);
                Assert.IsNull(avalonDockViewHost.ActiveDocumentView);
            }
        }

        [Test]
        public void Remove_ActiveDocumentViewInViewHostWithMultipleOtherViews_ActiveDocumentViewSetToNullAndActiveDocumentViewEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var testView3 = new TestView();
            var testView4 = new TestView();
            var testView5 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var activeViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);
                avalonDockViewHost.AddDocumentView(testView3);
                avalonDockViewHost.AddDocumentView(testView4);
                avalonDockViewHost.AddToolView(testView5, ToolViewLocation.Left);

                SetActiveView(avalonDockViewHost, testView1);

                // Precondition
                Assert.AreSame(testView1, avalonDockViewHost.ActiveDocumentView);

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => activeDocumentViewChangingCounter++;
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => activeDocumentViewChangedCounter++;
                avalonDockViewHost.ActiveViewChanged += (sender, args) => activeViewChangedCounter++;

                // Call
                avalonDockViewHost.Remove(testView1);

                // Assert
                Assert.AreEqual(1, activeDocumentViewChangingCounter);
                Assert.AreEqual(1, activeDocumentViewChangedCounter);
                Assert.AreEqual(0, activeViewChangedCounter);
                Assert.IsNull(avalonDockViewHost.ActiveDocumentView);
            }
        }

        [Test]
        public void Remove_NotActiveDocumentViewInViewHostWithMultipleOtherViews_ActiveDocumentViewNotAffectedAndNoActiveViewEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var testView3 = new TestView();
            var testView4 = new TestView();
            var testView5 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var activeViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);
                avalonDockViewHost.AddDocumentView(testView3);
                avalonDockViewHost.AddDocumentView(testView4);
                avalonDockViewHost.AddToolView(testView5, ToolViewLocation.Left);

                SetActiveView(avalonDockViewHost, testView3);

                // Precondition
                Assert.AreSame(testView3, avalonDockViewHost.ActiveDocumentView);

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => activeDocumentViewChangingCounter++;
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => activeDocumentViewChangedCounter++;
                avalonDockViewHost.ActiveViewChanged += (sender, args) => activeViewChangedCounter++;

                // Call
                avalonDockViewHost.Remove(testView1);

                // Assert
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreEqual(0, activeViewChangedCounter);
                Assert.AreSame(testView3, avalonDockViewHost.ActiveDocumentView);
            }
        }

        [Test]
        public void BringToFront_ActiveDocumentView_ViewBroughtToFrontFiredButActiveViewNotAffectedAndNoActiveViewEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var viewBroughtToFrontCounter = 0;
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var activeViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);
                SetActiveView(avalonDockViewHost, testView1);

                avalonDockViewHost.ViewBroughtToFront += (sender, args) => viewBroughtToFrontCounter++;
                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => activeDocumentViewChangingCounter++;
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => activeDocumentViewChangedCounter++;
                avalonDockViewHost.ActiveViewChanged += (sender, args) => activeViewChangedCounter++;

                // Call
                avalonDockViewHost.BringToFront(testView1);

                // Assert
                Assert.AreEqual(1, viewBroughtToFrontCounter);
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreEqual(0, activeViewChangedCounter);
                Assert.AreSame(testView1, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsActiveView(avalonDockViewHost, testView1));
            }
        }

        [Test]
        public void BringToFront_NonActiveDocumentView_ViewBroughtToFrontFiredButActiveViewNotAffectedAndNoActiveViewEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var viewBroughtToFrontCounter = 0;
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var activeViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);
                SetActiveView(avalonDockViewHost, testView2);

                avalonDockViewHost.ViewBroughtToFront += (sender, args) => viewBroughtToFrontCounter++;
                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => activeDocumentViewChangingCounter++;
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => activeDocumentViewChangedCounter++;
                avalonDockViewHost.ActiveViewChanged += (sender, args) => activeViewChangedCounter++;

                // Call
                avalonDockViewHost.BringToFront(testView1);

                // Assert
                Assert.AreEqual(1, viewBroughtToFrontCounter);
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreEqual(0, activeViewChangedCounter);
                Assert.AreSame(testView2, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsActiveView(avalonDockViewHost, testView2));
            }
        }

        [Test]
        public void SetImage_DocumentView_ImageSet()
        {
            // Setup
            var testView = new TestView();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView);

                // Precondition
                Assert.IsFalse(IsImageSet(avalonDockViewHost, testView));

                // Call
                avalonDockViewHost.SetImage(testView, new Bitmap(16, 16));

                // Assert
                Assert.IsTrue(IsImageSet(avalonDockViewHost, testView));
            }
        }

        #endregion

        #region Tool views

        [Test]
        public void AddToolView_NonControlView_ViewNotAddedAndNoViewOpenedEventFired()
        {
            // Setup
            var mocks = new MockRepository();
            var testView = mocks.StrictMock<IView>();
            mocks.ReplayAll();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                var viewOpenedCounter = 0;
                avalonDockViewHost.ViewOpened += (sender, args) => viewOpenedCounter++;

                // Call
                avalonDockViewHost.AddToolView(testView, ToolViewLocation.Left);

                // Assert
                CollectionAssert.IsEmpty(avalonDockViewHost.ToolViews);
                Assert.IsFalse(IsToolViewPresent(avalonDockViewHost, testView, ToolViewLocation.Left));
                Assert.AreEqual(0, viewOpenedCounter);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(ToolViewLocation.Left)]
        [TestCase(ToolViewLocation.Right)]
        [TestCase(ToolViewLocation.Bottom)]
        public void AddToolView_TestViews_ViewAddedAndViewOpenedEventFired(ToolViewLocation toolViewLocation)
        {
            // Setup
            var testView = new TestView();
            IEnumerable<ToolViewLocation> otherToolViewLocations = Enum.GetValues(typeof(ToolViewLocation))
                                                                       .Cast<ToolViewLocation>()
                                                                       .Except(new[]
                                                                       {
                                                                           toolViewLocation
                                                                       });

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                var viewOpenedCounter = 0;
                avalonDockViewHost.ViewOpened += (sender, args) =>
                {
                    Assert.AreSame(testView, args.View);

                    viewOpenedCounter++;
                };

                // Call
                avalonDockViewHost.AddToolView(testView, toolViewLocation);

                // Assert
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView
                    },
                    avalonDockViewHost.ToolViews);
                Assert.IsTrue(IsToolViewPresent(avalonDockViewHost, testView, toolViewLocation));
                Assert.IsFalse(otherToolViewLocations.Any(tvl => IsToolViewPresent(avalonDockViewHost, testView, tvl)));
                Assert.AreEqual(1, viewOpenedCounter);
            }
        }

        [Test]
        public void AddToolView_InvalidPosition_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidLocation = 4;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            using (var testView = new TestView())
            {
                // Call
                TestDelegate test = () => avalonDockViewHost.AddToolView(testView, (ToolViewLocation) invalidLocation);

                // Assert
                string expectedMessage = $"The value of argument 'toolViewLocation' ({invalidLocation}) is invalid for Enum type 'ToolViewLocation'.";
                string parameter = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(
                    test,
                    expectedMessage).ParamName;
                Assert.AreEqual("toolViewLocation", parameter);
            }
        }

        [Test]
        public void AddToolView_DocumentViewSetAsActiveView_ActiveDocumentViewNotAffected()
        {
            // Setup
            var testToolView = new TestView();
            var testDocumentView = new TestView();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testDocumentView);
                SetActiveView(avalonDockViewHost, testDocumentView);

                // Call
                avalonDockViewHost.AddToolView(testToolView, ToolViewLocation.Left);

                // Assert
                Assert.AreSame(testDocumentView, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsActiveView(avalonDockViewHost, testDocumentView));
            }
        }

        [Test]
        public void AddToolView_ToolViewWasAlreadyAdded_NoDuplicationNoViewOpenedEventFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddToolView(testView1, ToolViewLocation.Left);
                avalonDockViewHost.AddToolView(testView2, ToolViewLocation.Left);

                var viewOpenedCounter = 0;
                avalonDockViewHost.ViewOpened += (sender, args) => viewOpenedCounter++;

                // Call
                avalonDockViewHost.AddToolView(testView1, ToolViewLocation.Right);

                // Assert
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView1,
                        testView2
                    },
                    avalonDockViewHost.ToolViews);
                Assert.AreEqual(0, viewOpenedCounter);
            }
        }

        [Test]
        public void Remove_ToolViewInViewHostWithMultipleViews_ViewRemoved()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddToolView(testView1, ToolViewLocation.Left);
                avalonDockViewHost.AddToolView(testView2, ToolViewLocation.Left);

                // Precondition
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView1,
                        testView2
                    },
                    avalonDockViewHost.ToolViews);

                // Call
                avalonDockViewHost.Remove(testView1);

                // Assert
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView2
                    },
                    avalonDockViewHost.ToolViews);
            }
        }

        [Test]
        public void Remove_ToolView_ViewClosedEventsFired()
        {
            // Setup
            var testView = new TestView();
            var viewClosedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddToolView(testView, ToolViewLocation.Left);

                avalonDockViewHost.ViewClosed += (sender, args) =>
                {
                    Assert.AreSame(testView, args.View);

                    viewClosedCounter++;
                };

                // Call
                avalonDockViewHost.Remove(testView);

                // Assert
                Assert.AreEqual(1, viewClosedCounter);
            }
        }

        [Test]
        public void Remove_ToolViewInViewHostWithMultipleOtherViews_ActiveDocumentViewNotAffectedAndNoActiveViewEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var testView3 = new TestView();
            var testView4 = new TestView();
            var testView5 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var activeViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);
                avalonDockViewHost.AddDocumentView(testView3);
                avalonDockViewHost.AddDocumentView(testView4);
                avalonDockViewHost.AddToolView(testView5, ToolViewLocation.Left);

                SetActiveView(avalonDockViewHost, testView3);
                SetActiveView(avalonDockViewHost, testView5);

                // Precondition
                Assert.AreSame(testView3, avalonDockViewHost.ActiveDocumentView);

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => activeDocumentViewChangingCounter++;
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => activeDocumentViewChangedCounter++;
                avalonDockViewHost.ActiveViewChanged += (sender, args) => activeViewChangedCounter++;

                // Call
                avalonDockViewHost.Remove(testView5);

                // Assert
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreEqual(0, activeViewChangedCounter);
                Assert.AreSame(testView3, avalonDockViewHost.ActiveDocumentView);
            }
        }

        [Test]
        public void BringToFront_ActiveToolView_ViewBroughtToFrontFiredButActiveViewNotAffectedAndNoActiveViewEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var viewBroughtToFrontCounter = 0;
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var activeViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddToolView(testView1, ToolViewLocation.Left);
                avalonDockViewHost.AddToolView(testView2, ToolViewLocation.Bottom);
                SetActiveView(avalonDockViewHost, testView1);

                avalonDockViewHost.ViewBroughtToFront += (sender, args) => viewBroughtToFrontCounter++;
                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => activeDocumentViewChangingCounter++;
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => activeDocumentViewChangedCounter++;
                avalonDockViewHost.ActiveViewChanged += (sender, args) => activeViewChangedCounter++;

                // Call
                avalonDockViewHost.BringToFront(testView1);

                // Assert
                Assert.AreEqual(1, viewBroughtToFrontCounter);
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreEqual(0, activeViewChangedCounter);
                Assert.IsTrue(IsActiveView(avalonDockViewHost, testView1));
            }
        }

        [Test]
        public void BringToFront_NonActiveToolView_ViewBroughtToFrontFiredButActiveViewNotAffectedAndNoActiveViewEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var viewBroughtToFrontCounter = 0;
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var activeViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddToolView(testView1, ToolViewLocation.Left);
                avalonDockViewHost.AddToolView(testView2, ToolViewLocation.Bottom);
                SetActiveView(avalonDockViewHost, testView2);

                avalonDockViewHost.ViewBroughtToFront += (sender, args) => viewBroughtToFrontCounter++;
                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => activeDocumentViewChangingCounter++;
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => activeDocumentViewChangedCounter++;
                avalonDockViewHost.ActiveViewChanged += (sender, args) => activeViewChangedCounter++;

                // Call
                avalonDockViewHost.BringToFront(testView1);

                // Assert
                Assert.AreEqual(1, viewBroughtToFrontCounter);
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreEqual(0, activeViewChangedCounter);
                Assert.IsTrue(IsActiveView(avalonDockViewHost, testView2));
            }
        }

        [Test]
        public void SetImage_ToolView_ImageSet()
        {
            // Setup
            var testView = new TestView();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddToolView(testView, ToolViewLocation.Left);

                // Precondition
                Assert.IsFalse(IsImageSet(avalonDockViewHost, testView));

                // Call
                avalonDockViewHost.SetImage(testView, new Bitmap(16, 16));

                // Assert
                Assert.IsTrue(IsImageSet(avalonDockViewHost, testView));
            }
        }

        #endregion

        #region Helper methods

        private static bool IsDocumentViewPresent(AvalonDockViewHost avalonDockViewHost, IView documentView)
        {
            var layoutDocumentPaneGroup = TypeUtils.GetField<LayoutDocumentPaneGroup>(avalonDockViewHost, "LayoutDocumentPaneGroup");

            return layoutDocumentPaneGroup.Descendents()
                                          .OfType<LayoutDocumentPane>()
                                          .First()
                                          .Children
                                          .Any(c => ((WindowsFormsHost) c.Content).Child == documentView);
        }

        private static bool IsToolViewPresent(AvalonDockViewHost avalonDockViewHost, IView toolView, ToolViewLocation toolViewLocation)
        {
            string paneField;

            switch (toolViewLocation)
            {
                case ToolViewLocation.Left:
                    paneField = "LeftLayoutAnchorablePaneGroup";
                    break;
                case ToolViewLocation.Right:
                    paneField = "RightLayoutAnchorablePaneGroup";
                    break;
                case ToolViewLocation.Bottom:
                    paneField = "BottomLayoutAnchorablePaneGroup";
                    break;
                default:
                    paneField = "";
                    break;
            }

            var layoutAnchorablePaneGroup = TypeUtils.GetField<LayoutAnchorablePaneGroup>(avalonDockViewHost, paneField);

            return layoutAnchorablePaneGroup.Descendents()
                                            .OfType<LayoutAnchorablePane>()
                                            .First()
                                            .Children
                                            .Any(c => ((WindowsFormsHost) c.Content).Child == toolView);
        }

        private static bool IsActiveView(AvalonDockViewHost avalonDockViewHost, IView view)
        {
            return ((WindowsFormsHost) avalonDockViewHost.DockingManager.ActiveContent).Child == view;
        }

        private static bool IsAnyViewActive(AvalonDockViewHost avalonDockViewHost)
        {
            return avalonDockViewHost.DockingManager.ActiveContent != null;
        }

        private static bool IsImageSet(AvalonDockViewHost avalonDockViewHost, IView view)
        {
            return avalonDockViewHost.DockingManager
                                     .Layout
                                     .Descendents()
                                     .OfType<LayoutContent>()
                                     .First(lc => ((WindowsFormsHost) lc.Content).Child == view).IconSource != null;
        }

        private static void SetActiveView(AvalonDockViewHost avalonDockViewHost, IView view)
        {
            avalonDockViewHost.DockingManager.Layout.Descendents()
                              .OfType<LayoutContent>()
                              .First(d => ((WindowsFormsHost) d.Content).Child == view)
                              .IsActive = true;
        }

        #endregion
    }
}