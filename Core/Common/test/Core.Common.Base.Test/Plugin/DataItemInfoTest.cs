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

using System.Drawing;
using Core.Common.Base.Plugin;
using NUnit.Framework;

namespace Core.Common.Base.Test.Plugin
{
    [TestFixture]
    public class DataItemInfoTest
    {
        [Test]
        public void DefaultConstructor_NonGeneric_ExpectedValues()
        {
            // Call
            var dataItemInfo = new DataItemInfo();

            // Assert
            Assert.IsNull(dataItemInfo.ValueType);
            Assert.AreEqual("", dataItemInfo.Name);
            Assert.AreEqual("", dataItemInfo.Category);
            Assert.IsNull(dataItemInfo.Image);
            Assert.IsNull(dataItemInfo.AdditionalOwnerCheck);
            Assert.IsNull(dataItemInfo.CreateData);
        }

        [Test]
        public void DefaultConstructor_Generic_ExpectedValues()
        {
            // Call
            var dataItemInfo = new DataItemInfo<double>();

            // Assert
            Assert.AreEqual(typeof(double), dataItemInfo.ValueType);
            Assert.AreEqual("", dataItemInfo.Name);
            Assert.AreEqual("", dataItemInfo.Category);
            Assert.IsNull(dataItemInfo.Image);
            Assert.IsNull(dataItemInfo.AdditionalOwnerCheck);
            Assert.IsNull(dataItemInfo.CreateData);
        }

        [Test]
        public void GetSetAutomaticProperties_NonGeneric_ExpectedBehavior()
        {
            // Setup / Call
            var dataItemInfo = new DataItemInfo
            {
                ValueType = typeof(double),
                Name = "Some double",
                Category = "Nice category",
                Image = new Bitmap(16, 16),
                AdditionalOwnerCheck = o => true,
                CreateData = o => 1.2,
            };

            // Assert
            Assert.AreEqual(typeof(double), dataItemInfo.ValueType);
            Assert.AreEqual("Some double", dataItemInfo.Name);
            Assert.AreEqual("Nice category", dataItemInfo.Category);
            Assert.IsNotNull(dataItemInfo.Image);
            Assert.IsTrue(dataItemInfo.AdditionalOwnerCheck(null));
            Assert.AreEqual(1.2, dataItemInfo.CreateData(null));
        }

        [Test]
        public void GetSetAutomaticProperties_Generic_ExpectedBehavior()
        {
            // Setup / Call
            var dataItemInfo = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(16, 16),
                AdditionalOwnerCheck = o => false,
                CreateData = o => -1,
            };

            // assert
            Assert.AreEqual(typeof(int), dataItemInfo.ValueType);
            Assert.AreEqual("Some integer", dataItemInfo.Name);
            Assert.AreEqual("Better category", dataItemInfo.Category);
            Assert.IsNotNull(dataItemInfo.Image);
            Assert.IsFalse(dataItemInfo.AdditionalOwnerCheck(null));
            Assert.AreEqual(-1, dataItemInfo.CreateData(null));
        }

        [Test]
        public void ImplicitConversion_FromGenericToNonGeneric_ShouldCopyValues()
        {
            // Setup
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(16, 16),
                AdditionalOwnerCheck = o => false,
                CreateData = o => -1,
            };

            // Call
            var nonGenericInfo = (DataItemInfo) info;

            // Assert
            Assert.AreEqual(info.ValueType, nonGenericInfo.ValueType);
            Assert.AreEqual(info.Name, nonGenericInfo.Name);
            Assert.AreEqual(info.Category, nonGenericInfo.Category);
            Assert.AreEqual(info.AdditionalOwnerCheck(1), nonGenericInfo.AdditionalOwnerCheck(1));
            Assert.AreEqual(info.CreateData(null), nonGenericInfo.CreateData(null));
        }

        [Test]
        public void ImplicitConversion_FromGenericToNonGenericWithoutMethodsSet_MethodsShouldBeNull()
        {
            // Setup
            var info = new DataItemInfo<int>
            {
                Name = "Some integer",
                Category = "Better category",
                Image = new Bitmap(16, 16)
            };

            // Call
            var nonGenericInfo = (DataItemInfo) info;

            // Assert
            Assert.AreEqual(info.ValueType, nonGenericInfo.ValueType);
            Assert.AreEqual(info.Name, nonGenericInfo.Name);
            Assert.AreEqual(info.Category, nonGenericInfo.Category);
            Assert.IsNull(nonGenericInfo.AdditionalOwnerCheck);
            Assert.IsNull(nonGenericInfo.CreateData);
        }
    }
}