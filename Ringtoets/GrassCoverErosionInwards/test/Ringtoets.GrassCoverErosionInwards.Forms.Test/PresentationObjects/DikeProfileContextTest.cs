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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DikeProfileContextTest
    {
        [Test]
        public void Constructor_ValidValues_ExpectedValues()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));
            var dikeProfilesList = new ObservableList<DikeProfile>();

            // Call
            var context = new DikeProfileContext(dikeProfile, dikeProfilesList);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<DikeProfile>>(context);
            Assert.AreSame(dikeProfile, context.WrappedData);
            Assert.AreSame(dikeProfilesList, context.DikeProfilesList);
        }

        [Test]
        public void Constructor_DikeProfileIsNull_ThrowArgumentNullException()
        {
            // Setup
            var dikeProfilesList = new ObservableList<DikeProfile>();

            // Call
            TestDelegate call = () => new DikeProfileContext(null, dikeProfilesList);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wrappedData", paramName);
        }

        [Test]
        public void Constructor_DikeProfilesListIsNull_ThrowArgumentNullException()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0));

            // Call
            TestDelegate call = () => new DikeProfileContext(dikeProfile, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("dikeProfilesList", paramName);
        }
    }
}
