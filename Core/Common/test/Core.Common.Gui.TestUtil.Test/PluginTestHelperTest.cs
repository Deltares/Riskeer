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

using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;

namespace Core.Common.Gui.TestUtil.Test
{
    [TestFixture]
    public class PluginTestHelperTest
    {
        [Test]
        public void AssertPropertyInfoDefined_NullInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertPropertyInfoDefined(null, typeof(object), typeof(object));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertPropertyInfoDefined_NoInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertPropertyInfoDefined(new PropertyInfo[0], typeof(object), typeof(object));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertPropertyInfoDefined_NoMatchingInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertPropertyInfoDefined(new PropertyInfo[]
            {
                new PropertyInfo<int, IObjectProperties>()
            }, typeof(object), typeof(object));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertPropertyInfoDefined_MultipleInfosSingleMatching_ReturnsMatchingInfoFromList()
        {
            // Call
            PropertyInfo foundInfo = PluginTestHelper.AssertPropertyInfoDefined(new[]
            {
                new PropertyInfo(),
                new PropertyInfo<int, IObjectProperties>()
            }, typeof(int), typeof(IObjectProperties));

            // Assert
            Assert.AreEqual(typeof(int), foundInfo.DataType);
            Assert.AreEqual(typeof(IObjectProperties), foundInfo.PropertyObjectType);
        }

        [Test]
        public void AssertViewInfoDefined_WithoutViewDataTypeNullInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertViewInfoDefined(null, typeof(object), typeof(object));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertViewInfoDefined_WithoutViewDataTypeNoInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertViewInfoDefined(new ViewInfo[0], typeof(object), typeof(object));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertViewInfoDefined_WithoutViewDataTypeNoMatchingInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertViewInfoDefined(new ViewInfo[]
            {
                new ViewInfo<int, IView>()
            }, typeof(object), typeof(object));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertViewInfoDefined_WithoutViewDataTypeMultipleInfosSingleMatching_ReturnsMatchingInfoFromList()
        {
            // Call
            ViewInfo foundInfo = PluginTestHelper.AssertViewInfoDefined(new[]
            {
                new ViewInfo(),
                new ViewInfo<int, IView>()
            }, typeof(int), typeof(IView));

            // Assert
            Assert.AreEqual(typeof(int), foundInfo.DataType);
            Assert.AreEqual(typeof(IView), foundInfo.ViewType);
        }

        [Test]
        public void AssertViewInfoDefined_WithViewDataTypeNullInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertViewInfoDefined(null, typeof(object), typeof(object), typeof(object));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertViewInfoDefined_WithViewDataTypeNoInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertViewInfoDefined(new ViewInfo[0], typeof(object), typeof(object), typeof(object));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertViewInfoDefined_WithViewDataTypeNoMatchingInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertViewInfoDefined(new ViewInfo[]
            {
                new ViewInfo<int, IView>()
            }, typeof(object), typeof(object), typeof(object));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertViewInfoDefined_WithViewDataTypeMultipleInfosDifferentViewDataType_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => PluginTestHelper.AssertViewInfoDefined(new[]
            {
                new ViewInfo(),
                new ViewInfo<int, string, IView>()
            }, typeof(int), typeof(object), typeof(IView));

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertViewInfoDefined_WithViewDataTypeMultipleInfosSingleMatching_ReturnsMatchingInfoFromList()
        {
            // Call
            ViewInfo foundInfo = PluginTestHelper.AssertViewInfoDefined(new[]
            {
                new ViewInfo(),
                new ViewInfo<int, string, IView>()
            }, typeof(int), typeof(string), typeof(IView));

            // Assert
            Assert.AreEqual(typeof(int), foundInfo.DataType);
            Assert.AreEqual(typeof(string), foundInfo.ViewDataType);
            Assert.AreEqual(typeof(IView), foundInfo.ViewType);
        }
    }
}