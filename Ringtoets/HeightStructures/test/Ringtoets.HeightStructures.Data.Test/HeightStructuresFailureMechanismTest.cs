﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HeightStructures.Data.Properties;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase<FailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual(Resources.HeightStructureFailureMechanism_DisplayName, failureMechanism.Name);
            Assert.AreEqual(Resources.HeightStructureFailureMechanism_Code, failureMechanism.Code);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
            Assert.AreEqual("Berekeningen", failureMechanism.CalculationsGroup.Name);
            Assert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            Assert.IsInstanceOf<GeneralNormProbabilityInput>(failureMechanism.NormProbabilityInput);
        }

        [Test]
        public void AddSection_WithSection_AddedCustomFailureMechanismResult()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            
            // Call
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));

            // Assert
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.IsInstanceOf<FailureMechanismSectionResult>(failureMechanism.SectionResults.ElementAt(0));
        }
    }
}