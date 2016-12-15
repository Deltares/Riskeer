﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core.Common.Gui.Test.Forms.ViewHost
{
    [TestFixture]
    [RequiresSTA]
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
                Assert.IsFalse(IsAnyViewFocussed(avalonDockViewHost));
            }
        }

        [Test]
        public void Dispose_ViewHostWithMultipleViews_ViewsClearedActiveDocumentViewSetToNullAndActiveDocumentEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var testView3 = new TestView();
            var testView4 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;
            var avalonDockViewHost = new AvalonDockViewHost();

            avalonDockViewHost.AddDocumentView(testView1);
            avalonDockViewHost.AddDocumentView(testView2);
            avalonDockViewHost.AddToolView(testView3, ToolViewLocation.Left);
            avalonDockViewHost.AddToolView(testView4, ToolViewLocation.Left);

            // Precondition
            Assert.AreSame(testView2, avalonDockViewHost.ActiveDocumentView);
            Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView4));

            CollectionAssert.AreEqual(
                new[]
                {
                    testView1,
                    testView2
                },
                avalonDockViewHost.DocumentViews);

            CollectionAssert.AreEqual(
                new[]
                {
                    testView3,
                    testView4
                },
                avalonDockViewHost.ToolViews);

            avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
            avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

            // Call
            avalonDockViewHost.Dispose();

            // Assert
            Assert.AreEqual(1, activeDocumentViewChangingCounter);
            Assert.AreEqual(1, activeDocumentViewChangedCounter);
            CollectionAssert.IsEmpty(avalonDockViewHost.DocumentViews);
            CollectionAssert.IsEmpty(avalonDockViewHost.ToolViews);
            Assert.IsNull(avalonDockViewHost.ActiveDocumentView);
            Assert.IsFalse(IsAnyViewFocussed(avalonDockViewHost));
        }

        [Test]
        public void GivenHostWithView_WhenHostFocusLost_ViewChildrenAreValidated()
        {
            // Setup
            const string validatingTextBoxEventName = "Validating_TextBox", validatedTextBoxEventName = "Validated_TextBox";
            const string validatingNumericUpDownEventName = "Validating_NumericUpDown", validatedNumericUpDownEventName = "Validated_NumericUpDown";
            var firedEvents = new List<string>();

            var control1 = new TextBox();
            control1.Validating += (sender, args) => { firedEvents.Add(validatingTextBoxEventName); };
            control1.Validated += (sender, args) => { firedEvents.Add(validatedTextBoxEventName); };

            var control2 = new NumericUpDown();
            control2.Validating += (sender, args) => { firedEvents.Add(validatingNumericUpDownEventName); };
            control2.Validated += (sender, args) => { firedEvents.Add(validatedNumericUpDownEventName); };

            var testView = new TestView();
            testView.Controls.AddRange(new Control[]
            {
                control1,
                control2
            });

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView);

                // When
                avalonDockViewHost.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
            }
            // Assert
            CollectionAssert.AreEqual(new[]
            {
                validatingTextBoxEventName,
                validatedTextBoxEventName,
                validatingNumericUpDownEventName,
                validatedNumericUpDownEventName
            }, firedEvents);
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
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void AddDocumentView_MultipleTestViews_LastAddedViewSetAsActiveDocumentViewAndActiveDocumentEventsFired(int numberOfViewsToAdd)
        {
            // Setup
            var viewList = new List<IView>();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                for (var i = 0; i < numberOfViewsToAdd; i++)
                {
                    var testView = new TestView();

                    viewList.Add(testView);

                    // Call
                    avalonDockViewHost.AddDocumentView(testView);
                }

                // Assert
                Assert.AreEqual(numberOfViewsToAdd, activeDocumentViewChangingCounter);
                Assert.AreEqual(numberOfViewsToAdd, activeDocumentViewChangedCounter);
                Assert.AreSame(viewList.Last(), avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, viewList.Last()));
            }
        }

        [Test]
        public void AddDocumentView_ActiveDocumentViewWasAlreadyAdded_NoDuplicationNoViewOpenedEventFiredAndNoActiveDocumentEventsFired()
        {
            // Setup
            var testView = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView);

                var viewOpenedCounter = 0;
                avalonDockViewHost.ViewOpened += (sender, args) => viewOpenedCounter++;

                // Precondition
                Assert.AreSame(testView, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView));
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView
                    },
                    avalonDockViewHost.DocumentViews);

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                // Call
                avalonDockViewHost.AddDocumentView(testView);

                // Assert
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreSame(testView, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView));
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
        public void AddDocumentView_NonActiveDocumentViewWasAlreadyAdded_NoDuplicationNoViewOpenedEventFiredViewSetAsActiveDocumentViewAndActiveDocumentEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);

                var viewOpenedCounter = 0;
                avalonDockViewHost.ViewOpened += (sender, args) => viewOpenedCounter++;

                // Precondition
                Assert.AreNotSame(testView1, avalonDockViewHost.ActiveDocumentView);
                Assert.IsFalse(IsFocussedView(avalonDockViewHost, testView1));
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView1,
                        testView2
                    },
                    avalonDockViewHost.DocumentViews);

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                // Call
                avalonDockViewHost.AddDocumentView(testView1);

                // Assert
                Assert.AreEqual(1, activeDocumentViewChangingCounter);
                Assert.AreEqual(1, activeDocumentViewChangedCounter);
                Assert.AreSame(testView1, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView1));
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView1,
                        testView2
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
        public void Remove_ActiveDocumentViewInViewHostWithNoOtherDocumentViews_ActiveDocumentViewSetToNullAndActiveDocumentEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddToolView(testView2, ToolViewLocation.Left);

                // Precondition
                Assert.AreSame(testView1, avalonDockViewHost.ActiveDocumentView);
                Assert.IsFalse(IsFocussedView(avalonDockViewHost, testView1));

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                // Call
                avalonDockViewHost.Remove(testView1);

                // Assert
                Assert.AreEqual(1, activeDocumentViewChangingCounter);
                Assert.AreEqual(1, activeDocumentViewChangedCounter);
                Assert.IsNull(avalonDockViewHost.ActiveDocumentView);
                Assert.IsFalse(IsAnyViewFocussed(avalonDockViewHost));
            }
        }

        [Test]
        public void Remove_ActiveDocumentViewInViewHostWithMultipleOtherViews_ActiveDocumentViewSetToOtherViewAndActiveDocumentEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var testView3 = new TestView();
            var testView4 = new TestView();
            var testView5 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);
                avalonDockViewHost.AddDocumentView(testView3);
                avalonDockViewHost.AddDocumentView(testView4);
                avalonDockViewHost.AddToolView(testView5, ToolViewLocation.Left);

                avalonDockViewHost.SetFocusToView(testView1);

                // Precondition
                Assert.AreSame(testView1, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView1));

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                // Call
                avalonDockViewHost.Remove(testView1);

                // Assert
                Assert.AreEqual(1, activeDocumentViewChangingCounter);
                Assert.AreEqual(1, activeDocumentViewChangedCounter);
                Assert.IsNotNull(avalonDockViewHost.ActiveDocumentView);
                Assert.AreNotSame(testView1, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, avalonDockViewHost.ActiveDocumentView));
            }
        }

        [Test]
        public void Remove_NotActiveDocumentViewInViewHostWithMultipleOtherViews_ActiveDocumentViewNotAffectedAndNoActiveDocumentEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var testView3 = new TestView();
            var testView4 = new TestView();
            var testView5 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);
                avalonDockViewHost.AddDocumentView(testView3);
                avalonDockViewHost.AddDocumentView(testView4);
                avalonDockViewHost.AddToolView(testView5, ToolViewLocation.Left);

                avalonDockViewHost.SetFocusToView(testView3);

                // Precondition
                Assert.AreSame(testView3, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView3));

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                // Call
                avalonDockViewHost.Remove(testView1);

                // Assert
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreSame(testView3, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView3));
            }
        }

        [Test]
        public void SetFocusToView_ActiveDocumentView_ActiveDocumentViewNotAffectedAndNoActiveDocumentEventsFired()
        {
            // Setup
            var testView = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView);

                // Precondition
                Assert.AreSame(testView, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView));

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                // Call
                avalonDockViewHost.SetFocusToView(testView);

                // Assert
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreSame(testView, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView));
            }
        }

        [Test]
        public void SetFocusToView_NonActiveDocumentView_ActiveDocumentViewSetAndActiveDocumentEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);

                // Precondition
                Assert.AreSame(testView2, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView2));

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                // Call
                avalonDockViewHost.SetFocusToView(testView1);

                // Assert
                Assert.AreEqual(1, activeDocumentViewChangingCounter);
                Assert.AreEqual(1, activeDocumentViewChangedCounter);
                Assert.AreSame(testView1, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView1));
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
            var otherToolViewLocations = Enum.GetValues(typeof(ToolViewLocation))
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
            var invalidLocation = 4;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            using (var testView = new TestView())
            {
                // Call
                TestDelegate test = () => avalonDockViewHost.AddToolView(testView, (ToolViewLocation) invalidLocation);

                // Assert
                string expectedMessage = string.Format("The value of argument 'toolViewLocation' ({0}) is invalid for Enum type 'ToolViewLocation'.", invalidLocation);
                string parameter = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(
                    test,
                    expectedMessage).ParamName;
                Assert.AreEqual("toolViewLocation", parameter);
            }
        }

        [Test]
        public void AddToolView_ActiveDocumentViewNotNull_ActiveDocumentViewNotAffectedAndNoActiveDocumentEventsFired()
        {
            // Setup
            var testToolView = new TestView();
            var testDocumentView = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testDocumentView);

                // Precondition
                Assert.AreSame(testDocumentView, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testDocumentView));

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                // Call
                avalonDockViewHost.AddToolView(testToolView, ToolViewLocation.Left);

                // Assert
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreSame(testDocumentView, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testToolView));
            }
        }

        [Test]
        public void AddToolView_ToolViewWasAlreadyAdded_NoDuplicationNoViewOpenedEventFiredAndViewFocussed()
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

                // Precondition
                CollectionAssert.AreEqual(
                    new[]
                    {
                        testView1,
                        testView2
                    },
                    avalonDockViewHost.ToolViews);
                Assert.IsFalse(IsFocussedView(avalonDockViewHost, testView1));

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
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView1));
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
        public void Remove_ToolViewInViewHostWithMultipleOtherViews_ActiveDocumentViewNotAffectedAndNoActiveDocumentEventsFired()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();
            var testView3 = new TestView();
            var testView4 = new TestView();
            var testView5 = new TestView();
            var activeDocumentViewChangingCounter = 0;
            var activeDocumentViewChangedCounter = 0;

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddDocumentView(testView1);
                avalonDockViewHost.AddDocumentView(testView2);
                avalonDockViewHost.AddDocumentView(testView3);
                avalonDockViewHost.AddDocumentView(testView4);
                avalonDockViewHost.AddToolView(testView5, ToolViewLocation.Left);

                avalonDockViewHost.SetFocusToView(testView3);

                // Precondition
                Assert.AreSame(testView3, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView3));

                avalonDockViewHost.ActiveDocumentViewChanging += (sender, args) => { activeDocumentViewChangingCounter++; };
                avalonDockViewHost.ActiveDocumentViewChanged += (sender, args) => { activeDocumentViewChangedCounter++; };

                // Call
                avalonDockViewHost.Remove(testView5);

                // Assert
                Assert.AreEqual(0, activeDocumentViewChangingCounter);
                Assert.AreEqual(0, activeDocumentViewChangedCounter);
                Assert.AreSame(testView3, avalonDockViewHost.ActiveDocumentView);
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView3));
            }
        }

        [Test]
        public void SetFocusToView_NonFocussedToolView_ToolViewFocussed()
        {
            // Setup
            var testView1 = new TestView();
            var testView2 = new TestView();

            using (var avalonDockViewHost = new AvalonDockViewHost())
            {
                avalonDockViewHost.AddToolView(testView1, ToolViewLocation.Left);
                avalonDockViewHost.AddToolView(testView2, ToolViewLocation.Right);

                // Precondition
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView2));

                // Call
                avalonDockViewHost.SetFocusToView(testView1);

                // Assert
                Assert.IsTrue(IsFocussedView(avalonDockViewHost, testView1));
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

        private static bool IsFocussedView(AvalonDockViewHost avalonDockViewHost, IView view)
        {
            var dockingManager = TypeUtils.GetField<DockingManager>(avalonDockViewHost, "DockingManager");

            return ((WindowsFormsHost) dockingManager.ActiveContent).Child == view;
        }

        private static bool IsAnyViewFocussed(AvalonDockViewHost avalonDockViewHost)
        {
            var dockingManager = TypeUtils.GetField<DockingManager>(avalonDockViewHost, "DockingManager");

            return dockingManager.ActiveContent != null;
        }

        private static bool IsImageSet(AvalonDockViewHost avalonDockViewHost, IView view)
        {
            var dockingManager = TypeUtils.GetField<DockingManager>(avalonDockViewHost, "DockingManager");

            return dockingManager.Layout.Descendents()
                                 .OfType<LayoutContent>()
                                 .First(lc => ((WindowsFormsHost) lc.Content).Child == view).IconSource != null;
        }

        #endregion

        private class TestView : UserControl, IView
        {
            public object Data { get; set; }
        }
    }
}