// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Drawing;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Gui.TestUtil
{
    public abstract class ViewInfoTest<TView> where TView : IDisposable, IView
    {
        protected Type DataType;
        protected Type ViewDataType;
        protected Image ViewIcon;
        protected string ViewName;
        protected PluginBase Plugin;

        [SetUp]
        public void SetUp()
        {
            Plugin = CreatePlugin();
            Info = Plugin.GetViewInfos().FirstOrDefault(vi => vi.ViewType == typeof(TView));
            if (ViewDataType == null)
            {
                ViewDataType = DataType;
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (Plugin != null)
            {
                Plugin.Dispose();
            }
        }

        [TestCase]
        public void Initialized_Always_DataTypeAndViewTypeAsExpected()
        {
            Assert.NotNull(Info, "Expected a viewInfo definition for views with type {0}.", typeof(TView));
            Assert.AreEqual(DataType, Info.DataType);
            Assert.AreEqual(ViewDataType, Info.ViewDataType);
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var expectedViewType = Info.ViewType;

            // Assert
            Assert.AreEqual(typeof(TView), expectedViewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var expectedDataType = Info.DataType;

            // Assert
            Assert.AreEqual(DataType, expectedDataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            Type expectedViewDataType = Info.ViewDataType;

            // Assert
            Assert.AreEqual(ViewDataType, expectedViewDataType);
        }

        [Test]
        public void Image_Always_ReturnsViewIcon()
        {
            // Call
            Image image = Info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(ViewIcon, image);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            using (var view = CreateView())
            {
                // Call
                var expectedViewName = Info.GetViewName(view, null);

                // Assert
                Assert.AreEqual(ViewName, expectedViewName);
            }
        }

        [Test]
        [TestCaseSource("CloseForDataTests")]
        public void CloseForData_ForDifferentObjects_ReturnsExpectedValue(CloseForDataTest test)
        {
            using (var view = CreateView())
            {
                // Call
                var closeForData = Info.CloseForData(view, test.DataToCloseFor);

                // Assert
                Assert.AreEqual(test.ExpectedResult, closeForData);
            }
        }

        protected virtual IEnumerable<CloseForDataTest> CloseForDataTests
        {
            get
            {
                return new[]
                {
                    new CloseForDataTest()
                };
            }
        }

        protected abstract PluginBase CreatePlugin();
        protected abstract TView CreateView();
        protected ViewInfo Info { get; private set; }

        public class CloseForDataTest
        {
            public bool ExpectedResult { get; set; }
            public object DataToCloseFor { get; set; }
        }
    }
}