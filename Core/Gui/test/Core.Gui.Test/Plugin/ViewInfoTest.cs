// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Windows.Media;
using Core.Common.Controls.Views;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Gui.Test.Plugin
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
            Assert.IsNull(viewInfo.GetSymbol);
            Assert.IsNull(viewInfo.GetFontFamily);
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
            string GetViewName(IView view, object o) => "";
            string GetSymbol() => "<symbol>";
            FontFamily GetFontFamily() => new FontFamily();
            bool AdditionalDataCheck(object o) => true;
            object GetViewData(object o) => 45;
            void AfterCreate(IView view, object o) {}
            bool CloseForData(IView view, object o) => true;
            IView CreateInstance(object o) => viewInstance;

            // Call
            viewInfo.DataType = newDataType;
            viewInfo.ViewDataType = newViewDataType;
            viewInfo.ViewType = viewType;
            viewInfo.Description = newDescription;
            viewInfo.GetViewName = GetViewName;
            viewInfo.GetSymbol = GetSymbol;
            viewInfo.GetFontFamily = GetFontFamily;
            viewInfo.AdditionalDataCheck = AdditionalDataCheck;
            viewInfo.GetViewData = GetViewData;
            viewInfo.AfterCreate = AfterCreate;
            viewInfo.CloseForData = CloseForData;
            viewInfo.CreateInstance = CreateInstance;

            // Assert
            Assert.AreEqual(newDataType, viewInfo.DataType);
            Assert.AreEqual(newViewDataType, viewInfo.ViewDataType);
            Assert.AreEqual(viewType, viewInfo.ViewType);
            Assert.AreEqual(newDescription, viewInfo.Description);
            Assert.AreEqual((Func<IView, object, string>) GetViewName, viewInfo.GetViewName);
            Assert.AreEqual((Func<string>) GetSymbol, viewInfo.GetSymbol);
            Assert.AreEqual((Func<FontFamily>) GetFontFamily, viewInfo.GetFontFamily);
            Assert.AreEqual((Func<object, bool>) AdditionalDataCheck, viewInfo.AdditionalDataCheck);
            Assert.AreEqual((Func<object, object>) GetViewData, viewInfo.GetViewData);
            Assert.AreEqual((Action<IView, object>) AfterCreate, viewInfo.AfterCreate);
            Assert.AreEqual((Func<IView, object, bool>) CloseForData, viewInfo.CloseForData);
            Assert.AreEqual((Func<object, IView>) CreateInstance, viewInfo.CreateInstance);
        }

        [Test]
        public void CreateInstance_ViewTypeWithDefaultConstructor_ReturnView()
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
            var text = viewInfo.ToString();

            // Assert
            var expectedText = $"{viewInfo.DataType} : {viewInfo.ViewDataType} : {viewInfo.ViewType}";
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
            Assert.IsNull(viewInfo.GetSymbol);
            Assert.IsNull(viewInfo.GetFontFamily);
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
            string GetViewName(IView view, int o) => "";
            string GetSymbol() => "<symbol>";
            FontFamily GetFontFamily() => new FontFamily();
            bool AdditionalDataCheck(int o) => true;
            string GetViewData(int o) => o.ToString();
            void AfterCreate(IView view, int o) {}
            bool CloseForData(IView view, object o) => true;
            StringView CreateInstance(int o) => new StringView();

            // Call
            viewInfo.Description = newDescription;
            viewInfo.GetViewName = (Func<IView, int, string>) GetViewName;
            viewInfo.GetSymbol = GetSymbol;
            viewInfo.GetFontFamily = GetFontFamily;
            viewInfo.AdditionalDataCheck = AdditionalDataCheck;
            viewInfo.GetViewData = GetViewData;
            viewInfo.AfterCreate = (Action<IView, int>) AfterCreate;
            viewInfo.CloseForData = (Func<IView, object, bool>) CloseForData;
            viewInfo.CreateInstance = CreateInstance;

            // Assert
            Assert.AreEqual(newDescription, viewInfo.Description);
            Assert.AreEqual((Func<IView, int, string>) GetViewName, viewInfo.GetViewName);
            Assert.AreEqual((Func<string>) GetSymbol, viewInfo.GetSymbol);
            Assert.AreEqual((Func<FontFamily>) GetFontFamily, viewInfo.GetFontFamily);
            Assert.AreEqual((Func<int, bool>) AdditionalDataCheck, viewInfo.AdditionalDataCheck);
            Assert.AreEqual((Func<int, string>) GetViewData, viewInfo.GetViewData);
            Assert.AreEqual((Action<IView, int>) AfterCreate, viewInfo.AfterCreate);
            Assert.AreEqual((Func<IView, object, bool>) CloseForData, viewInfo.CloseForData);
        }

        [Test]
        public void ToString_GenericViewInfoWithRelevantFieldsInitialized_ReturnText()
        {
            // Setup
            var viewInfo = new ViewInfo<int, string, StringView>();

            // Call
            var text = viewInfo.ToString();

            // Assert
            var expectedText = $"{viewInfo.DataType} : {viewInfo.ViewDataType} : {viewInfo.ViewType}";
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
            const string symbol = "<symbol>";
            var fontFamily = new FontFamily();

            string GetViewName(IView view, int o)
            {
                Assert.AreSame(stringView, view);
                Assert.AreEqual(dataObject, o);
                return newViewName;
            }

            string GetSymbol() => symbol;
            FontFamily GetFontFamily() => fontFamily;

            bool AdditionalDataCheck(int o)
            {
                Assert.AreEqual(dataObject, o);
                return true;
            }

            string GetViewData(int o)
            {
                Assert.AreEqual(dataObject, o);
                return o.ToString();
            }

            var afterCreateCalled = false;

            void AfterCreate(IView view, int o)
            {
                Assert.AreSame(stringView, view);
                Assert.AreEqual(dataObject, o);
                afterCreateCalled = true;
            }

            bool CloseForData(IView view, object o)
            {
                Assert.AreSame(stringView, view);
                Assert.AreEqual(dataObject, o);
                return true;
            }

            viewInfo.Description = newDescription;
            viewInfo.GetViewName = (Func<IView, int, string>) GetViewName;
            viewInfo.GetSymbol = GetSymbol;
            viewInfo.GetFontFamily = GetFontFamily;
            viewInfo.AdditionalDataCheck = AdditionalDataCheck;
            viewInfo.GetViewData = GetViewData;
            viewInfo.AfterCreate = (Action<IView, int>) AfterCreate;
            viewInfo.CloseForData = (Func<IView, object, bool>) CloseForData;
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
            Assert.AreEqual(symbol, info.GetSymbol());
            Assert.AreSame(fontFamily, info.GetFontFamily());
            Assert.IsTrue(viewInfo.AdditionalDataCheck(dataObject));
            Assert.AreEqual(dataObject.ToString(), viewInfo.GetViewData(dataObject));
            Assert.AreEqual("A", viewInfo.CreateInstance(dataObject).Text);

            viewInfo.AfterCreate(stringView, dataObject);
            Assert.IsTrue(afterCreateCalled);
        }

        private class StringView : IView
        {
            public string Text { get; set; }

            public object Data { get; set; }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}