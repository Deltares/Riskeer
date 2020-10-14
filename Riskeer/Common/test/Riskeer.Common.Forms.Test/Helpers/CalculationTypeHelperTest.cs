// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.ComponentModel;
using System.Drawing;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TreeNodeInfos;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class CalculationTypeHelperTest
    {
        [Test]
        public void GetCalculationTypeImage_InvalidCalculationType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const CalculationType calculationType = (CalculationType) 99;

            // Call
            void Call() => CalculationTypeHelper.GetCalculationTypeImage(calculationType);

            // Assert
            var expectedMessage = $"The value of argument 'calculationType' ({calculationType}) is invalid for Enum type '{nameof(CalculationType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCaseSource(typeof(CalculationTypeTestHelper), nameof(CalculationTypeTestHelper.CalculationTypeWithImageCases))]
        public void GetCalculationTypeImage_ValidCalculationType_ReturnsImage(CalculationType calculationType, Bitmap expectedImage)
        {
            // Call
            Bitmap image = CalculationTypeHelper.GetCalculationTypeImage(calculationType);
            
            // Assert
            TestHelper.AssertImagesAreEqual(expectedImage, image);
        }
    }
}