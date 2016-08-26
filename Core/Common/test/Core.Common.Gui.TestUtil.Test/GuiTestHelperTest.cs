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

using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;

namespace Core.Common.Gui.TestUtil.Test
{
    [TestFixture]
    public class GuiTestHelperTest
    {
        [Test]
        public void AssertPropertyInfoDefined_NullInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => GuiTestHelper.AssertPropertyInfoDefined<object, object>(null);
            
            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertPropertyInfoDefined_NoInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => GuiTestHelper.AssertPropertyInfoDefined<object, object>(new PropertyInfo[0]);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertPropertyInfoDefined_NoMatchingInfos_ThrowsAssertionException()
        {
            // Call
            TestDelegate test = () => GuiTestHelper.AssertPropertyInfoDefined<object, object>(new PropertyInfo[]
            {
                new PropertyInfo<int,IObjectProperties>()
            });

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertPropertyInfoDefined_SingleMatchingInfo_DoesNotThrow()
        {
            // Call
            TestDelegate test = () => GuiTestHelper.AssertPropertyInfoDefined<int, IObjectProperties>(new PropertyInfo[]
            {
                new PropertyInfo<int,IObjectProperties>()
            });

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void AssertPropertyInfoDefined_MultipleInfosSingleMatching_DoesNotThrow()
        {
            // Call
            var foundInfo = GuiTestHelper.AssertPropertyInfoDefined<int, IObjectProperties>(new []
                                          {
                                              new PropertyInfo(), 
                                              new PropertyInfo<int, IObjectProperties>()
                                          });

            // Assert
            Assert.AreEqual(typeof(int), foundInfo.DataType);
            Assert.AreEqual(typeof(IObjectProperties), foundInfo.PropertyObjectType);
        }
    }
}