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

using System;
using System.Drawing;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Test.Properties;
using NUnit.Framework;
using Rhino.Mocks;

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
            Assert.IsNull(viewInfo.CloseForData);
            Assert.IsNotNull(viewInfo.CreateInstance);
        }

        [Test]
        public void SimpleProperties_SetNewValues_GetNewlySetValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewInstance = mocks.Stub<IView>();
            mocks.ReplayAll();

            var viewInfo = new ViewInfo();

            Type newDataType = typeof(int);
            Type newViewDataType = typeof(string);
            Type viewType = typeof(StringView);
            const string newDescription = "<text>";
            Func<IView, object, string> getViewNameDelegate = (view, o) => "";
            Image icon = Resources.abacus;
            Func<object, bool> additionalDataDelegate = o => true;
            Func<object, object> getViewDataDelegate = o => 45;
            Action<IView, object> afterCreateDelegate = (view, o) =>
            {
                // Do something useful
            };
            Func<IView, object, bool> closeViewForDataDelegate = (view, o) => true;
            Func<object, IView> createInstanceDelegate = o => viewInstance;

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
            viewInfo.CloseForData = closeViewForDataDelegate;
            viewInfo.CreateInstance = createInstanceDelegate;

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
            Assert.AreEqual(closeViewForDataDelegate, viewInfo.CloseForData);
            Assert.AreEqual(createInstanceDelegate, viewInfo.CreateInstance);
        }

        [Test]
        public void CreateInstance_ViewtypeWithDefaultConstructor_ReturnView()
        {
            // Setup
            var viewInfo = new ViewInfo
            {
                DataType = typeof(int),
                ViewDataType = typeof(string),
                ViewType = typeof(StringView)
            };

            int data = new Random(21).Next();

            // Call
            IView view = viewInfo.CreateInstance(data);

            // Assert
            Assert.IsInstanceOf<StringView>(view);
            Assert.IsNull(view.Data);
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
            string text = viewInfo.ToString();

            // Assert
            string expectedText = $"{viewInfo.DataType} : {viewInfo.ViewDataType} : {viewInfo.ViewType}";
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
            Assert.IsNull(viewInfo.CloseForData);
            Assert.IsNotNull(viewInfo.CreateInstance);
        }

        [Test]
        public void SimpleProperties_GenericViewInfoSetNewValues_GetNewlySetValues()
        {
            // Setup
            var viewInfo = new ViewInfo<int, string, StringView>();

            const string newDescription = "<text>";
            Func<IView, int, string> getViewNameDelegate = (view, o) => "";
            Image icon = Resources.abacus;
            Func<int, bool> additionalDataDelegate = o => true;
            Func<int, string> getViewDataDelegate = o => o.ToString();
            Action<IView, int> afterCreateDelegate = (view, o) =>
            {
                // Do something useful
            };
            Func<IView, object, bool> closeViewForDataDelegate = (view, o) => true;
            Func<int, StringView> createInstanceDelegate = o => new StringView();

            // Call
            viewInfo.Description = newDescription;
            viewInfo.GetViewName = getViewNameDelegate;
            viewInfo.Image = icon;
            viewInfo.AdditionalDataCheck = additionalDataDelegate;
            viewInfo.GetViewData = getViewDataDelegate;
            viewInfo.AfterCreate = afterCreateDelegate;
            viewInfo.CloseForData = closeViewForDataDelegate;
            viewInfo.CreateInstance = createInstanceDelegate;

            // Assert
            Assert.AreEqual(newDescription, viewInfo.Description);
            Assert.AreEqual(getViewNameDelegate, viewInfo.GetViewName);
            Assert.AreEqual(icon, viewInfo.Image);
            Assert.AreEqual(additionalDataDelegate, viewInfo.AdditionalDataCheck);
            Assert.AreEqual(getViewDataDelegate, viewInfo.GetViewData);
            Assert.AreEqual(afterCreateDelegate, viewInfo.AfterCreate);
            Assert.AreEqual(closeViewForDataDelegate, viewInfo.CloseForData);
        }

        [Test]
        public void ToString_GenericViewInfoWithRelevantFieldsInitialized_ReturnText()
        {
            // Setup
            var viewInfo = new ViewInfo<int, string, StringView>();

            // Call
            string text = viewInfo.ToString();

            // Assert
            string expectedText = $"{viewInfo.DataType} : {viewInfo.ViewDataType} : {viewInfo.ViewType}";
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void CreateInstance_ViewTypeHasDefaultConstructor_ReturnView()
        {
            // Setup
            var viewInfo = new ViewInfo<int, string, StringView>();
            int data = new Random(21).Next();

            // Call
            StringView view = viewInfo.CreateInstance(data);

            // Assert
            Assert.IsNotNull(view);
            Assert.IsNull(view.Data);
        }

        [Test]
        public void ImplicitOperator_WithAllMethodsSet_InfoFullyConverted()
        {
            // Setup
            var viewInfo = new ViewInfo<int, string, StringView>();

            var stringView = new StringView();
            const int dataObject = 11;

            const string newDescription = "<text>";
            const string newViewName = "<view name>";
            Func<IView, int, string> getViewNameDelegate = (view, o) =>
            {
                Assert.AreSame(stringView, view);
                Assert.AreEqual(dataObject, o);
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
            var afterCreateDelegateCalled = false;
            Action<IView, int> afterCreateDelegate = (view, o) =>
            {
                Assert.AreSame(stringView, view);
                Assert.AreEqual(dataObject, o);
                afterCreateDelegateCalled = true;
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
            viewInfo.CloseForData = closeViewForDataDelegate;
            viewInfo.CreateInstance = o => new StringView
            {
                Text = "A"
            };

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
            Assert.AreEqual(newViewName, info.GetViewName(stringView, dataObject));
            Assert.AreEqual(icon, info.Image);
            Assert.IsTrue(viewInfo.AdditionalDataCheck(dataObject));
            Assert.AreEqual(dataObject.ToString(), viewInfo.GetViewData(dataObject));
            Assert.AreEqual("A", viewInfo.CreateInstance(dataObject).Text);

            viewInfo.AfterCreate(stringView, dataObject);
            Assert.IsTrue(afterCreateDelegateCalled);
        }

        private class StringView : IView
        {
            public object Data { get; set; }
            public string Text { get; set; }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}