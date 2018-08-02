// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.DikeProfiles;

namespace Ringtoets.Common.IO.Test.DikeProfiles
{
    [TestFixture]
    public class DikeProfileDataTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var dikeProfileData = new DikeProfileData();

            // Assert
            Assert.IsNull(dikeProfileData.Id);
            Assert.IsNaN(dikeProfileData.Orientation);
            Assert.AreEqual(DamType.None, dikeProfileData.DamType);
            Assert.IsNaN(dikeProfileData.DamHeight);
            Assert.IsNaN(dikeProfileData.DikeHeight);
            Assert.AreEqual(SheetPileType.Coordinates, dikeProfileData.SheetPileType);
            Assert.IsNull(dikeProfileData.Memo);
            CollectionAssert.IsEmpty(dikeProfileData.ForeshoreGeometry);
            CollectionAssert.IsEmpty(dikeProfileData.DikeGeometry);
        }

        [Test]
        public void Id_SetNewValue_GetNewlySetValue()
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            const string coolText = "haha";

            // Call
            dikeProfileData.Id = coolText;

            // Assert
            Assert.AreEqual(coolText, dikeProfileData.Id);
        }

        [Test]
        public void Orientation_SetNewValue_GetNewlySetValue()
        {
            // Setup
            double newValue = new Random(21).NextDouble();
            var dikeProfileData = new DikeProfileData();

            // Call
            dikeProfileData.Orientation = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.Orientation);
        }

        [Test]
        public void DamType_SetNewValue_GetsNewlySetValue()
        {
            // Setup
            int index = new Random(21).Next(0, 3);
            var dikeProfileData = new DikeProfileData();

            DamType newValue = Enum.GetValues(typeof(DamType)).OfType<DamType>().ElementAt(index);

            // Call
            dikeProfileData.DamType = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.DamType);
        }

        [Test]
        public void DamHeight_SetNewValue_GetNewlySetValue()
        {
            // Setup
            double newValue = new Random(21).NextDouble();
            var dikeProfileData = new DikeProfileData();

            // Call
            dikeProfileData.DamHeight = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.DamHeight);
        }

        [Test]
        public void DikeHeight_SetNewValue_GetNewlySetValue()
        {
            // Setup
            double newValue = new Random(21).NextDouble();
            var dikeProfileData = new DikeProfileData();

            // Call
            dikeProfileData.DikeHeight = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.DikeHeight);
        }

        [Test]
        public void SheetPilingType_SetNewValue_GetsNewlySetValue()
        {
            // Setup
            int index = new Random(21).Next(0, 3);
            var dikeProfileData = new DikeProfileData();

            SheetPileType newValue = Enum.GetValues(typeof(SheetPileType)).OfType<SheetPileType>().ElementAt(index);

            // Call
            dikeProfileData.SheetPileType = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.SheetPileType);
        }

        [Test]
        public void Memo_SetNewValue_GetNewlySetValue()
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            const string coolText = "hihi";

            // Call
            dikeProfileData.Memo = coolText;

            // Assert
            Assert.AreEqual(coolText, dikeProfileData.Memo);
        }

        [Test]
        public void ForeshoreGeometry_SetNewArray_GetsNewlySetArray()
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            var newValue = new[]
            {
                new RoughnessPoint(new Point2D(0, 0), 1.0),
                new RoughnessPoint(new Point2D(1, 1), 0.9)
            };

            // Call
            dikeProfileData.ForeshoreGeometry = newValue;

            // Assert
            Assert.AreSame(newValue, dikeProfileData.ForeshoreGeometry);
        }

        [Test]
        public void DikeGeometry_SetNewArray_GetsNewlySetArray()
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            var newValue = new[]
            {
                new RoughnessPoint(new Point2D(1, 1), 1.0),
                new RoughnessPoint(new Point2D(3, 3), 0.9)
            };

            // Call
            dikeProfileData.DikeGeometry = newValue;

            // Assert
            Assert.AreSame(newValue, dikeProfileData.DikeGeometry);
        }
    }
}