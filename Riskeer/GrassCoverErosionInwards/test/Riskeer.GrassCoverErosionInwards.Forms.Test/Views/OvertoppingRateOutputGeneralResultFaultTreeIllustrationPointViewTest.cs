﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.Threading;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointViewTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var view = new OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView(
                new GrassCoverErosionInwardsCalculation(),
                () => new TestGeneralResultFaultTreeIllustrationPoint());

            // Assert
            Assert.IsInstanceOf<GeneralResultFaultTreeIllustrationPointView>(view);
        }
    }
}