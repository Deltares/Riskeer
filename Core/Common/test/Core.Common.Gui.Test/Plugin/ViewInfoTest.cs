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
using System.Drawing;

using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Test.Properties;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Plugin
{
    [TestFixture]
    public class ViewInfoTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var viewInfo = new ViewInfo();

            // Assert
            Assert.IsNull(viewInfo.DataType);
            Assert.IsNull(viewInfo.ViewDataType);
            Assert.IsNull(viewInfo.ViewType);
            Assert.IsNull(viewInfo.Description);
            Assert.IsNull(viewInfo.GetViewName);
            Assert.IsNull(viewInfo.Image);
            Assert.IsNull(viewInfo.AdditionalDataCheck);
            Assert.IsNull(viewInfo.GetViewData);
            Assert.IsNull(viewInfo.AfterCreate);
            Assert.IsNull(viewInfo.OnActivateView);
            Assert.IsNull(viewInfo.CloseForData);
        }

        [Test]
        public void SimpleProperties_SetNewValues_GetNewlySetValues()
        {
            // Setup
            var viewInfo = new ViewInfo();

            var newDataType = typeof(int);
            var newViewDataType = typeof(string);
            var viewType = typeof(StringView);
            var newDescription = "<text>";
            Func<IView, object, string> getViewNameDelegate = (view, o) => "";
            Image icon = Resources.abacus;
            Func<object, bool> additionalDataDelegate = o => true;
            Func<object, object> getViewDataDelegate = o => 45;
            Action<IView, object> afterCreateDelegate = (view, o) =>
            {
                // Do something useful
            };
            Action<IView, object> onActivateViewDelegate = (view, o) =>
            {
                // React to activation
            };
            Func<IView, object, bool> closeViewForDataDelegate = (view, o) => true;

            // Call
            viewInfo.DataType = newDataType;
            viewInfo.ViewDataType = newViewDataType;
            viewInfo.ViewType = viewType;
            viewInfo.Description = newDescription;
            viewInfo.GetViewName = getViewNameDelegate;
            viewInfo.Image = icon;
            viewInfo.AdditionalDataCheck = additionalDataDelegate;
            viewInfo.GetViewData = getViewDataDelegate;
            viewInfo.AfterCreate = afterCreateDelegate;
            viewInfo.OnActivateView = onActivateViewDelegate;
            viewInfo.CloseForData = closeViewForDataDelegate;

            // Assert
            Assert.AreEqual(newDataType, viewInfo.DataType);
            Assert.AreEqual(newViewDataType, viewInfo.ViewDataType);
            Assert.AreEqual(viewType, viewInfo.ViewType);
            Assert.AreEqual(newDescription, viewInfo.Description);
            Assert.AreEqual(getViewNameDelegate, viewInfo.GetViewName);
            Assert.AreEqual(icon, viewInfo.Image);
            Assert.AreEqual(additionalDataDelegate, viewInfo.AdditionalDataCheck);
            Assert.AreEqual(getViewDataDelegate, viewInfo.GetViewData);
            Assert.AreEqual(afterCreateDelegate, viewInfo.AfterCreate);
            Assert.AreEqual(onActivateViewDelegate, viewInfo.OnActivateView);
            Assert.AreEqual(closeViewForDataDelegate, viewInfo.CloseForData);
        }

        [Test]
        public void ToString_WithRelevantFieldsInitialized_ReturnText()
        {
            // Setup
            var viewInfo = new ViewInfo
            {
                DataType = typeof(int),
                ViewDataType = typeof(string),
                ViewType = typeof(StringView)
            };

            // Call
            var text = viewInfo.ToString();

            // Assert
            var expectedText = string.Format("{0} : {1} : {2}",
                                             viewInfo.DataType, viewInfo.ViewDataType, viewInfo.ViewType);
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void DefaultGenericConstructor_ExpectedValues()
        {
            // Call
            var viewInfo = new ViewInfo<int, string, StringView>();

            // Assert
            Assert.AreEqual(typeof(int), viewInfo.DataType);
            Assert.AreEqual(typeof(string), viewInfo.ViewDataType);
            Assert.AreEqual(typeof(StringView), viewInfo.ViewType);
            Assert.IsNull(viewInfo.Description);
            Assert.IsNull(viewInfo.GetViewName);
            Assert.IsNull(viewInfo.Image);
            Assert.IsNull(viewInfo.AdditionalDataCheck);
            Assert.IsNull(viewInfo.GetViewData);
            Assert.IsNull(viewInfo.AfterCreate);
            Assert.IsNull(viewInfo.OnActivateView);
            Assert.IsNull(viewInfo.CloseForData);
        }

        [Test]
        public void SimpleProperties_GenericViewInfoSetNewValues_GetNewlySetValues()
        {
            // Setup
            var viewInfo = new ViewInfo<int, string, StringView>();

            var newDescription = "<text>";
            Func<IView, object, string> getViewNameDelegate = (view, o) => "";
            Image icon = Resources.abacus;
            Func<int, bool> additionalDataDelegate = o => true;
            Func<int, string> getViewDataDelegate = o => o.ToString();
            Action<IView, int> afterCreateDelegate = (view, o) =>
            {
                // Do something useful
            };
            Action<IView, object> onActivateViewDelegate = (view, o) =>
            {
                // React to activation
            };
            Func<IView, object, bool> closeViewForDataDelegate = (view, o) => true;

            // Call
            viewInfo.Description = newDescription;
            viewInfo.GetViewName = getViewNameDelegate;
            viewInfo.Image = icon;
            viewInfo.AdditionalDataCheck = additionalDataDelegate;
            viewInfo.GetViewData = getViewDataDelegate;
            viewInfo.AfterCreate = afterCreateDelegate;
            viewInfo.OnActivateView = onActivateViewDelegate;
            viewInfo.CloseForData = closeViewForDataDelegate;

            // Assert
            Assert.AreEqual(newDescription, viewInfo.Description);
            Assert.AreEqual(getViewNameDelegate, viewInfo.GetViewName);
            Assert.AreEqual(icon, viewInfo.Image);
            Assert.AreEqual(additionalDataDelegate, viewInfo.AdditionalDataCheck);
            Assert.AreEqual(getViewDataDelegate, viewInfo.GetViewData);
            Assert.AreEqual(afterCreateDelegate, viewInfo.AfterCreate);
            Assert.AreEqual(onActivateViewDelegate, viewInfo.OnActivateView);
            Assert.AreEqual(closeViewForDataDelegate, viewInfo.CloseForData);
        }

        [Test]
        public void ToString_GenericViewInfoWithRelevantFieldsInitialized_ReturnText()
        {
            // Setup
            var viewInfo = new ViewInfo<int, string, StringView>();

            // Call
            var text = viewInfo.ToString();

            // Assert
            var expectedText = string.Format("{0} : {1} : {2}",
                                             viewInfo.DataType, viewInfo.ViewDataType, viewInfo.ViewType);
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void ImplicitOperator_WithAllMethodsSet_InfoFullyConverted()
        {
            // Setup
            var viewInfo = new ViewInfo<int, string, StringView>();

            var stringView = new StringView();
            var dataObject = 11;

            var newDescription = "<text>";
            const string newViewName = "<view name>";
            Func<IView, object, string> getViewNameDelegate = (view, o) =>
            {
                Assert.AreSame(stringView, view);
                Assert.AreEqual(dataObject.ToString(), o);
                return newViewName;
            };
            Image icon = Resources.abacus;
            Func<int, bool> additionalDataDelegate = o =>
            {
                Assert.AreEqual(dataObject, o);
                return true;
            };
            Func<int, string> getViewDataDelegate = o =>
            {
                Assert.AreEqual(dataObject, o);
                return o.ToString();
            };
            bool afterCreateDelegateCalled = false;
            Action<IView, int> afterCreateDelegate = (view, o) =>
            {
                Assert.AreSame(stringView, view);
                Assert.AreEqual(dataObject, o);
                afterCreateDelegateCalled = true;
            };
            bool onActivateViewDelegateCalled = false;
            Action<IView, object> onActivateViewDelegate = (view, o) =>
            {
                Assert.AreSame(stringView, view);
                Assert.AreEqual(dataObject, o);
                onActivateViewDelegateCalled = true;
            };
            Func<IView, object, bool> closeViewForDataDelegate = (view, o) =>
            {
                Assert.AreSame(stringView, view);
                Assert.AreEqual(dataObject, o);
                return true;
            };

            viewInfo.Description = newDescription;
            viewInfo.GetViewName = getViewNameDelegate;
            viewInfo.Image = icon;
            viewInfo.AdditionalDataCheck = additionalDataDelegate;
            viewInfo.GetViewData = getViewDataDelegate;
            viewInfo.AfterCreate = afterCreateDelegate;
            viewInfo.OnActivateView = onActivateViewDelegate;
            viewInfo.CloseForData = closeViewForDataDelegate;

            // Precondition
            Assert.IsInstanceOf<ViewInfo<int, string, StringView>>(viewInfo);

            // Call
            ViewInfo info = viewInfo;

            // Assert
            Assert.IsInstanceOf<ViewInfo>(info);
            Assert.AreEqual(typeof(int), info.DataType);
            Assert.AreEqual(typeof(string), info.ViewDataType);
            Assert.AreEqual(typeof(StringView), info.ViewType);
            Assert.AreEqual(newDescription, info.Description);
            Assert.AreEqual(newViewName, info.GetViewName(stringView, dataObject.ToString()));
            Assert.AreEqual(icon, info.Image);
            Assert.IsTrue(viewInfo.AdditionalDataCheck(dataObject));
            Assert.AreEqual(dataObject.ToString(), viewInfo.GetViewData(dataObject));

            viewInfo.AfterCreate(stringView, dataObject);
            Assert.IsTrue(afterCreateDelegateCalled);

            viewInfo.OnActivateView(stringView, dataObject);
            Assert.IsTrue(onActivateViewDelegateCalled);

            Assert.IsTrue(viewInfo.CloseForData(stringView, dataObject));
        }

        private class StringView : IView
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public object Data { get; set; }
            public string Text { get; set; }
        }
    }
}