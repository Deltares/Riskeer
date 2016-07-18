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

using Application.Ringtoets.Storage.DbContext;

using NUnit.Framework;

using Ringtoets.GrassCoverErosionInwards.Data;

using Application.Ringtoets.Storage.Read.GrassCoverErosionInwards;

namespace Application.Ringtoets.Storage.Test.Read.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_ValidEntity_ReturnCalculation()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = 457,
                Name = "sodhfksn",
                Comments = "s;ohfgwjo5p09u"
            };

            // Call
            GrassCoverErosionInwardsCalculation calculation = entity.Read();

            // Assert
            Assert.AreEqual(entity.GrassCoverErosionInwardsCalculationEntityId, calculation.StorageId);
            Assert.AreEqual(entity.Name, calculation.Name);
            Assert.AreEqual(entity.Comments, calculation.Comments);
        }
    }
}