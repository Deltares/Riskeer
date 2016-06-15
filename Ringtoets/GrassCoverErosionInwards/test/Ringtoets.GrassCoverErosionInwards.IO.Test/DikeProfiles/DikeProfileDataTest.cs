// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.DikeProfiles
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
            Assert.IsNaN(dikeProfileData.CrestLevel);
            Assert.AreEqual(ProfileType.Coordinates, dikeProfileData.ProfileType);
            Assert.IsNull(dikeProfileData.Memo);
            CollectionAssert.IsEmpty(dikeProfileData.ForeshoreGeometry);
            CollectionAssert.IsEmpty(dikeProfileData.DikeGeometry);
        }

        [Test]
        public void Id_SetNewValue_GetNewlySetValue()
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            string coolText = "haha";

            // Call
            dikeProfileData.Id = coolText;

            // Assert
            Assert.AreEqual(coolText, dikeProfileData.Id);
        }

        [Test]
        public void Orientation_SetNewValue_GetNewlySetValue([Random(0, 360.0, 1)]double newValue)
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            // Call
            dikeProfileData.Orientation = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.Orientation);
        }

        [Test]
        public void DamType_SetNewValue_GetsNewlySetValue([Random(0, 3, 1)]int index)
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            DamType newValue = Enum.GetValues(typeof(DamType)).OfType<DamType>().ElementAt(index);

            // Call
            dikeProfileData.DamType = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.DamType);
        }

        [Test]
        public void DamHeight_SetNewValue_GetNewlySetValue([Random(-999.999, 999.999, 1)]double newValue)
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            // Call
            dikeProfileData.DamHeight = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.DamHeight);
        }

        [Test]
        public void CrestLevel_SetNewValue_GetNewlySetValue([Random(-999.999, 999.999, 1)]double newValue)
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            // Call
            dikeProfileData.CrestLevel = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.CrestLevel);
        }

        [Test]
        public void ProfileType_SetNewValue_GetsNewlySetValue([Random(0, 3, 1)]int index)
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            ProfileType newValue = Enum.GetValues(typeof(ProfileType)).OfType<ProfileType>().ElementAt(index);

            // Call
            dikeProfileData.ProfileType = newValue;

            // Assert
            Assert.AreEqual(newValue, dikeProfileData.ProfileType);
        }

        [Test]
        public void Memo_SetNewValue_GetNewlySetValue()
        {
            // Setup
            var dikeProfileData = new DikeProfileData();

            string coolText = "hihi";

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
                new RoughnessProfileSection(new Point2D(0, 0), new Point2D(1, 1), 1.0),
                new RoughnessProfileSection(new Point2D(1, 1), new Point2D(3, 3), 0.9),
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
                new RoughnessProfileSection(new Point2D(0, 0), new Point2D(1, 1), 1.0),
                new RoughnessProfileSection(new Point2D(1, 1), new Point2D(3, 3), 0.9),
            };

            // Call
            dikeProfileData.DikeGeometry = newValue;

            // Assert
            Assert.AreSame(newValue, dikeProfileData.DikeGeometry);
        }
    }
}